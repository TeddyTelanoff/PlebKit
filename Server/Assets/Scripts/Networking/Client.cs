using System;
using System.Net.Sockets;
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
	public byte[] recieveBuffer;

	public void Connect(TcpClient client) {
		socket = client;
		socket.ReceiveBufferSize = dataBufferSize;
		socket.SendBufferSize = dataBufferSize;

		stream = socket.GetStream();

		recieveBuffer = new byte[dataBufferSize];

		stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceiveHandshakeCallback, null);
		print("client found");
	}

	void ReceiveHandshakeCallback(IAsyncResult result) {
		int len = stream.EndRead(result);

		if (len <= 0)
			return; // todo disconnect

		byte[] data = new byte[len];
		Array.Copy(recieveBuffer, data, len);

		string str = Encoding.UTF8.GetString(data);
		OpeningHandshake(str);
		
		// proceed to normal callback
		stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceiveCallback, null);
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

	void ReceiveCallback(IAsyncResult result) {
		// todo implement
	}
}
