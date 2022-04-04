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
		server.AddWebSocketService<Default>("/");
		server.Start();
		print($"server active on port {port}");
	}

	void OnApplicationQuit() {
		server.Stop();
	}
	
	public class Default: WebSocketBehavior
	{
		protected override void OnOpen() {
			Send(Encoding.UTF8.GetBytes("hello, other"));
		}

		protected override void OnMessage(MessageEventArgs e) =>
			print($"someone said {e.Data}!");
	}
}
