using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerFish: MonoBehaviour
{
	public Player player;
	
	public int bait;
	public int[] fishes;

	void OnValidate() {
		if (player == null)
			player = GetComponent<Player>();
	}

	public void GoFishing() {
		int i = Random.Range(0, GameLogic.instance.fishSpeciesValue.Length);
		fishes[i]++;
		bait--;
	}
}
