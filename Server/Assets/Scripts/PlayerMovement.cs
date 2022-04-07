using PacketExt;

using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
	[PacketHandler(ClientToServer.Movement)]
	static void Movement(ushort client, Packet packet) {
		Vector3 movement = packet.GetVector3();
		Server.instance.clients[client].player.transform.position = movement;

		BroadcastMovement(client, movement);
	}

	static void BroadcastMovement(ushort client, Vector3 movement) {
		Packet packet = Packet.Create(ServerToClient.PlayerMovement);
		packet.AddUShort(client);
		packet.AddVector3(movement);
		Server.instance.SendAll(packet, client);
	}
}
