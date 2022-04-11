
using UnityEngine;
using UnityEngine.UI;

public class QuizScreen: MonoBehaviour
{
	public Text promptText;
	public Text[] choiceTexts;

	public void DisplayQuestion(Question question) {
		promptText.text = question.prompt;
		for (int i = 0; i < choiceTexts.Length; i++)
			choiceTexts[i].text = question.choices[i];
	}
}