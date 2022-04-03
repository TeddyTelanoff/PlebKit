using RiptideNetworking;

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement: MonoBehaviour
{
	public float speed;
	bool[] inputs;

	[SerializeField]
	Rigidbody rb;

	void OnValidate() {
		if (rb == null)
			rb = GetComponent<Rigidbody>();
	}

	void Start() {
		inputs = new bool[5];
	}

	void Update() {
		inputs[0] |= Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
		inputs[1] |= Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
		inputs[2] |= Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
		inputs[3] |= Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
		inputs[4] |= Input.GetKey(KeyCode.Space);
	}

	void FixedUpdate() {
		Move();
		SendPosition();
		
		for (int i = 0; i < inputs.Length; i++)
			inputs[i] = false;
	}

	void Move() {
		Vector3 vel = Vector3.zero;
		if (inputs[0])
			vel.z++;
		if (inputs[1])
			vel.x--;
		if (inputs[2])
			vel.z--;
		if (inputs[3])
			vel.x++;

		vel *= speed;
		vel *= Time.deltaTime;
		
		rb.AddForce(vel, ForceMode.Impulse);
	}

	void SendPosition() {
		Message msg = Message.Create(MessageSendMode.unreliable, ClientToServerId.PlayerPosition);
		msg.AddVector3(transform.position);
		NetworkManager.instance.client.Send(msg);
	}
}
