﻿public class GrenadeLauncher : Weapon
{
	public GrenadeLauncher(Player owner, Map map)
		: base(WeaponList.GrenadeLauncher, owner, map)
	{

	}

	public override Projectile CreateProjectile(OpenTK.Vector2 target, int index)
	{
		return new Grenade(owner, index, owner.Position, target, map);
	}
}