using System.Collections.Generic;
using System.Text;

using UnityEngine;

using WebSocketSharp;
using WebSocketSharp.Server;

public class Server: MonoBehaviour
{
	public int port;

	public WebSocketServer server;
	
	void Start() {
		server = new WebSocketServer(port);
		server.AddWebSocketService<ServerBehavior>("/");
		server.Start();
		print($"server active on port {port}");
	}

	void OnApplicationQuit() {
		server.Stop();
	}

	public class ServerBehavior: WebSocketBehavior
	{
		// this lib is garbage, but it is only one out there so 😀🔫
		public static Dictionary<string, ServerBehavior> clients = new Dictionary<string, ServerBehavior>();
		
		protected override void OnOpen() {
			clients.Add(ID, this);
			
			Packet packet = Packet.Create(ServerToClient.Spawn);
			packet.AddString("jeff");
			packet.AddFloat(0).AddFloat(2).AddFloat(4);
			Send(packet);
		}

		protected override void OnMessage(MessageEventArgs e) =>
			print($"someone said {e.Data}!");

		public void Send(Packet packet) {
			packet.MakeReadable();
			Send(packet.readBuffer);
		}
	}
}
