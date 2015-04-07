using OpenTK;
public class Pistol : Weapon
{
	public Pistol(Player p, Map map)
		:base(WeaponList.Pistol, p, map)
	{
	}

	public override Projectile CreateProjectile(Vector2 target, int index)
	{
		return new Bullet(owner, index, new Vector2(2, 0.5f), owner.Position, target, map);
	}
}