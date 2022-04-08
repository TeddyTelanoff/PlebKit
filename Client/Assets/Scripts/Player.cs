using System.Collections.Generic;

using PacketExt;

using UnityEngine;

// Player stuff for all players (local & other)
public class Player: MonoBehaviour
{
	public static readonly Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();

	public static Player localPlayer;
	public int id;
	public string username;
	public bool isLocal;

	public PlayerMovement movement;

	void OnValidate() {
		if (movement == null)
			TryGetComponent(out movement);
	}

	[PacketHandler(ServerToClient.PlayerMovement)]
	public static void PlayerMovement(Packet packet) {
		ushort client = packet.GetUShort();
		players[client].transform.position = packet.GetVector3();
	}

	[PacketHandler(ServerToClient.Spawn)]
	public static void OnSpawn(Packet packet) {
		ushort id = packet.GetUShort();
		string username = packet.GetString();
		Vector3 spawnpoint = packet.GetVector3();
		float speed = packet.GetFloat();
		print($"{username} spawned!, spawnpoint: {spawnpoint}");

		SpawnPlayer(id, username, spawnpoint, speed);
	}

	[PacketHandler(ServerToClient.Disconnect)]
	public static void OnDisconnect(Packet packet) {
		ushort id = packet.GetUShort();
		print($"player {id} disconnected");
		Destroy(players[id].gameObject);
	}

	static void SpawnPlayer(ushort id, string username, Vector3 spawnpoint, float speed) {
		Player player;
		
		if (id == Client.instance.clientId) // hey look it's me!
		{
			player = Instantiate(GameLogic.instance.localPlayerPrefab).GetComponent<Player>();
			player.isLocal = true;
			player.movement.speed = speed;
		}
		else
		{
			player = Instantiate(GameLogic.instance.otherPlayerPrefab).GetComponent<Player>();
		}
		
		player.id = id;
		player.username = username;
		player.transform.position = spawnpoint;
	}
}
