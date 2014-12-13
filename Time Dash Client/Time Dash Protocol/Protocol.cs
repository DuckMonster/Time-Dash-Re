using System.Collections.Generic;

public class Port
{
	public const int TCP = 1255, UDP = 1337;
}

public enum Protocol
{
	EnterMap,
	PlayerJoin,
	PlayerLeave,
	PlayerPosition,
	PlayerInput,

	PlayerDie
}