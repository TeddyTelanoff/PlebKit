using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public enum Activity: ushort
{
	None,
	Quiz,
	Fish,
}

public class PlayerActivity: MonoBehaviour
{
	public delegate void ActivityDoer();

	public Dictionary<Activity, ActivityDoer> activityDoers;

	public GameObject activityButton;
	public Text activityText;
	public Activity activity;

	void Awake() {
		activityButton = GameLogic.instance.activityButton;
		activityText = GameLogic.instance.activityText;
		
		activityDoers = new Dictionary<Activity, ActivityDoer> {
			{ Activity.Quiz, SendDoQuiz },
			{ Activity.Fish, DoFish },
		};
	}

	public void SwitchActivity(Activity newActivity) {
		activity = newActivity;
		activityText.text = GameLogic.instance.activityTexts[(int) activity];
        
		activityButton.SetActive(activity != Activity.None);
	}

	public void DoActivity() {
		activityButton.SetActive(false);
		activityDoers[activity]();
	}

	public void SendDoQuiz() {
		Packet packet = Packet.Create(ClientToServer.Quiz);
		Client.instance.Send(packet);
	}

	void DoFish() {
		Player.localPlayer.fish.GoFishing();
	}

	public void FinishActivity() {
		activityButton.SetActive(activity != Activity.None);
	}
}
