using UnityEngine;
using UnityEngine.UI;

public class QuizFeedbackScreen: MonoBehaviour
{
	public Color rightColor;
	public Color wrongColor;

	public Image feedbackPanel;
	public Text feedbackText;

	public GameObject viewCorrectAnswerButton;

	public string answer;
	
	public void DisplayFeedback(string answer, int diffInBait) {
		if (diffInBait > 0)
		{
			feedbackPanel.color = rightColor;
			feedbackText.text = $"+{diffInBait} Bait";
			viewCorrectAnswerButton.SetActive(false);
		}
		else
		{
			feedbackPanel.color = wrongColor;
			feedbackText.text = $"{diffInBait /* don't need to add '-', cuz it is already added since diffInBait is negative */} Bait";
			viewCorrectAnswerButton.SetActive(true);
		}

		this.answer = answer;
	}

	public void ViewCorrectAnswer() {
		viewCorrectAnswerButton.SetActive(false);
		feedbackText.text = answer;
	}

	public void NextQustion() {
		gameObject.SetActive(false);
		Player.localPlayer.GetComponent<PlayerActivity>().SendDoQuiz();
	}

	public void Exit() {
		gameObject.SetActive(false);
		Player.localPlayer.GetComponent<PlayerActivity>().FinishActivity();
	}
}