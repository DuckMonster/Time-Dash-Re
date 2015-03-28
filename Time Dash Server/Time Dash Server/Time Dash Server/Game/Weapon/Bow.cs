using OpenTK;

public class Bow : Weapon
{
	public Bow(Player owner, Map map)
		: base(WeaponStats.Bow, owner, map)
	{
	}

	public override Projectile CreateProjectile(Vector2 target)
	{
		return new Arrow(owner, owner.Position, target, damage, Charge, map);
	}
}