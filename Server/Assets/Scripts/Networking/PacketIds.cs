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
	UpgradeResult,
	UpgradeInfo,
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