using System;
using System.Text;

using PacketExt;

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

	void Start() {
	#if UNITY_EDITOR
		PacketHandling.VerifyDictionary();
	#endif

		socket = new WebSocket($"ws://localhost:{port}/");
		socket.OnOpen += OnOpen;
		socket.OnClose += OnClose;
		socket.OnError += OnError;
		socket.OnMessage += OnMessage;
	}

	void OnApplicationQuit() {
		socket.Close();
	}

	public void Connect() {
		socket.Connect();
		print("connecting...");
	}

	public void Send(Packet packet) {
		packet.Finish();
		socket.Send(packet.readBuffer);
	}

#if !UNITY_WEBGL || UNITY_EDITOR
	void FixedUpdate() {
		socket.DispatchMessageQueue();
	}
#endif

	void OnOpen() {
		print("socket open");
	}

	void OnMessage(byte[] bytes) {
		Packet packet = new Packet(bytes);
		if (bytes.Length < 4 || bytes.Length < packet.GetInt())
			throw new Exception("packet is not big enough. uhh, I mean: size doesn't matter");
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
	}

	[PacketHandler(ServerToClient.Welcome)]
	public static void OnWelcome(Packet packet) {
		instance.clientId = packet.GetUShort();
		JoinScreen.instance.SendJoin();
	}
}
