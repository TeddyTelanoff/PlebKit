using UnityEngine;
using UnityEngine.UI;

public class PlayerMenu: MonoBehaviour
{
	public Player player;
	public Text usernameText;

	public void Select(Player player) {
		usernameText.text = player.username;
		this.player = player;
	}

	public void Kick() {
		player.client.Disconnect("kicked");
		gameObject.SetActive(false);
	}
}
