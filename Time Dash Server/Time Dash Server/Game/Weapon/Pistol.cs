using OpenTK;

public class Pistol : Weapon
{
	public Pistol(Player p, Map map)
		: base(WeaponList.Pistol, p, map)
	{
	}

	public override Projectile CreateProjectile(Vector2 target)
	{
		return new Bullet(owner, owner.Position, target, damage, map);
	}
}