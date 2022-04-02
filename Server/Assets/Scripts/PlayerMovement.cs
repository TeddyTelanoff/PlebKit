using RiptideNetworking;

using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement: MonoBehaviour
{
	public const int inputCount = 5;
	
	public Player player;
	public Rigidbody rb;

	public float speed;
	public bool[] inputs;

	void OnValidate() {
		if (player == null)
			player = GetComponent<Player>();
		if (rb == null)
			rb = GetComponent<Rigidbody>();
	}

	void Start() {
		inputs = new bool[inputCount];
	}

	void FixedUpdate() {
		Vector2 dir = Vector2.zero;
		if (inputs[0])
			dir.y--;
		if (inputs[1])
			dir.x--;
		if (inputs[2])
			dir.y++;
		if (inputs[3])
			dir.x++;
		
		Move(dir);
	}

	void Move(Vector2 dir) {
		Vector3 force = new Vector3(dir.x, 0, dir.y);
		force *= speed;
		force *= Time.deltaTime;
		
		rb.AddForce(force, ForceMode.Impulse);
		SendMovement();
	}

	void SendMovement() {
		Message msg = Message.Create(MessageSendMode.unreliable, ServerToClientId.PlayerMovement);
		msg.AddUShort(player.id);
		msg.AddVector3(transform.position);
		
		NetworkManager.instance.server.SendToAll(msg);
	}
}
