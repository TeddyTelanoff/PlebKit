using UnityEngine;

public class PlayerFish: MonoBehaviour
{
	public int bait;
	public int[] fishes;

	void Start() {
		fishes = new int[GameLogic.instance.fishSpecies.Length];
	}

	public void GoFishing() {
		SendDoFish();
		GameLogic.instance.fishScreen.gameObject.SetActive(true);
	}

	[PacketHandler(ServerToClient.FishResult)]
	public static void FishResult(Packet packet) {
		int specieId = packet.GetInt();
		Player.localPlayer.fish.fishes[specieId]++;
		Player.localPlayer.fish.bait--;
		GameLogic.instance.fishScreen.DisplayResult(ref GameLogic.instance.fishSpecies[specieId]);
	}

	void SendDoFish() {
		Packet packet = Packet.Create(ClientToServer.Fish);
		Client.instance.Send(packet);
	}
}
