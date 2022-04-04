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
	UpdateResources,
}

public enum ClientToServerId: ushort
{
	Name = 1,
	PlayerPosition,
	Quiz,
	QuizGuess,
	Fish,
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
			}
		}
	}
	
	public Server server { get; private set; }

	[SerializeField]
	ushort port;
	[SerializeField]
	ushort maxClientCount;

	void Awake() {
		instance = this;
	}

	void Start() {
		Application.targetFrameRate = 60;
		
		RiptideLogger.Initialize(Debug.Log,Debug.Log, Debug.LogWarning, Debug.LogError, false);

		server = new Server();
		server.Start(port, maxClientCount);
		server.ClientDisconnected += PlayerDisconnected;
	}

	void FixedUpdate() {
		server.Tick();
	}

	void OnApplicationQuit() {
		server.Stop();
	}

	void PlayerDisconnected(object sender, ClientDisconnectedEventArgs e) {
		if (Player.players.TryGetValue(e.Id, out Player player))
			Destroy(player.gameObject);
	}
}
