using UnityEngine;
using UnityEngine.UI;

public class StartButton: MonoBehaviour
{
	public Button startButton;

	void OnValidate() {
		if (startButton == null)
			startButton = GetComponent<Button>();
	}

	public void StartGame() {
		startButton.interactable = false;
		// player player player player .....
		foreach (Client client in Server.instance.clients.Values)
			client.player.SwitchWorldsAndSend(World.Pier);

		GameLogic.instance.spawnWorld = World.Pier;
	}
}
