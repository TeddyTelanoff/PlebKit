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
		PacketHandling.MakeDictionary();

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
	}

	void OnMessage(byte[] bytes) {
		Packet packet = new Packet(bytes);
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
	static void OnWelcome(Packet packet) {
		instance.clientId = packet.GetUShort();
		JoinScreen.instance.SendJoin();
	}
}
