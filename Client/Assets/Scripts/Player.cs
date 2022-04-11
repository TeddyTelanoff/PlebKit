using System.Collections.Generic;

using PacketExt;

using UnityEngine;

// Player stuff for all players (local & other)
public class Player: MonoBehaviour
{
	public static readonly Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();

	public static Player localPlayer;
	public ushort id;
	public string username;
	public bool isLocal;

	public World world;

	public PlayerMovement movement;
	public PlayerQuiz quiz;
	public PlayerFish fish;

	public float money;

	void OnValidate() {
		if (movement == null)
			TryGetComponent(out movement);
		if (quiz == null)
			TryGetComponent(out quiz);
		if (fish == null)
			TryGetComponent(out fish);
	}

	void OnDestroy() {
		if (isLocal)
		{
			GameLogic.instance.SwitchWorlds(World.Lobby);
			GameLogic.instance.supplyPanel.SetActive(false);
		}
		players.Remove(id);
	}

	public static void UpdateSupplyDisplay() {
		GameLogic.instance.supplyText.text = $"Bait: {localPlayer.fish.bait}; Money: {localPlayer.money}";
	}

	[PacketHandler(ServerToClient.PlayerMovement)]
	public static void PlayerMovement(Packet packet) {
		ushort client = packet.GetUShort();
		if (players.TryGetValue(client, out Player player))
			player.transform.position = packet.GetVector3();
	}

	[PacketHandler(ServerToClient.Spawn)]
	public static void OnSpawn(Packet packet) {
		ushort id = packet.GetUShort();
		string username = packet.GetString();
		Vector3 spawnpoint = packet.GetVector3();
		float speed = packet.GetFloat();
		World world = (World) packet.GetUShort();
		print($"{username} spawned!, spawnpoint: {spawnpoint}");

		SpawnPlayer(id, username, spawnpoint, speed, world);
	}

	[PacketHandler(ServerToClient.Disconnect)]
	public static void OnDisconnect(Packet packet) {
		ushort id = packet.GetUShort();
		print($"player {id} disconnected");
		Destroy(players[id].gameObject);
	}

	[PacketHandler(ServerToClient.SwitchWorlds)]
	public static void SwitchWorlds(Packet packet) {
		if (players.TryGetValue(packet.GetUShort(), out Player player))
		{
			World newWorld = (World) packet.GetUShort();
			
			if (player.isLocal)
				GameLogic.instance.SwitchWorlds(newWorld);
			else if (newWorld != localPlayer.world)
				player.gameObject.SetActive(false); // hide players from other worlds
			
			player.world = newWorld;
			player.transform.position = Vector3.zero;
		}
	}

	[PacketHandler(ServerToClient.InventoryUpdate)]
	public static void InventoryUpdate(Packet packet) {
		float money = packet.GetFloat();
		int bait = packet.GetInt();
		int[] fishes = packet.GetInts();

		localPlayer.money = money;
		localPlayer.fish.bait = bait;
		localPlayer.fish.fishes = fishes;

		UpdateSupplyDisplay();
	}

	static void SpawnPlayer(ushort id, string username, Vector3 spawnpoint, float speed, World world) {
		Player player;
		
		if (id == Client.instance.clientId) // hey look it's me!
		{
			player = Instantiate(GameLogic.instance.localPlayerPrefab).GetComponent<Player>();
			player.isLocal = true;
			player.movement.speed = speed;

			localPlayer = player;
			GameLogic.instance.SwitchWorlds(world);
			GameLogic.instance.supplyPanel.SetActive(true);
			UpdateSupplyDisplay();
			foreach (Player otherPlayer in players.Values)
				if (otherPlayer.world != world)
					otherPlayer.gameObject.SetActive(false);
		}
		else
		{
			player = Instantiate(GameLogic.instance.otherPlayerPrefab).GetComponent<Player>();
		}
		
		player.id = id;
		player.username = username;
		player.transform.position = spawnpoint;
		player.world = world;
		
		if (localPlayer != null && world != localPlayer.world)
			player.gameObject.SetActive(false);
		players.Add(id, player);
	}
}
