using OpenTK;
using System.Collections.Generic;
using TKTools;

public class Grenade : Projectile
{
	Actor hitActor = null;

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

		List<Actor> actorsHit = Map.GetActorRadius<Actor>(position, 0.2f, owner);

		if (actorsHit.Count > 0)
		{
			hitActor = actorsHit[0];
			Hit(actorsHit[0]);
		}

		base.Logic();
	}

	public override void Hit(Vector2 position)
	{
		base.Hit(position);
		HitArea(position);
	}

	public void HitArea(Vector2 position)
	{
		List<Actor> actors = Map.GetActorRadius<Actor>(position, 2f, hitActor);

		foreach (Actor a in actors)
			a.Hit(Damage / 2, TKMath.GetAngle(position, a.position), owner);
	}
}