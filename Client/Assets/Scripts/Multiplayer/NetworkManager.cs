using System;

using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

public enum ServerToClientId: ushort
{
	SpawnPlayer = 1,
	PlayerPosition,
	SwitchWorlds,
	Question,
	QuestionFeedback,
}

public enum ClientToServerId: ushort
{
	Name = 1,
	PlayerPosition,
	Quiz,
	QuizGuess,
}

public class NetworkManager: MonoBehaviour
{
	private static NetworkManager _instance;
	public static NetworkManager instance {
		get => _instance;
		private set {
			if (_instance is null)
				_instance = value;
			else if (_instance != value)
			{
				Debug.LogWarning($"instance of {nameof(NetworkManager)} exists, destroying duplicate");
				Destroy(value);
			}
		}
	}
	
	public Client client { get; private set; }

	[SerializeField]
	string ip;
	[SerializeField]
	ushort port;

	void Awake() {
		instance = this;
	}

	void Start() {
		RiptideLogger.Initialize(Debug.Log,Debug.Log, Debug.LogWarning, Debug.LogError, false);

		client = new Client();
		client.Connected += OnConnect;
		client.ConnectionFailed += OnFailToConnect;
		client.ClientDisconnected += OnPlayerLeft;
		client.Disconnected += OnDisconnect;
	}

	void FixedUpdate() {
		client.Tick();
	}

	void OnApplicationQuit() {
		client.Disconnect();
	}
	
	public void Connect() => 
		client.Connect($"{ip}:{port}");

	void OnConnect(object sender, EventArgs e) {
		UIManager.instance.SendName();
	}

	void OnFailToConnect(object sender, EventArgs e) {
		UIManager.instance.BackToJoin();
	}

	void OnPlayerLeft(object sender, ClientDisconnectedEventArgs e) {
		if (Player.players.TryGetValue(e.Id, out Player player))
			Destroy(player.gameObject);
	}

	void OnDisconnect(object sender, EventArgs e) {
		UIManager.instance.BackToJoin();
		foreach (Player player in Player.players.Values)
			Destroy(player.gameObject);
	}
}