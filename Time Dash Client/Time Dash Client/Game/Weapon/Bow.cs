public class Bow : Weapon
{
	public Bow(Player owner, Map map)
		: base(WeaponStats.Bow, owner, map)
	{
	}

	public override Projectile CreateProjectile(OpenTK.Vector2 target, int index)
	{
		return new Arrow(owner, index, target, Charge, map);
	}
}