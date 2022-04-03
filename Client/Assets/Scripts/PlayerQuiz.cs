using RiptideNetworking;

using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerActivity))]
public class PlayerQuiz: MonoBehaviour
{
	public Question question;

	public void Guess(int guess) {
		Message msg = Message.Create(MessageSendMode.reliable, ClientToServerId.QuizGuess);
		msg.AddInt(guess);
		NetworkManager.instance.client.Send(msg);

		GameLogic.instance.quizScreen.gameObject.SetActive(false);
	}
    
	[MessageHandler((ushort) ServerToClientId.Question)]
	static void Question(Message msg) {
		GameLogic.instance.quizScreen.gameObject.SetActive(true);
		
		var question = new Question(msg.GetString(), msg.GetStrings());
		Player.localPlayer.GetComponent<PlayerQuiz>().question = question;
		GameLogic.instance.quizScreen.DisplayQuestion(question);
	}
    
	[MessageHandler((ushort) ServerToClientId.QuestionFeedback)]
	static void QuestionFeedback(Message msg) {
		GameLogic.instance.quizFeedbackScreen.gameObject.SetActive(true);
		int answer = msg.GetInt();
		int updatedBait = msg.GetInt();
		GameLogic.instance.quizFeedbackScreen.DisplayFeedback(Player.localPlayer.GetComponent<PlayerQuiz>().question.choices[answer],
															  updatedBait - Player.localPlayer.bait);

		Player.localPlayer.bait = updatedBait;
	}
}
