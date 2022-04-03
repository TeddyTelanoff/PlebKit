using System.Collections.Generic;

using RiptideNetworking;

using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player: MonoBehaviour
{
	public static readonly Dictionary<ushort, Player> players = new Dictionary<ushort, Player>();
	
	public ushort id { get; private set; }
	public string username { get; private set; }

	public World world;
	public PlayerMovement movement;
	public PlayerUI ui;

	void OnValidate() {
		if (movement == null)
			movement = GetComponent<PlayerMovement>();
	}

	void OnDestroy() {
		players.Remove(id);
		Destroy(ui.gameObject);
	}

	public void SwitchWorldsAndSend(World newWorld) {
		Message msg = Message.Create(MessageSendMode.reliable, ServerToClientId.SwitchWorlds);
		msg.AddUShort(id);
		msg.AddUShort((ushort) newWorld);

		world = newWorld;
		transform.position = Vector3.zero;
		NetworkManager.instance.server.SendToAll(msg);
	}

	public static void Spawn(ushort id, string username) {
		foreach (Player other in players.Values)
			other.SendSpawnPlayer(id);
		
		Player player = Instantiate(GameLogic.instance.playerPrefab).GetComponent<Player>();
		player.name = $"player #{id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
		player.id = id;
		player.username = string.IsNullOrEmpty(username) ? $"Guest #{id}" : username;
		player.world = GameLogic.instance.spawnWorld;
		
		player.SendSpawnPlayer();
		players.Add(id, player);

		PlayerUI ui = Instantiate(GameLogic.instance.playerUIPrefab, GameLogic.instance.playerList).GetComponent<PlayerUI>();
		ui.transform.localPosition = new Vector3(ui.transform.localPosition.x, -30 * players.Count, ui.transform.localPosition.z);
		ui.player = player;
		player.ui = ui;
	}

	Message AddSpawnData(Message msg) =>
		msg.AddUShort(id).AddString(username).AddFloat(movement.speed).AddUShort((ushort) world);

	void SendSpawnPlayer(ushort to) {
		Message msg = Message.Create(MessageSendMode.reliable, ServerToClientId.SpawnPlayer);
		NetworkManager.instance.server.Send(AddSpawnData(msg), to);
	}

	void SendSpawnPlayer() {
		Message msg = Message.Create(MessageSendMode.reliable, ServerToClientId.SpawnPlayer);
		NetworkManager.instance.server.SendToAll(AddSpawnData(msg));
	}

	void SendPosition() {
		Message msg = Message.Create(MessageSendMode.unreliable, ServerToClientId.PlayerPosition);
		msg.AddVector3(transform.position);
		NetworkManager.instance.server.SendToAll(msg, id);
	}

	[MessageHandler((ushort) ClientToServerId.Name)]
	static void Name(ushort client, Message msg) {
		Spawn(client, msg.GetString());
	}

	[MessageHandler((ushort) ClientToServerId.PlayerPosition)]
	static void PlayerPosition(ushort client, Message msg) {
		if (players.TryGetValue(client, out Player player))
		{
			player.transform.position = msg.GetVector3();
			player.SendPosition();
		}
	}
}
