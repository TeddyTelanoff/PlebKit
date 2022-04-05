using System.Text;

using NativeWebSocket;

using UnityEngine;

public class Client : MonoBehaviour
{
	public static Client instance;
	
	public WebSocket socket;

	void Awake() {
		instance = this;
	}

	void Start() {
		socket = new WebSocket("ws://localhost:62490/");
		socket.OnOpen += OnOpen;
		socket.OnClose += OnClose;
		socket.OnError += OnError;
		socket.OnMessage += OnMessage;

		socket.Connect();
		print("connecting...");
	}

#if !UNITY_WEBGL || UNITY_EDITOR
	void FixedUpdate() {
		socket.DispatchMessageQueue();
	}
#endif

	void OnOpen() {
		print("socket open");
		socket.SendText("helllo meister");
	}

	void OnMessage(byte[] bytes) {
		Packet packet = new Packet(bytes);
		ServerToClient id = (ServerToClient) packet.GetUShort();
		switch (id)
		{
		case ServerToClient.Spawn:
			string username = packet.GetString();
			Vector3 spawnpoint = new Vector3(packet.GetFloat(), packet.GetFloat(), packet.GetFloat());
			print($"name: {username}, spawnpoint: {spawnpoint}");
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
