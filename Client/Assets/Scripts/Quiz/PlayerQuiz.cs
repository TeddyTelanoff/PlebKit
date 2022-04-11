using PacketExt;

using UnityEngine;

public class PlayerQuiz: MonoBehaviour
{
	public Question question;

	public void Guess(int guess) {
		Packet packet = Packet.Create(ClientToServer.QuizGuess);
		packet.AddInt(guess);
		Client.instance.Send(packet);
		
		GameLogic.instance.quizScreen.gameObject.SetActive(false);
	}
	
	[PacketHandler(ServerToClient.Question)]
	public static void Question(Packet packet) {
		GameLogic.instance.quizScreen.gameObject.SetActive(true);
		
		var question = new Question { prompt = packet.GetString(), choices = packet.GetStrings() };
		Player.localPlayer.GetComponent<PlayerQuiz>().question = question;
		GameLogic.instance.quizScreen.DisplayQuestion(question);
	}
	
	[PacketHandler(ServerToClient.QuestionFeedback)]
	public static void QuestionFeedback(Packet packet) {
		GameLogic.instance.quizFeedbackScreen.gameObject.SetActive(true);
		int answer = packet.GetInt();
		int updatedBait = packet.GetInt();
		GameLogic.instance.quizFeedbackScreen.DisplayFeedback(
			Player.localPlayer.GetComponent<PlayerQuiz>().question.choices[answer],
			updatedBait - Player.localPlayer.fish.bait);
		Player.localPlayer.fish.bait = updatedBait;
	}
}
