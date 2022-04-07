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

	[PacketHandler(ServerToClient.PlayerMovement)]
	static void PlayerMovement(Packet packet) {
		ushort client = packet.GetUShort();
		players[client].transform.position = packet.GetVector3();
	}

	[PacketHandler(ServerToClient.Spawn)]
	static void OnSpawn(Packet packet) {
		ushort id = packet.GetUShort();
		string username = packet.GetString();
		Vector3 spawnpoint = packet.GetVector3();
		print($"{username} spawned!, spawnpoint: {spawnpoint}");

		SpawnPlayer(id, username, spawnpoint);
	}

	static void SpawnPlayer(ushort id, string username, Vector3 spawnpoint) {
		Player player;
		
		if (id == Client.instance.clientId) // hey look it's me!
		{
			player = Instantiate(GameLogic.instance.localPlayerPrefab).GetComponent<Player>();
			player.isLocal = true;
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
