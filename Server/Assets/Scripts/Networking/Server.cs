using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using PacketExt;

using UnityEngine;

public class Server: MonoBehaviour
{
	public static Server instance;

	public GameObject clientPrefab;
	
	public ushort maxClients;
	public int port;

	public HttpListener httpListener;

	public Dictionary<ushort, Client> clients = new Dictionary<ushort, Client>();
	public Stack<ushort> availableClientIds = new Stack<ushort>();

	void Awake() {
		instance = this;
	}

	void Start() {
		PacketHandling.MakeDictionary();

		httpListener = new HttpListener();
		httpListener.Prefixes.Add($"http://*:{port}/");
		httpListener.Start();

		ConnectAsync();

		// start from client id 1 -> maxClients
		// (in a stack, you pop the last item you pushed)
		for (ushort i = maxClients; i > 0; i--)
			availableClientIds.Push(i);

		print($"server active on port {port}");
	}

	async Task ConnectAsync() {
		while (Application.isPlaying)
		{
			HttpListenerContext ctx = await httpListener.GetContextAsync();
			print($"connection?");
			
			print(ctx.Request.ContentLength64);

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
													  client.ConnectAsync(ctx).Wait();
												  });
			}
		}
	}

	void OnApplicationQuit() {
		foreach (Client client in clients.Values)
			client.Disconnect("server close");
		
		httpListener.Close();
	}

	public void Send(Packet packet, ushort clientId) {
		packet.Finish();
		clients[clientId].Send(packet.readBuffer);
	}

	public void SendAll(Packet packet, ushort except = 0) {
		packet.Finish();
		foreach (Client client in clients.Values)
			if (client.id != except)
				client.Send(packet.readBuffer);
	}

	[PacketHandler(ClientToServer.Join)]
	static void PlayerJoined(ushort clientId, Packet packet) {
		string username = packet.GetString();
		if (string.IsNullOrEmpty(username))
			username = "Guest " + clientId;
			
		print($"{username} joined!");
		Player.Spawn(clientId, username);

		Packet spawnPacket = MakeSpawnPacket(clientId, username, Vector3.zero, instance.clients[clientId].player.movement.speed);
		instance.SendAll(spawnPacket, clientId);

		foreach (Client otherClient in instance.clients.Values)
		{
			Packet otherSpawnPacket = MakeSpawnPacket(
				otherClient.id, otherClient.player.username,
				otherClient.player.transform.position,
				otherClient.player.movement.speed
			);
			instance.Send(otherSpawnPacket, clientId);
		}
	}

	static Packet MakeSpawnPacket(ushort id, string username, Vector3 where, float speed) {
		Packet packet = Packet.Create(ServerToClient.Spawn);
		packet.AddUShort(id);
		packet.AddString(username);
		packet.AddVector3(where);
		packet.AddFloat(speed);
		packet.AddUShort((ushort) GameLogic.instance.spawnWorld);

		return packet;
	}
}
