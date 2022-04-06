using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

public class Client: MonoBehaviour
{
	public const int dataBufferSize = 4096;
	
	public int id;

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
		handshakeDone = false;
		print("client found");
	}

	void ReceiveHandshakeCallback(IAsyncResult result) {
		int len = stream.EndRead(result);

		if (len <= 0)
		{
			Disconnect(false);
			return;
		}

		byte[] data = new byte[len];
		Array.Copy(receiveBuffer, data, len);

		string str = Encoding.UTF8.GetString(data);
		OpeningHandshake(str);
		handshakeDone = true;
		
		stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
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

	void Disconnect(bool sendHandshake = true) {
		if (sendHandshake)
			; // todo send closing handshake
		
		socket.Close();
		stream = null;
		receiveBuffer = null;
		socket = null;
		
		gameObject.SetActive(false);
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

	byte[] DecodeMessage(byte[] data) {
		bool fin = (data[0] & 0b10000000) != 0;
		bool mask = (data[0] & 0b10000000) != 0;

		int opcode = data[0] & 0b00001111;
		int msglen = data[1] & 0b01111111;
		int offset = 2;

		if (msglen == 126 || msglen == 127)
			throw new Exception("teddy has incomplete code");

		if (mask)
		{
			byte[] decoded = new byte[msglen];
			byte[] masks = new byte[4] { data[offset], data[offset + 1], data[offset + 2], data[offset + 3] };
			offset += 4;
			
			for (int i = 0; i < msglen; i++)
				decoded[i] = (byte)(data[offset + i] ^ masks[i % 4]);

			return decoded;
		}
		
		throw new Exception("mask bit not set");
	}
}
