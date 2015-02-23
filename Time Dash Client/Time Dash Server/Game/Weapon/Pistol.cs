using OpenTK;

public class Pistol : Weapon
{
	public Pistol(Player p, Map map)
		: base(WeaponStats.Pistol, p, map)
	{
	}

	public override Projectile CreateBullet(Vector2 target, int index)
	{
		return new Bullet(owner, index, damage, target, map);
	}
}