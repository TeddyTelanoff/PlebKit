using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
	public Button startButton;

	public void StartGame() {
		startButton.interactable = false;
		// player player player player .....
		foreach (Client client in Server.instance.clients.Values)
			client.player.SwitchWorldsAndSend(World.Pier);

		GameLogic.instance.spawnWorld = World.Pier;
	}
}
