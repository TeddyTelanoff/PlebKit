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
	public byte[] receiveBuffer;
	public bool handshakeDone;

	public void Connect(TcpClient client) {
		socket = client;
		socket.ReceiveBufferSize = dataBufferSize;
		socket.SendBufferSize = dataBufferSize;

		stream = socket.GetStream();

		receiveBuffer = new byte[dataBufferSize];

		stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveHandshakeCallback, null);
		print("client found");
	}

	void ReceiveHandshakeCallback(IAsyncResult result) {
		int len = stream.EndRead(result);

		if (len <= 0)
		{
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
		packet.MakeReadable();
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
		return;
		stream.BeginRead(Array.Empty<byte>(), 0, 0, result => {
														socket.Close();
														handshakeDone = false;
													}, null);
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
				Disconnect();
				return;
			}

			byte[] data = new byte[len];
			Array.Copy(receiveBuffer, data, len);
			
			// check for closing connection before anything bad can happen
			if (HandleClosingHandshake(data))
				return;

			byte[] decoded = DecodeMessage(data);
			ThreadManager.ExecuteOnMainThread(() => {
												  Packet packet = new Packet(decoded);
												  ClientToServer packetId = (ClientToServer) packet.GetUShort();
												  PacketHandling.handlers[packetId](id, packet);
											  });
			
			stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
		}
		catch (Exception e)
		{
			print($"error receiving tcp data: {e}");
			Disconnect();
		}
	}

	byte[] DecodeMessage(byte[] bytes) {
		bool fin = (bytes[0] & 0b10000000) != 0;
		bool mask = (bytes[1] & 0b10000000) != 0;

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
		packet.MakeReadable();
		Send(packet.readBuffer);
	}
}
