using OpenTK;
public class Rifle : Weapon
{
	public Rifle(Player p, Map m)
		: base(WeaponFireType.Auto, 10, 2, 20, p, m)
	{
	}

	public override Bullet CreateBullet(Vector2 target, int index)
	{
		return new Bullet(owner, index, new Vector2(1, 0.05f), target, map);
	}
}