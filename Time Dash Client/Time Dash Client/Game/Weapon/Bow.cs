using OpenTK;

public class Bow : Weapon
{
	public Bow(Player owner, Map map)
		: base(WeaponList.Bow, owner, map)
	{
	}

	public override Projectile CreateProjectile(Vector2 target, int index)
	{
		Vector2 startPos = owner.Position + (target - owner.Position).Normalized() * 0.5f;
		return new Arrow(owner, index, startPos, target, Charge, map);
	}
}