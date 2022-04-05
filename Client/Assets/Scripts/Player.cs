using PacketExt;

using UnityEngine;

public class Player: MonoBehaviour
{
	void FixedUpdate() {
		SendPosition();
	}

	public void SendPosition() {
		Packet packet = Packet.Create(ClientToServer.Position);
		packet.AddVector3(transform.position);
		Client.instance.Send(packet);
	}
}
