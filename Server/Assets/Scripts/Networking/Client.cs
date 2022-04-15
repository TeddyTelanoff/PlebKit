using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

public class Client: MonoBehaviour
{
	public const int dataBufferSize = 4096;
	
	public ushort id;

	public Player player;
	
	public WebSocket socket;
	public ArraySegment<byte> receiveBuffer;
	public bool disconnecting;

	public async Task ConnectAsync(HttpListenerContext httpContext) {
		print(httpContext.Request.HttpMethod);
		if (!httpContext.Request.IsWebSocketRequest)
		{
			httpContext.Response.StatusCode = 400;
			httpContext.Response.Close();
			print("nope, not a websocket request");
			return;
		}

		HttpListenerWebSocketContext ctx = await httpContext.AcceptWebSocketAsync(null);
		print("gg");
		socket = ctx.WebSocket;
		print(socket.State);

		receiveBuffer = new ArraySegment<byte>(new byte[dataBufferSize]);
		
		ReceiveMessageAsync();

		print($"client {id} found");
	}

	async Task ReceiveMessageAsync() {
		while (socket.State == WebSocketState.Open)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				WebSocketReceiveResult result;

				do
				{
					result = await socket.ReceiveAsync(receiveBuffer, CancellationToken.None);
					ms.Write(receiveBuffer.Array!, receiveBuffer.Offset, result.Count);
				} while (!result.EndOfMessage);

				ms.Seek(0, SeekOrigin.Begin);
				byte[] bytes = ms.GetBuffer();

				if (result.MessageType == WebSocketMessageType.Binary)
				{
					ThreadManager.ExecuteOnMainThread(() => {
														  Packet packet = new Packet(bytes);
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
				}
				else
				{
					print(Encoding.UTF8.GetString(ms.GetBuffer()));

					throw new Exception("txt msg received");
				}
			}
		}
	}

	public void Disconnect(string reason) {
		if (disconnecting)
			return;
		
		disconnecting = true;
		
		SendDisconnect();
		socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None)
			  .ContinueWith(task => {
								print($"client {id} disconnected.");
								Destroy(gameObject);
							});
		print($"client {id} disconnecting...");
	}

	public void Send(byte[] data) {
		if (disconnecting)
			return;
		if (socket != null && socket.State != WebSocketState.Open)
			Disconnect("self disconnect"); // todo this is a hacky solution
		else
			socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
	}

	void SendDisconnect() {
		Packet packet = Packet.Create(ServerToClient.Disconnect);
		packet.AddUShort(id);
		Server.instance.SendAll(packet);
	}
}
