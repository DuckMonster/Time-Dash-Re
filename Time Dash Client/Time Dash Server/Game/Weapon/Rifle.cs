using OpenTK;

public class Rifle : Weapon
{
	public Rifle(Player p, Map m)
		: base(WeaponStats.Rifle, p, m)
	{

	}

	public override Projectile CreateBullet(Vector2 target, int index)
	{
		return new Bullet(owner, index, damage, target, map);
	}
}