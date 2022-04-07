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
	public Stack<ushort> availableClientIds = new Stack<ushort>();

	void Awake() {
		instance = this;
	}

	void Start() {
		PacketHandling.MakeDictionary();

		tcpListener = new TcpListener(IPAddress.Any, port);
		tcpListener.Start();
		tcpListener.BeginAcceptTcpClient(ConnectCallback, null);

		for (ushort i = 1; i <= maxClients; i++)
			availableClientIds.Push(i);

		print($"server active on port {port}");
	}

	void ConnectCallback(IAsyncResult result) {
		TcpClient tcpClient = tcpListener.EndAcceptTcpClient(result);
		tcpListener.BeginAcceptTcpClient(ConnectCallback, null);

		print($"connection?");

		lock (availableClientIds)
		{
			if (availableClientIds.Count <= 0)
				// todo respond with: server full
				return;
			
			ushort id = availableClientIds.Pop();
			ThreadManager.ExecuteOnMainThread(() => {
												  Client client = Instantiate(clientPrefab).GetComponent<Client>();
												  client.id = id;
												  clients.Add(id, client);
												  client.Connect(tcpClient);
											  });
		}
	}

	void OnApplicationQuit() {
		foreach (Client client in clients.Values)
			client.Disconnect();
		
		tcpListener.Stop();
	}

	public void Send(Packet packet, ushort client) {
		packet.MakeReadable();
		clients[client].Send(packet.readBuffer);
	}

	public void SendAll(Packet packet, ushort except = 0) {
		packet.MakeReadable();
		foreach (Client client in clients.Values)
			if (client.id != except)
				client.Send(packet.readBuffer);
	}

	[PacketHandler(ClientToServer.Join)]
	static void PlayerJoined(ushort client, Packet packet) {
		string username = packet.GetString();
		if (string.IsNullOrEmpty(username))
			username = "Guest " + client;
			
		print($"{username} joined!");
		instance.clients[client].player.gameObject.SetActive(true);

		SendSpawn(client, username, Vector3.zero);
	}

	static void SendSpawn(ushort id, string username, Vector3 where) {
		Packet packet = Packet.Create(ServerToClient.Spawn);
		packet.AddUShort(id);
		packet.AddString(username);
		packet.AddVector3(where);
		instance.SendAll(packet);
	}
}
