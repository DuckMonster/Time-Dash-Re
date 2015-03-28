using OpenTK;
public class Rifle : Weapon
{
	public Rifle(Player p, Map m)
		: base(WeaponStats.Rifle, p, m)
	{
	}

	public override Projectile CreateProjectile(Vector2 target, int index)
	{
		return new Bullet(owner, index, new Vector2(1, 0.05f), owner.Position, target, map);
	}
}