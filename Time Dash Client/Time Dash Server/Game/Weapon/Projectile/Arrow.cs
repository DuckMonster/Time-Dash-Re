using OpenTK;
using System.Collections.Generic;
using TKTools;

public class Arrow : Projectile
{
	public Arrow(Player owner, int id, float damage, Vector2 target, float charge, Map map)
		: base(owner, id, damage * charge, map)
	{
		size = new Vector2(0.2f, 0.2f);
		velocity = (target - position).Normalized() * (10 + 120f * charge);
	}

	public override void Logic()
	{
		if (!Active) return;

		velocity.Y -= Stats.defaultStats.Gravity * 1f * Game.delta;

		Vector2 stepVector = velocity * Game.delta;

		List<Player> players = Map.RayTracePlayer(position, position + stepVector, size, owner);
		if (players.Count > 0)
		{
			Hit(players[0]);
			return;
		}

		Vector2 collidePos;
		bool coll = Map.RayTraceCollision(position, position + stepVector, size, out collidePos);

		position = collidePos;

		if (coll)
		{
			Hit(position);
			return;
		}
	}

	public override void Hit(Vector2 position)
	{
		base.Hit(position);
	}
}