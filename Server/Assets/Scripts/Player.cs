using System.Collections.Generic;

using RiptideNetworking;

using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player: MonoBehaviour
{
	public static readonly Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();
	
	public ushort id { get; private set; }
	public string username { get; private set; }

	public PlayerMovement movement;

	void OnValidate() {
		if (movement == null)
			movement = GetComponent<PlayerMovement>();
	}

	void OnDestroy() {
		players.Remove(id);
	}

	public static void Spawn(ushort id, string username) {
		foreach (Player other in players.Values)
			other.SendSpawnPlayer(id);
		
		Player player = Instantiate(GameLogic.instance.playerPrefab).GetComponent<Player>();
		player.name = $"player #{id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
		player.id = id;
		player.username = string.IsNullOrEmpty(username) ? $"Guest #{id}" : username;
		
		player.SendSpawnPlayer();
		players.Add(id, player);
	}

	Message AddSpawnData(Message msg) {
		msg.AddUShort(id).AddString(username).AddVector3(transform.position);
		return msg;
	}

	void SendSpawnPlayer(ushort to) {
		Message msg = Message.Create(MessageSendMode.reliable, ServerToClientId.SpawnPlayer);
		NetworkManager.instance.server.Send(AddSpawnData(msg), to);
	}

	void SendSpawnPlayer() {
		Message msg = Message.Create(MessageSendMode.reliable, ServerToClientId.SpawnPlayer);
		NetworkManager.instance.server.SendToAll(AddSpawnData(msg));
	}

	[MessageHandler((ushort) ClientToServerId.Name)]
	static void Name(ushort client, Message msg) {
		Spawn(client, msg.GetString());
	}

	[MessageHandler((ushort) ClientToServerId.Input)]
	static void Input(ushort client, Message msg) {
		if (players.TryGetValue(client, out Player player))
			player.movement.inputs = msg.GetBools(PlayerMovement.inputCount);
	}
}
