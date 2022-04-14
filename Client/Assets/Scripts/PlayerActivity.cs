using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public enum Activity: ushort
{
	None,
	Quiz,
	Fish,
	Sell,
	Upgrade,
}

[RequireComponent(typeof(Player))]
public class PlayerActivity: MonoBehaviour
{
	public delegate void ActivityDoer();

	public Dictionary<Activity, ActivityDoer> activityDoers;

	public Player player;
	
	public GameObject activityButton;
	public Text activityText;
	public Activity activity;

	void OnValidate() {
		if (player == null)
			player = GetComponent<Player>();
	}

	void Awake() {
		activityButton = GameLogic.instance.activityButton;
		activityText = GameLogic.instance.activityText;
		
		activityDoers = new Dictionary<Activity, ActivityDoer> {
			{ Activity.Quiz, SendDoQuiz },
			{ Activity.Fish, DoFish },
			{ Activity.Sell, SendSellFish },
			{ Activity.Upgrade, OpenUpgradeScreen },
		};
	}

	public void SwitchActivity(Activity newActivity) {
		activity = newActivity;
		activityText.text = GameLogic.instance.activityTexts[(int) activity];
        
		activityButton.SetActive(activity != Activity.None);
	}

	public void StartActivity() {
		activityButton.SetActive(false);
		activityDoers[activity]();

		player.movement.enabled = false;
	}

	public void FinishActivity() {
		activityButton.SetActive(activity != Activity.None);
		player.movement.enabled = true;
	}

	public void FinishUpgrade() {
		FinishActivity();
	}

	public void SendDoQuiz() {
		Packet packet = Packet.Create(ClientToServer.Quiz);
		Client.instance.Send(packet);
	}

	void DoFish() {
		Player.localPlayer.fish.GoFishing();
	}

	void SendSellFish() {
		Packet packet = Packet.Create(ClientToServer.SellFish);
		Client.instance.Send(packet);
	}

	void OpenUpgradeScreen() {
		GameLogic.instance.upgradeScreen.SetActive(true);
	}
}
