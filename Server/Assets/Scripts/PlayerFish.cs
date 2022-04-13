using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerFish: MonoBehaviour
{
	public Player player;

	public float waitTime;
	
	public int bait;
	public int[] fishes;
	public int fishCapactiyPerSpecie => player.upgrades.HasFlag(Upgrade.Backpack) ? 20 : 8;

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
			float gain = fishes[i] * GameLogic.instance.fishSpecies[i].value;
			if (player.upgrades.HasFlag(Upgrade.Value))
				gain *= 1.5f; // yes value op
			player.money += gain;
			fishes[i] = 0;
		}
		
		player.SendInventoryUpdate();
	}

	void SendFishResult(int fishSpecieId) {
		Packet packet = Packet.Create(ServerToClient.FishResult);
		packet.AddInt(fishSpecieId);
		packet.AddInt(fishes[fishSpecieId]);
		Server.instance.Send(packet, player.client.id);
	}

	public void GoFishing() {
		IEnumerator CoRoutine() {
			int i = 0;
			if (bait > 0)
			{
				yield return new WaitForSeconds(waitTime);

				float v = Random.Range(0, GameLogic.instance.totalFishSpeciesChance);
				float a = 0;
				for (; i < GameLogic.instance.fishSpecies.Length; i++)
					if (v < a + GameLogic.instance.fishSpecies[i].chance)
					{
						if (fishes[i] < fishCapactiyPerSpecie)
							fishes[i]++;

						break;
					}
					else
						a += GameLogic.instance.fishSpecies[i].chance;

				bait--;

			}

			fishing = false;
			SendFishResult(i);
		}

		if (fishing)
			return; // teddy, just please stop, it's not fair
		fishing = true;

		// todo fishing areas
		StartCoroutine(CoRoutine());
	}
}