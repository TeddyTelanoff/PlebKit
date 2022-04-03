using System.Collections.Generic;

using RiptideNetworking;

using UnityEngine;

public class Player: MonoBehaviour
{
	public static Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();
	public static Player localPlayer { get; private set; }
	
	public ushort id { get; private set; }
	public bool isLocal { get; private set; }

	public string username { get; private set; }

	[SerializeField]
	PlayerMovement movement;

	public World world { get; private set; }

	void OnDestroy() {
		players.Remove(id);
		GameLogic.instance.activityButton.SetActive(false);
	}

	void Move(Vector3 newPos) {
		transform.position = newPos;
	}

	public static void Spawn(ushort id, string username, Vector3 pos, float speed, World world) {
		Player player;
		if (id == NetworkManager.instance.client.Id)
		{
			player = Instantiate(GameLogic.instance.localPlayerPrefab, pos, Quaternion.identity).GetComponent<Player>();
			localPlayer = player;
			player.isLocal = true;
			player.movement.speed = speed;
			GameLogic.instance.activityButton.SetActive(false);
			GameLogic.instance.SwitchWorlds(World.Lobby, world);
		}
		else
		{
			player = Instantiate(GameLogic.instance.otherPlayerPrefab, pos, Quaternion.identity).GetComponent<Player>();
			player.isLocal = false;
		}
		
		player.name = $"player #{id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
		player.id = id;
		player.username = string.IsNullOrEmpty(username) ? $"Guest #{id}" : username;
		player.world = world;
		
		players.Add(id, player);
	}

	[MessageHandler((ushort) ServerToClientId.SpawnPlayer)]
	static void SpawnPlayer(Message msg) {
		Spawn(msg.GetUShort(), msg.GetString(), Vector3.zero, msg.GetFloat(), (World) msg.GetUShort());
	}

	[MessageHandler((ushort) ServerToClientId.PlayerPosition)]
	static void PlayerMovement(Message msg) {
		if (players.TryGetValue(msg.GetUShort(), out Player player))
			player.Move(msg.GetVector3());
	}

	[MessageHandler((ushort) ServerToClientId.SwitchWorlds)]
	static void SwitchWorlds(Message msg) {
		if (players.TryGetValue(msg.GetUShort(), out Player player))
		{
			World newWorld = (World) msg.GetUShort();
			
			if (player.isLocal)
				GameLogic.instance.SwitchWorlds(player.world, newWorld);
			else if (newWorld != localPlayer.world)
				player.gameObject.SetActive(false); // hide players from other worlds
			
			player.world = newWorld;
			player.transform.position = Vector3.zero;
		}
	}
}
