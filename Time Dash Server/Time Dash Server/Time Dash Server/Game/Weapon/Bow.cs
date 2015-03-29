using OpenTK;

public class Bow : Weapon
{
	public Bow(Player owner, Map map)
		: base(WeaponStats.Bow, owner, map)
	{
	}

	public override Projectile CreateProjectile(Vector2 target)
	{
		Vector2 startPos = owner.Position + (target - owner.Position).Normalized() * 0.5f;
		return new Arrow(owner, startPos, target, damage, Charge, map);
	}
}