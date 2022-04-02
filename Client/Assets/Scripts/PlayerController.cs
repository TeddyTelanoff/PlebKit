using RiptideNetworking;

using UnityEngine;

public class PlayerController: MonoBehaviour
{
	public const int inputCount = 5;

	bool[] inputs;

	void Start() {
		inputs = new bool[inputCount];
	}

	void Update() {
		inputs[0] |= Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
		inputs[1] |= Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
		inputs[2] |= Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
		inputs[3] |= Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
		inputs[4] |= Input.GetKey(KeyCode.Space);
	}

	void FixedUpdate() {
		SendInputs();
		for (int i = 0; i < inputs.Length; i++)
			inputs[i] = false;
	}

	void SendInputs() {
		Message msg = Message.Create(MessageSendMode.unreliable, ClientToServerId.Input);
		msg.AddBools(inputs, false);
		NetworkManager.instance.client.Send(msg);
	}
}
