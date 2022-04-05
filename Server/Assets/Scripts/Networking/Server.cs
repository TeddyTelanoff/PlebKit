using System.Collections.Generic;
using System.Text;

using PacketExt;

using UnityEngine;

using WebSocketSharp;
using WebSocketSharp.Server;

public class Server: MonoBehaviour
{
	public static Server instance;
	
	public int port;

	public WebSocketServer server;

	void Awake() {
		instance = this;
	}

	void Start() {
		PacketHandling.MakeDictionary();
		
		server = new WebSocketServer(port);
		server.AddWebSocketService<ServerBehavior>("/");
		server.Start();
		print($"server active on port {port}");
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space))
			print(ServerBehavior.clients.Count);
	}

	void OnApplicationQuit() {
		server.Stop();
	}

	public void Send(Packet packet, ClientID client) {
		if (ServerBehavior.clients.TryGetValue(client, out ServerBehavior behavior))
			behavior.Send(packet);
	}

	public void SendAll(Packet packet, ClientID except = ClientID.Nobody) {
		foreach (ClientID client in ServerBehavior.clients.Keys)
		{
			if (client != except)
				Send(packet, client);
		}
	}

	[PacketHandler(ClientToServer.Join)]
	static void PlayerJoined(ClientID client, Packet packet) {
		string username = packet.GetString();
		if (username.IsNullOrEmpty())
			username = "Guest " + client;
			
		print($"{username} joined!");

		SendSpawn(username, Vector3.zero);
	}

	static void SendSpawn(string username, Vector3 where) {
		Packet packet = Packet.Create(ServerToClient.Spawn);
		packet.AddString(username);
		packet.AddVector3(where);
		instance.SendAll(packet);
	}

	public class ServerBehavior: WebSocketBehavior
	{
		// this lib is garbage, but it is only one out there so 😀🔫
		public static Dictionary<ClientID, ServerBehavior> clients = new Dictionary<ClientID, ServerBehavior>();

		public ClientID clientId;
		
		protected override void OnOpen() {
			clientId = (ClientID) ID.GetHashCode();
			lock (clients)
			{
				clients.Add(clientId, this);
			}

			print("client found");
		}

		protected override void OnMessage(MessageEventArgs e) {
			print("we got a packet");
			Packet packet = new Packet(e.RawData);
			ClientToServer id = (ClientToServer) packet.GetUShort();
			print($"got message #{id}");
			PacketHandling.handlers[id].Invoke(clientId, packet);
		}

		protected override void OnClose(CloseEventArgs e) {
			clients.Remove(clientId);
		}

		public void Send(Packet packet) {
			packet.MakeReadable();
			Send(packet.readBuffer);
		}

		public void Broadcast(Packet packet) {
			packet.MakeReadable();
			Sessions.Broadcast(packet.readBuffer);
		}
	}
}
