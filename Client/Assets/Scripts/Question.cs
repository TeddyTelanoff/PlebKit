using System;

[Serializable]
public struct Question
{
	public string prompt;
	public string[] choices;
	public int answer;

	public Question(string prompt, string[] choices) {
		this.prompt = prompt;
		this.choices = choices;
		answer = -1; // we are client we should not know answer
	}
}