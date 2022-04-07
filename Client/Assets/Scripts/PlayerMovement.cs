using PacketExt;

using UnityEngine;

// player movement for local player
public class PlayerMovement: MonoBehaviour
{
	void FixedUpdate() {
		SendPosition();
	}

	void SendPosition() {
		Packet packet = Packet.Create(ClientToServer.Movement);
		packet.AddVector3(transform.position);
		Client.instance.Send(packet);
	}
}
