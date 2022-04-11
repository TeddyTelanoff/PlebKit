using System.Collections;

using UnityEngine;

public class PlayerFish: MonoBehaviour
{
	public int bait;
	public int[] fishes;

	void Start() {
		fishes = new int[GameLogic.instance.fishSpecies.Length];
	}

	public void GoFishing() {
		if (bait <= 0)
			return;
		
		IEnumerator HackySolution() {
			SendDoFish();

			yield return new WaitForFixedUpdate();
			SendDoFish();

			yield return new WaitForFixedUpdate();
			SendDoFish();
		}
		
		GameLogic.instance.fishScreen.gameObject.SetActive(true);
		StartCoroutine(HackySolution());
	}

	[PacketHandler(ServerToClient.FishResult)]
	public static void FishResult(Packet packet) {
		int specieId = packet.GetInt();
		Player.localPlayer.fish.fishes[specieId]++;
		Player.localPlayer.fish.bait--;

		Player.UpdateSupplyDisplay();
		GameLogic.instance.fishScreen.DisplayResult(ref GameLogic.instance.fishSpecies[specieId]);
	}

	void SendDoFish() {
		Packet packet = Packet.Create(ClientToServer.Fish);
		Client.instance.Send(packet);
	}
}
