using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerFish: MonoBehaviour
{
	public Player player;

	public float waitTime;
	
	public int bait;
	public int[] fishes;

	public bool fishing;

	void OnValidate() {
		if (player == null)
			player = GetComponent<Player>();
	}

	void Start() {
		fishes = new int[GameLogic.instance.fishSpecies.Length];
	}

	public void SellFish() {
		for (int i = 0; i < fishes.Length; i++)
		{
			player.money += fishes[i] * GameLogic.instance.fishSpecies[i].value;
			fishes[i] = 0;
		}
		
		player.SendInventoryUpdate();
	}

	void SendFishResult(int fishSpecieId) {
		Packet packet = Packet.Create(ServerToClient.FishResult);
		packet.AddInt(fishSpecieId);
		Server.instance.Send(packet, player.client.id);
	}

	public void GoFishing() {
		IEnumerator CoRoutine() {
			yield return new WaitForSeconds(waitTime);
			float v = Random.Range(0, GameLogic.instance.totalFishSpeciesChance);
			float a = 0;
			int i = 0;
			for (; i < GameLogic.instance.fishSpecies.Length; i++)
				if (v < a + GameLogic.instance.fishSpecies[i].chance)
				{
					fishes[i]++;

					break;
				}
				else
					a += GameLogic.instance.fishSpecies[i].chance;
			bait--;

			fishing = false;
			SendFishResult(i);
		}

		if (bait <= 0)
			return; // stop cheating, teddy
		
		if (fishing)
			return; // teddy, just please stop, it's not fair
		fishing = true;

		// todo fishing areas
		StartCoroutine(CoRoutine());
	}
}