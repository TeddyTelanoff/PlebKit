using UnityEngine;

public class PlayerListManager: MonoBehaviour
{
	public static PlayerListManager instance;
	
	public GameObject itemPrefab;

	void Awake() {
		instance = this;
	}

	public void AddItem(Player player) {
		PlayerListItem item = Instantiate(itemPrefab, transform).GetComponent<PlayerListItem>();
		item.player = player;
		player.listItem = item;
		ReorderList();
	}

	public void RemoveItem(PlayerListItem item) {
		Destroy(item.gameObject);
		ReorderList();
	}

	public void ReorderList() {
		int count = 0;
		for (ushort i = 1; i <= Server.instance.maxClients; i++)
		{
			if (!Server.instance.clients.ContainsKey(i))
				continue;
			
			Player player = Server.instance.clients[i].player;
			player.listItem.transform.localPosition = new Vector3(player.listItem.transform.localPosition.x, -30 + -60 * count,
																  player.listItem.transform.localPosition.z);
			count++;
		}
	}
}
