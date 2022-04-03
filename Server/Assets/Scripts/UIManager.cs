using RiptideNetworking;

using UnityEngine;
using UnityEngine.UI;

public class UIManager: MonoBehaviour
{
	public Button startButton;

	public void StartGame() {
		startButton.interactable = false;
		// player player player player .....
		foreach (Player player in Player.players.Values)
			player.SwitchWorldsAndSend(World.Pier);
	}
}
