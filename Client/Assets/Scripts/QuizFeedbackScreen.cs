using UnityEngine;
using UnityEngine.UI;

public class QuizFeedbackScreen: MonoBehaviour
{
	public Color rightColor;
	public Color wrongColor;

	public Image feedbackPanel;
	public Text feedbackText;

	public void DisplayFeedback(int answer, int diffInBait) {
		if (diffInBait > 0)
		{
			feedbackPanel.color = rightColor;
			feedbackText.text = $"+{diffInBait} Bait";
		}
		else
		{
			feedbackPanel.color = wrongColor;
			feedbackText.text = $"{diffInBait /* don't need to add '-', cuz it is already added since diffInBait is negative */} Bait";
		}
		
		// todo show correct answer
	}
}
