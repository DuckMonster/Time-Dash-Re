using OpenTK;
using System.Collections.Generic;
using TKTools;
using TKTools.Mathematics;

public class Arrow : Projectile
{
	List<Actor> hitActors = new List<Actor>();

	public Arrow(Actor owner, Vector2 position, Vector2 target, float damage, float charge, Map map)
		: base(owner, position, target, damage * charge, map)
	{
		Size = new Vector2(0.2f, 0.2f);
		velocity = (target - position).Normalized() * (10 + 120f * charge);
	}

	public override void Logic()
	{
		if (!Active) return;

		velocity.Y -= Stats.defaultStats.Gravity * 1f * Game.delta;

		Vector2 stepVector = velocity * Game.delta;

		List<Actor> actors = Map.RayTraceActor<Actor>(Position, Position + stepVector, Size, owner);
		foreach (Actor a in actors)
		{
			if (hitActors.Contains(a)) continue;

			Hit(a);
			hitActors.Add(a);
		}

		Vector2 collidePos;
		bool coll = Map.RayTraceCollision(Position, Position + stepVector, Size, out collidePos);

		Position = collidePos;

		if (coll)
		{
			Hit(Position);
			return;
		}
	}

	public override void Hit(Actor a)
	{
		a.Hit(Damage, TKMath.GetAngle(velocity), this);
	}

	public override void Hit(Vector2 position)
	{
		base.Hit(position);
	}
}