using UnityEngine;
using UnityEngine.UI;

public class FishScreen: MonoBehaviour
{
	public Text fishingText;
	public GameObject resultScreen;
	public Text fishResultText;
	public Text valueText;

	void OnEnable() {
		fishingText.gameObject.SetActive(true);
		resultScreen.gameObject.SetActive(false);
	}
	
	public void DisplayResult(ref FishSpecie specie) {
		fishingText.gameObject.SetActive(false);
		resultScreen.gameObject.SetActive(true);

		fishResultText.text = specie.name;
		valueText.text = $"${specie.value:F2}";
	}

	public void FishAgain() {
		gameObject.SetActive(false);
		Player.localPlayer.fish.GoFishing();
	}

	public void Exit() {
		gameObject.SetActive(false);
		Player.localPlayer.GetComponent<PlayerActivity>().FinishActivity();
	}
}
