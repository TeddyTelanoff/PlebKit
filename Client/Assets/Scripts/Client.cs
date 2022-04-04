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

	void OnMessage(byte[] data) {
		string str = Encoding.UTF8.GetString(data);
		print($"server said {str}");
		return;
		
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
