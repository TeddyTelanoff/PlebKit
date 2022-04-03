using RiptideNetworking;

using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerActivity))]
public class PlayerQuiz: MonoBehaviour
{
	public Player player;

	void OnValidate() {
		if (player == null)
			player = GetComponent<Player>();
	}

	public void Guess(int guess) {
		Message msg = Message.Create(MessageSendMode.reliable, ClientToServerId.QuizGuess);
		msg.AddInt(guess);
		NetworkManager.instance.client.Send(msg);

		GameLogic.instance.quizScreen.gameObject.SetActive(false);
	}
    
	[MessageHandler((ushort) ServerToClientId.Question)]
	static void Question(Message msg) {
		GameLogic.instance.quizScreen.gameObject.SetActive(true);
		GameLogic.instance.quizScreen.DisplayQuestion(new Question(msg.GetString(), msg.GetStrings()));
	}
    
	[MessageHandler((ushort) ServerToClientId.QuestionFeedback)]
	static void QuestionFeedback(Message msg) {
		GameLogic.instance.quizFeedbackScreen.gameObject.SetActive(true);
		int answer = msg.GetInt();
		int updatedBait = msg.GetInt();
		GameLogic.instance.quizFeedbackScreen.DisplayFeedback(answer, updatedBait - Player.localPlayer.bait);

		Player.localPlayer.bait = updatedBait;
	}
}
