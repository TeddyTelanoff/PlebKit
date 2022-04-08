public enum ServerToClient: ushort
{
	Welcome,
	Disconnect,
	Spawn,
	PlayerMovement,
}

public enum ClientToServer: ushort
{
	Join,
	Movement,
}