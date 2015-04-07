public class Port
{
	public const int TCP = 1255, UDP = 1255;
}

public enum Protocol
{
	RequestInfo,

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
	PlayerShoot,

	ProjectileExistance,

	PlayerHit,
	PlayerKill,
	PlayerDie,
	PlayerRespawn,
	PlayerDisable,

	PlayerEquipWeapon,
	PlayerReload,
	PlayerInventory,
	PlayerSwapWeapon,
	PlayerBuyWeapon,

	PlayerWin,
	TeamWin,

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

public enum Protocol_CTF
{
	TeamScore,
	FlagGrabbed,
	FlagDropped,
	FlagCaptured,
	FlagReturned,
	FlagPosition,
	RoundTimer
}

public enum Protocol_SY
{
	PlayerScrap,

	ScrapExistance,
	ScrapPosition,
	ScrapCollect,
	ScrapReturn,
	StashScrapAmount,

	CreepExistance,
	CreepPosition,
	CreepHit,
	CreepDie,
	CreepShoot,
	CreepCustom,

	TowerExistance,
	TowerTarget,
	TowerRotation,
	TowerShoot,
	TowerHit,
	TowerDie
}