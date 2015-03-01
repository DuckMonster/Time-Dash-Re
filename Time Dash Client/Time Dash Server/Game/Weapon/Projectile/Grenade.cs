using OpenTK;
using System.Collections.Generic;
using TKTools;
public class Grenade : Projectile
{
	public Grenade(Player owner, int id, float damage, Vector2 target, Map map)
		: base(owner, id, damage, map)
	{
		size = new Vector2(0.4f, 0.4f);
		velocity = (target - position).Normalized() * 30f;
	}

	public override void Logic()
	{
		if (!Active) return;

		velocity.Y -= Stats.defaultStats.Gravity * 1f * Game.delta;

		Vector2 stepVector = velocity * Game.delta;

		if (Map.GetCollision(position + stepVector, size))
		{
			Vector2 collidePos;
			Map.RayTraceCollision(position, position + stepVector, size, out collidePos);
			Hit(collidePos);
		}

		List<Player> playersHit = Map.GetPlayerRadius(position, 0.2f, owner);

		if (playersHit.Count > 0)
			Hit(playersHit[0]);

		base.Logic();
	}

	public override void Hit(Vector2 position)
	{
		base.Hit(position);
		HitArea(position);
	}

	public override void Hit(Player p)
	{
		base.Hit(p);
	}

	public void HitArea(Vector2 position)
	{
		List<Player> players = Map.GetPlayerRadius(position, 2f);

		foreach (Player p in players)
		{
			p.Hit(Damage, owner, TKMath.GetAngle(position, p.position));
		}
	}
}