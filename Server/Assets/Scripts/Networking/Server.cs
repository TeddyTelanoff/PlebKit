using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

using PacketExt;

using UnityEngine;

public class Server: MonoBehaviour
{
	public static Server instance;

	public GameObject clientPrefab;
	
	public int maxClients;
	public int port;

	public TcpListener tcpListener;

	public Dictionary<ushort, Client> clients = new Dictionary<ushort, Client>();

	void Awake() {
		instance = this;
	}

	void Start() {
		PacketHandling.MakeDictionary();

		tcpListener = new TcpListener(IPAddress.Any, port);
		tcpListener.Start();
		tcpListener.BeginAcceptTcpClient(ConnectCallback, null);

		for (ushort i = 1; i <= maxClients; i++)
		{
			Client client = Instantiate(clientPrefab).GetComponent<Client>();
			client.id = i;
			clients.Add(i, client);
		}

		print($"server active on port {port}");
	}

	void ConnectCallback(IAsyncResult result) {
		TcpClient client = tcpListener.EndAcceptTcpClient(result);
		tcpListener.BeginAcceptTcpClient(ConnectCallback, null);

		for (ushort i = 1; i <= maxClients; i++)
			if (clients[i].socket == null)
			{
				ThreadManager.ExecuteOnMainThread(() => {
													  clients[i].gameObject.SetActive(true);
													  clients[i].Connect(client);
												  });
				return;
			}

		// todo respond with: server full
	}

	void OnApplicationQuit() {
		tcpListener.Stop();
	}

	public void Send(Packet packet, ushort client) {
		packet.MakeReadable();
		clients[client].Send(packet.readBuffer);
	}

	public void SendAll(Packet packet, ushort except = 0) {
		packet.MakeReadable();
		for (ushort i = 1; i <= maxClients; i++)
			if (i != except && clients[i].socket != null)
				clients[i].Send(packet.readBuffer);
	}

	[PacketHandler(ClientToServer.Join)]
	static void PlayerJoined(ushort client, Packet packet) {
		string username = packet.GetString();
		if (string.IsNullOrEmpty(username))
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
}
