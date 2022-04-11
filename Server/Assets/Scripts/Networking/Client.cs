using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using PacketExt;

using UnityEngine;

public class Client: MonoBehaviour
{
	public const int dataBufferSize = 4096;
	
	public ushort id;

	public Player player;
	
	public TcpClient socket;
	public NetworkStream stream;
	public Packet receivedData;
	public byte[] receiveBuffer;
	public bool handshakeDone;

	public void Connect(TcpClient client) {
		socket = client;
		socket.ReceiveBufferSize = dataBufferSize;
		socket.SendBufferSize = dataBufferSize;

		stream = socket.GetStream();

		receivedData = new Packet();
		receiveBuffer = new byte[dataBufferSize];

		stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveHandshakeCallback, null);
		print($"client {id} found");
	}

	void ReceiveHandshakeCallback(IAsyncResult result) {
		int len = stream.EndRead(result);

		if (len <= 0)
		{
			print("len = 0");
			Disconnect();
			return;
		}

		byte[] data = new byte[len];
		Array.Copy(receiveBuffer, data, len);

		string str = Encoding.UTF8.GetString(data);
		OpeningHandshake(str);
		handshakeDone = true;

		SendWelcome();

		stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
	}

	void SendWelcome() {
		Packet packet = Packet.Create(ServerToClient.Welcome);
		packet.AddUShort(id);
		packet.Finish();
		Send(packet.readBuffer);
	}

	void OpeningHandshake(string msg) {
		string key = Regex.Match(msg, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
		const string guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
		string strToHash = key + guid;

		byte[] responseKey = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(strToHash));

		byte[] response = Encoding.UTF8.GetBytes(
			"HTTP/1.1 101 Switching Protocols\r\n" +
			"Connection: Upgrade\r\n" +
			"Upgrade: websocket\r\n" +
			"Sec-WebSocket-Accept: " + Convert.ToBase64String(responseKey) + "\r\n\r\n"
		);
		
		stream.BeginWrite(response, 0, response.Length, null, null);
	}

	void ClosingHandshake() {
		byte[] frame = new byte[2];
		frame[0] = 0b10001000;
		frame[1] = 0b00000000;

		stream.Write(frame, 0, frame.Length);

		socket.Close();
		handshakeDone = false;
	}

	// see if we get a closing handshake msg, if so send back a closing handshake
	bool HandleClosingHandshake(byte[] bytes) {
		if (bytes[0] != 0b10001000)
			return false;
		
		// opcode is 8, so it's a close connection
		Disconnect();

		return true;
	}

	public void Disconnect() {
		SendDisconnect();
		ClosingHandshake();

		ThreadManager.ExecuteOnMainThread(() => {
											  lock (Server.instance.availableClientIds)
											  {
												  Server.instance.clients.Remove(id);
												  Server.instance.availableClientIds.Push(id);
											  }

											  Destroy(gameObject);
										  });
		print($"client {id} disconnected");
	}

	void NotFixedUpdate() {
		if (!stream.DataAvailable)
			return;

		if (!handshakeDone)
			return;

		if (socket.Available <= 0)
			return;

		byte[] data = new byte[socket.Available];
		stream.Read(data, 0, socket.Available);
		byte[] decoded = DecodeMessage(data);
		
		ThreadManager.ExecuteOnMainThread(() => {
											  Packet packet = new Packet(decoded);
											  ClientToServer packetId = (ClientToServer) packet.GetUShort();
											  PacketHandling.handlers[packetId](id, packet);
										  });
	}

	void ReceiveCallback(IAsyncResult result) {
		try
		{
			int len = stream.EndRead(result);

			if (len <= 0)
			{
				print("len = 0");
				Disconnect();
				return;
			}

			byte[] data = new byte[len];
			Array.Copy(receiveBuffer, data, len);
			
			// check for closing connection before anything bad can happen
			if (HandleClosingHandshake(data))
				return;

			byte[] decoded = DecodeMessage(data);
			if (HandleData(decoded))
				receivedData.Reset();
			
			stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
		}
		catch (Exception e)
		{
			print($"error receiving tcp data: {e}");
			Disconnect();
		}
	}

	// this is "not copied" from tom weiland tcp/udp networking series
	// actually like all the networking (except websocket stuff) is from tom weiland tcp/udp multiplayer game series
	// anyways this function builds on a packet because tcp does not like being complete
	// it returns true if we should reset receivedData
	// anyways you've made it this far maybe try subscribing to 'Treidex' on YouTube (you can watch my videos too)
	bool HandleData(byte[] data) {
		int packetLen;

		receivedData.AddBytes(data);
		receivedData.ReadReady();
		
		
		packetLen = 0;
		if (receivedData.unreadLen >= 4)
		{
			// we got a new packet
			packetLen = receivedData.GetInt();

			if (packetLen <= 0)
				return true;
		}

		while (packetLen > 0 && packetLen <= receivedData.unreadLen)
		{

			byte[] packetBytes = receivedData.GetBytes(packetLen);

			// if (packetBytes.Length < 4 || packetBytes.Length < packet.GetInt())
			// 	throw new Exception("P. is not big enough. uhh, I mean: size doesn't matter");

			ThreadManager.ExecuteOnMainThread(() => {
												  Packet packet = new Packet(packetBytes);
												  ClientToServer packetId = (ClientToServer) packet.GetUShort();
												  
												  try
												  {
													  PacketHandling.handlers[packetId](id, packet);
												  }
												  catch (KeyNotFoundException)
												  {
													  throw new Exception($"packet #{packetId} not handled");
												  }
											  });
			
			packetLen = 0;
			if (receivedData.unreadLen >= 4)
			{
				// we got a new packet
				packetLen = receivedData.GetInt();

				if (packetLen <= 0)
					return true;
			}
		} ;

		// this is probably the most comment function in this whole project
		// maybe even the most commented function in my whole career (if u can call it that)
		// perhaps even the ONLY commented piece of code in my "life"
		return packetLen <= 1;
	}

	byte[] DecodeMessage(byte[] bytes) {
		bool fin = (bytes[0] & 0b10000000) != 0;
		bool mask = (bytes[1] & 0b10000000) != 0;

		if (!fin)
			throw new Exception("fatal bug; just hang urself at this point");

		int opcode = bytes[0] & 0b00001111;
		int msglen = bytes[1] & 0b01111111;
		int offset = 2;

		if (msglen == 126 || msglen == 127)
			throw new Exception("teddy has incomplete code");

		if (mask)
		{
			byte[] decoded = new byte[msglen];
			byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
			offset += 4;

			for (int i = 0; i < msglen; i++)
				decoded[i] = (byte) (bytes[offset + i] ^ masks[i % 4]);

			return decoded;
		}
		
		throw new Exception("mask bit not set");
	}

	public void Send(byte[] data) {
		if (!socket.Connected)
			Disconnect(); // todo this is a hacky solution
		
		byte[] frame = MakeFrame(data);
		stream.WriteAsync(frame, 0, frame.Length);
	}

	// what's this "hard coding" ur speaking of??
	byte[] MakeFrame(byte[] data) {
		if (data.Length > 125)
			throw new Exception("teddy code bad hehehehaw"); // if I get this exception, I need to impl fragmentation
		
		byte[] bytes = new byte[data.Length + 2];
		bytes[0] = 0b10000010; // fin bit + opcode 2
		bytes[1] = (byte) data.Length;
		
		Array.Copy(data, 0, bytes, 2, data.Length);

		return bytes;
	}

	void SendDisconnect() {
		Packet packet = Packet.Create(ServerToClient.Disconnect);
		packet.AddUShort(id);
		Server.instance.SendAll(packet);
	}
}
