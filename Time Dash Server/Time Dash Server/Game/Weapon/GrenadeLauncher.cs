public class GrenadeLauncher : Weapon
{
	public GrenadeLauncher(Player owner, Map map)
		: base(WeaponStats.GrenadeLauncher, owner, map)
	{

	}

	public override Projectile CreateProjectile(OpenTK.Vector2 target, int index)
	{
		return new Grenade(owner, index, damage, target, map);
	}
}