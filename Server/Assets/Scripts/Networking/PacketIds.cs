public enum ServerToClient: ushort
{
	Welcome,
	Disconnect,
	Spawn,
	PlayerMovement,
	SwitchWorlds,
	Question,
	QuestionFeedback,
}

public enum ClientToServer: ushort
{
	Join,
	Movement,
	Quiz,
	QuizGuess,
}