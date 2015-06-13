using OpenTK;
using System.Collections.Generic;
using TKTools;
using TKTools.Mathematics;

public class Grenade : Projectile
{
	Actor hitActor = null;

	public Grenade(Actor owner, Vector2 position, Vector2 target, float damage, Map map)
		: base(owner, position, target, damage, map)
	{
		Size = new Vector2(0.4f, 0.4f);
		velocity = (target - position).Normalized() * 30f;
	}

	public override void Logic()
	{
		if (!Active) return;

		velocity.Y -= Stats.defaultStats.Gravity * 1f * Game.delta;

		Vector2 stepVector = velocity * Game.delta;

		if (Map.GetCollision(Position + stepVector, Size))
		{
			Vector2 collidePos;
			Map.RayTraceCollision(Position, Position + stepVector, Size, out collidePos);
			Hit(collidePos);
		}

		List<Actor> actorsHit = Map.GetActorRadius<Actor>(Position, 0.2f, owner);

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
			a.Hit(Damage / 2, TKMath.GetAngle(position, a.Position), owner);
	}
}