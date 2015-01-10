using System.Collections.Generic;

public class Port
{
	public const int TCP = 1255, UDP = 1255;
}

public enum Protocol
{
	EnterMap,
	PlayerName,
	PlayerJoin,
	PlayerLeave,
	PlayerPosition,
	PlayerLand,
	PlayerInput,
	PlayerInputPure,
	PlayerJump,

	CreateTeam,
	PlayerJoinTeam,

	PlayerDodge,
	PlayerDash,
	PlayerDodgeCollision,
	PlayerDashCollision,

	PlayerKill,
	PlayerDie,
	PlayerRespawn,
	PlayerDisable,

	PlayerWin,

	MapArgument,

	ServerPosition
}

public enum Protocol_KOTH
{
	CurrentHolder,
	PlayerScore
}

public enum Protocol_DM
{
	PlayerScore
}

public enum Protocol_CP
{
	TeamOwner,
	TeamProgress
}