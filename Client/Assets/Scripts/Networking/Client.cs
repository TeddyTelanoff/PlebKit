using System.Text;

using PacketExt;

using NativeWebSocket;

using UnityEngine;

public class Client : MonoBehaviour
{
	public static Client instance;
	
	public WebSocket socket;
	public int port;

	void Awake() {
		instance = this;
	}

	void Start() {
		socket = new WebSocket($"ws://localhost:{port}/");
		socket.OnOpen += OnOpen;
		socket.OnClose += OnClose;
		socket.OnError += OnError;
		socket.OnMessage += OnMessage;
	}

	public void Connect() {
		socket.Connect();
		print("connecting...");
	}

	public void Send(Packet packet) {
		packet.MakeReadable();
		socket.Send(packet.readBuffer);
	}

#if !UNITY_WEBGL || UNITY_EDITOR
	void FixedUpdate() {
		socket.DispatchMessageQueue();
	}
#endif

	void OnOpen() {
		print("socket open");
		JoinScreen.instance.SendJoin();
	}

	void OnMessage(byte[] bytes) {
		Packet packet = new Packet(bytes);
		ServerToClient id = (ServerToClient) packet.GetUShort();
		
		switch (id)
		{
		case ServerToClient.Spawn:
			string username = packet.GetString();
			Vector3 spawnpoint = packet.GetVector3();
			print($"{username} spawned!, spawnpoint: {spawnpoint}");
			break;
		}
	}

	void OnError(string err) {
		print($"error with socket: {err}");
	}

	void OnClose(WebSocketCloseCode code) {
		print($"socket closed because: {code}");
	}
}
