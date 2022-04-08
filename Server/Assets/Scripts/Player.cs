using PacketExt;

using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class Player : MonoBehaviour
{
	public Client client;

	public PlayerMovement movement;

	public string username;
	public World world;

	void OnValidate() {
		if (client == null)
			client = GetComponentInParent<Client>();
		
		if (movement == null)
			movement = GetComponent<PlayerMovement>();
	}

	public void SwitchWorldsAndSend(World newWorld) {
		Packet packet = Packet.Create(ServerToClient.SwitchWorlds);
		packet.AddUShort(client.id);
		packet.AddUShort((ushort) newWorld);

		world = newWorld;
		transform.position = Vector3.zero;
		Server.instance.SendAll(packet);
	}
}
