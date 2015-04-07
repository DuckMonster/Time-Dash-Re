using OpenTK;

public class GrenadeLauncher : Weapon
{
	public GrenadeLauncher(Player owner, Map map)
		: base(WeaponList.GrenadeLauncher, owner, map)
	{

	}

	public override Projectile CreateProjectile(Vector2 target)
	{
		return new Grenade(owner, owner.Position, target, damage, map);
	}
}