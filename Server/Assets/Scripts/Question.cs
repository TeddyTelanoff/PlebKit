using System;

[Serializable]
public struct Question
{
	public string prompt;
	public string[] choices;
	public int answer;
}