public enum ServerToClient: ushort
{
	Welcome,
	Disconnect,
	Spawn,
	PlayerMovement,
	SwitchWorlds,
}

public enum ClientToServer: ushort
{
	Join,
	Movement,
}