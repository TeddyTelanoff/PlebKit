using PacketExt;

using UnityEngine;

// player movement for local player
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement: MonoBehaviour
{
	public Rigidbody rb;
	public float speed;

	void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate() {
		Vector3 dir = Vector3.zero;
		dir.x = Input.GetAxisRaw("Horizontal");
		dir.z = Input.GetAxisRaw("Vertical");
		Move(dir);
		
		if (transform.position.y <= -10)
			transform.position = Vector3.zero;

		SendPosition();
	}

	void Move(Vector3 dir) {
		// I like my code hard ;)
		if (Player.localPlayer.world == World.Lobby)
			dir *= 1883;
		else
			dir *= speed;
		dir *= Time.deltaTime;

		rb.AddForce(dir);
	}

	void SendPosition() {
		Packet packet = Packet.Create(ClientToServer.Movement);
		packet.AddVector3(transform.position);
		Client.instance.Send(packet);
	}
}
