using NativeWebSocket;

using UnityEngine;

public class Client : MonoBehaviour
{
	WebSocket socket;
	
	void Start() {
		socket = new WebSocket("ws://localhost:62490/");
		socket.OnOpen += OnOpen;
		socket.OnClose += OnClose;
		socket.OnError += OnError;
		socket.OnMessage += OnMessage;
		
		socket.Connect();
		print("connecting...");
	}

	void FixedUpdate() {
		socket.DispatchMessageQueue();
	}

	void OnOpen() {
		print("socket open");
	}

	void OnMessage(byte[] data) {
		foreach (byte bite in data)
			print(bite + ' ');
	}

	void OnError(string err) {
		print($"error with socket: {err}");
	}

	void OnClose(WebSocketCloseCode code) {
		print($"socket closed because: {code}");
	}
}
