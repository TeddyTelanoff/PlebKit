using System.Collections;

using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerFish: MonoBehaviour
{
	public Player player;

	public float waitTime;

	public float luck;
	public int bait;
	public int[] fishes;
	public int fishCapactiyPerSpecie;

	public bool fishing;

	public float value;
	
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
			gain *= value; // yes value op
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

				int nSpecies = GameLogic.instance.fishSpecies.Length;
				float totalChance = 0;
				for (int j = 0; j < nSpecies; j++)
					totalChance += GameLogic.instance.fishSpecies[j].chance * ((luck - 1) * (j / (float) nSpecies) + 1);

				
				float v = Random.Range(0, totalChance);
				for (; i < GameLogic.instance.fishSpecies.Length; i++)
					if (v < GameLogic.instance.fishSpecies[i].chance * ((luck - 1) * (i / (float) nSpecies) + 1))
					{
						if (fishes[i] < fishCapactiyPerSpecie)
							fishes[i]++;

						break;
					}
					else
						v += GameLogic.instance.fishSpecies[i].chance * ((luck - 1) * (i / (float) nSpecies) + 1);

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