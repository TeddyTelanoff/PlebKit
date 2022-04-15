using System;
using System.Collections;

using UnityEngine;

public class PlayerListManager: MonoBehaviour
{
	public static PlayerListManager instance;
	
	public GameObject itemPrefab;

	void Awake() {
		instance = this;
	}

	void Start() {
		StartCoroutine(UpdateMoneyRoutine());
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
		if (Server.instance.clients.Count <= 1)
			return;
		
		int count = 0;
		Client[] clients = new Client[Server.instance.clients.Count];
		Server.instance.clients.Values.CopyTo(clients, 0);
		Array.Sort(clients, ClientComparison);

		foreach (Client client in clients)
		{
			Player player = client.player;
			player.listItem.transform.localPosition = new Vector3(player.listItem.transform.localPosition.x, -30 + -60 * count,
																  player.listItem.transform.localPosition.z);

			player.listItem.moneyText.text = $"${player.money:F2}";
			count++;
		}
	}

	int ClientComparison(Client x, Client y) {
		return x.player.money.CompareTo(y.player.money);
	}

	IEnumerator UpdateMoneyRoutine() {
		while (true)
		{
			ReorderList();

			yield return new WaitForSeconds(10);
		}
	}
}
