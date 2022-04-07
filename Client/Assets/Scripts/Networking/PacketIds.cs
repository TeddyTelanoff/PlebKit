public enum ServerToClient: ushort
{
	Welcome,
	Spawn,
	PlayerMovement,
}

public enum ClientToServer: ushort
{
	Join,
	Movement,
}