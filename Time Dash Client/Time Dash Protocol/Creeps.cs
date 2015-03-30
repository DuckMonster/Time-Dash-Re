public enum CreepType
{
	Flyer,
	FlyerBig 
}

public static class CreepStats
{
	public static readonly float FlyerHealth = 1.5f, FlyerChargeTime = 0.6f, FlyerIdleSpeed = 2f, FlyerChaseSpeed = 4f, FlyerImpactDamage = 1f, FlyerBulletDamage = 0.8f;
	public static readonly float BigFlyerHealth = 1.5f, BigFlyerChargeTime = 0.6f, BigFlyerIdleSpeed = 1.2f, BigFlyerChaseSpeed = 2.4f, BigFlyerImpactDamage = 2f, BigFlyerBulletDamage = 1f;
}