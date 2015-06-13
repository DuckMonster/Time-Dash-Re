using OpenTK;
using System;
using TKTools;
using TKTools.Mathematics;

public class Rifle : Weapon
{
	static Random rng = new Random();

	public Rifle(Player p, Map m)
		: base(WeaponList.Rifle, p, m)
	{

	}

	public override Projectile CreateProjectile(Vector2 target)
	{ 
		float dir = TKMath.GetAngle(owner.Position, target);
		dir = dir + (float)(rng.NextDouble() - 0.5) * 5f;

		target = owner.Position + TKMath.GetAngleVector(dir);

		return new Bullet(owner, owner.Position, target, damage, map);
	}
}