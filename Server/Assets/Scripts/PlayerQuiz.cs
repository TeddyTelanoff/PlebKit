using RiptideNetworking;

using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerQuiz: MonoBehaviour
{
	public Question question;

	public Player player;

	void OnValidate() {
		if (player == null)
			player = GetComponent<Player>();
	}

	public void NewQuestion() {
		question = GameLogic.instance.questions[Random.Range(0, GameLogic.instance.questions.Length)];
		JumbleQuestion();
	}

	public void SendQuestion() {
		Message msg = Message.Create(MessageSendMode.reliable, ServerToClientId.Question);
		msg.AddString(question.prompt);
		msg.AddStrings(question.choices);
		NetworkManager.instance.server.Send(msg, player.id);
	}

	public void JumbleQuestion() {
		// we can directly edit field because this.question is a copy of GameLogic.questions
		int[] rngs = new int[4];
		rngs[0] = Random.Range(0, 4);
		rngs[1] = Random.Range(0, 3);
		rngs[2] = Random.Range(0, 2);
		rngs[3] = 0;

		if (rngs[1] == rngs[0])
			rngs[1]++;
		while (rngs[2] == rngs[0] || rngs[2] == rngs[1])
			rngs[2]++;
		while (rngs[3] == rngs[0] || rngs[3] == rngs[1] || rngs[3] == rngs[2])
			rngs[3]++;

		string[] newAnswers = new string[4];
		newAnswers[rngs[0]] = question.choices[0];
		newAnswers[rngs[1]] = question.choices[1];
		newAnswers[rngs[2]] = question.choices[2];
		newAnswers[rngs[3]] = question.choices[3];

		question.answer = rngs[question.answer];
		question.choices = newAnswers;
	}

	void SendFeedback() {
		Message msg = Message.Create(MessageSendMode.reliable, ServerToClientId.QuestionFeedback);
		msg.AddInt(question.answer);
		msg.AddInt(player.bait);
		NetworkManager.instance.server.Send(msg, player.id);
	}

	public void Guess(int guess) {
		if (question.answer == guess)
			player.bait++;
		else
			player.bait--;

		SendFeedback();
	}
}
