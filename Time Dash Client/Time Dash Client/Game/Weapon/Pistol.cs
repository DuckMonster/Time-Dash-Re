using OpenTK;
public class Pistol : Weapon
{
	public Pistol(Player p, Map map)
		:base(WeaponFireType.Single, 0f, 0.8f, 6, p, map)
	{
	}

	public override Bullet CreateBullet(Vector2 target, int index)
	{
		return new Bullet(owner, index, new Vector2(2, 0.5f), target, map);
	}
}