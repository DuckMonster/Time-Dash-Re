using OpenTK;
using System.Collections.Generic;

public class Bullet : Entity
{
	Player owner;
	public int id;

	Vector2 directionVector;

	bool active = true;

	public Bullet(Player p, int id, Vector2 target, Map m)
		: base(p.position, m)
	{
		owner = p;
		directionVector = (target - p.position).Normalized();
		this.id = id;

		size = new Vector2(0.1f, 0.1f);
	}

	public override void Logic()
	{
		if (!active) return;

		base.Logic();

		Vector2 stepVector = directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		if (map.GetCollision(this, stepVector))
			Hit(null);
		else position += directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		List<Player> playersHit = map.GetPlayerRadius(position, 0.2f, owner);

		if (playersHit.Count > 0)
			Hit(playersHit[0]);
	}

	void Hit(Player p)
	{
		if (p != null)
		{
			p.Hit(owner, this);
		}

		active = false;
	}
}