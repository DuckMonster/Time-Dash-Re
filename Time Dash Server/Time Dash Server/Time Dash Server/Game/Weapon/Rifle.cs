using OpenTK;

public class Rifle : Weapon
{
	public Rifle(Player p, Map m)
		: base(WeaponStats.Rifle, p, m)
	{

	}

	public override Projectile CreateProjectile(Vector2 target)
	{
		return new Bullet(owner, owner.Position, target, damage, map);
	}
}