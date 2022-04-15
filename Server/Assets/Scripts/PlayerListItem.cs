using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem: MonoBehaviour
{
	public Text usernameText;
	public Text moneyText;
	public Text idText;

	public Player player;

	void Start() {
		usernameText.text = player.username;
		idText.text = player.client.id.ToString();
	}

	public void Select() {
		GameLogic.instance.playerMenu.gameObject.SetActive(true);
		GameLogic.instance.playerMenu.Select(player);
	}
}
