using System.Collections.Generic;

using RiptideNetworking;

using UnityEngine;

public class Player: MonoBehaviour
{
	public static Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();
	
	public ushort id { get; private set; }
	public bool isLocal { get; private set; }

	public string username { get; private set; }

	void OnDestroy() {
		players.Remove(id);
	}

	void Move(Vector3 newPos) {
		transform.position = newPos;
	}

	public static void Spawn(ushort id, string username, Vector3 pos) {
		Player player;
		if (id == NetworkManager.instance.client.Id)
		{
			player = Instantiate(GameLogic.instance.localPlayerPrefab, pos, Quaternion.identity).GetComponent<Player>();
			player.isLocal = true;
		}
		else
		{
			player = Instantiate(GameLogic.instance.otherPlayerPrefab, pos, Quaternion.identity).GetComponent<Player>();
			player.isLocal = false;
		}
		
		player.name = $"player #{id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
		player.id = id;
		player.username = string.IsNullOrEmpty(username) ? $"Guest #{id}" : username;
		
		players.Add(id, player);
	}

	[MessageHandler((ushort) ServerToClientId.SpawnPlayer)]
	static void SpawnPlayer(Message msg) {
		Spawn(msg.GetUShort(), msg.GetString(), msg.GetVector3());
	}

	[MessageHandler((ushort) ServerToClientId.PlayerMovement)]
	static void PlayerMovement(Message msg) {
		if (players.TryGetValue(msg.GetUShort(), out Player player))
			player.Move(msg.GetVector3());
	}
}
