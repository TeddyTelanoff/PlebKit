using PacketExt;

using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
	public Client client;

	public PlayerMovement movement;
	public PlayerListItem listItem;

	public string username;
	public World world;

	void OnValidate() {
		if (client == null)
			client = GetComponentInParent<Client>();
		
		if (movement == null)
			movement = GetComponent<PlayerMovement>();
	}

	void OnDestroy() {
		PlayerListManager.instance.RemoveItem(listItem);
	}

	public void SwitchWorldsAndSend(World newWorld) {
		Packet packet = Packet.Create(ServerToClient.SwitchWorlds);
		packet.AddUShort(client.id);
		packet.AddUShort((ushort) newWorld);

		world = newWorld;
		transform.position = Vector3.zero;
		Server.instance.SendAll(packet);
	}

	public static void Spawn(ushort clientId, string username) {
		Client client = Server.instance.clients[clientId];
		client.player.gameObject.SetActive(true);
		client.player.username = username;
		client.player.name = $"{username} #{clientId}";
		
		PlayerListManager.instance.AddItem(client.player);
	}
}
