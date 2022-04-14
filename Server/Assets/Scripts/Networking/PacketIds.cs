public enum ServerToClient: ushort
{
	Welcome,
	Disconnect,
	Spawn,
	PlayerMovement,
	SwitchWorlds,
	Question,
	QuestionFeedback,
	InventoryUpdate,
	FishResult,
	UpgradeInfo,
	UpgradeResult,
}

public enum ClientToServer: ushort
{
	Join,
	Movement,
	Quiz,
	QuizGuess,
	Fish,
	SellFish,
	Upgrade,
	UpgradeInfo,
}