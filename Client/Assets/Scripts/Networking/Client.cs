using System;

using NativeWebSocket;

using UnityEngine;

public class Client : MonoBehaviour
{
	public static Client instance;

	public uint clientId;
	
	public WebSocket socket;
	public int port;

	void Awake() {
		instance = this;
	}

#if UNITY_EDITOR
	void Start() {
		PacketHandling.VerifyDictionary();
	}
#endif

	void OnApplicationQuit() {
	    if (socket != null)
		    socket.Close();
	}

	public void Connect(string ip) {
		socket = new WebSocket($"ws://{ip}:{port}/");
		socket.OnOpen += OnOpen;
    	socket.OnClose += OnClose;
    	socket.OnError += OnError;
    	socket.OnMessage += OnMessage;
		socket.Connect();
		print("connecting...");
	}

	public void Send(Packet packet) {
		if (socket.State != WebSocketState.Open)
			return;
		
		packet.Finish();
		socket.Send(packet.readBuffer);
	}

#if !UNITY_WEBGL || UNITY_EDITOR
	void FixedUpdate() {
		if (socket != null)
			socket.DispatchMessageQueue();
	}
#endif

	void OnOpen() {
		print("socket open");
	}

	void OnMessage(byte[] bytes) {
		Packet packet = new Packet(bytes);
		if (bytes.Length < 4 || bytes.Length < packet.GetInt())
			throw new Exception("P. is not big enough. uhh, I mean: size doesn't matter");
		ServerToClient packetId = (ServerToClient) packet.GetUShort();

		if (PacketHandling.handlers.TryGetValue(packetId, out PacketHandling.PacketHandler handler))
			handler(packet);
		else
			print($"no handler for {packetId}");
	}

	void OnError(string err) {
		print($"error with socket: {err}");
	}

	void OnClose(WebSocketCloseCode code) {
		print($"socket closed because: {code}");
		JoinScreen.instance.BackToJoin();
		
		foreach (Player player in Player.players.Values)
			Destroy(player.gameObject);
	}

	[PacketHandler(ServerToClient.Welcome)]
	public static void OnWelcome(Packet packet) {
		instance.clientId = packet.GetUShort();
		JoinScreen.instance.SendJoin();
	}
}
