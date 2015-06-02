using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;

public class Bullet : Projectile
{
	Vector2 directionVector;

	public float Direction
	{
		get
		{
			return TKMath.GetAngle(directionVector);
		}
	}

	public Bullet(Actor p, Vector2 position, Vector2 target, float damage, Map m)
		: base(p, position, target, damage, m)
	{
		directionVector = (target - position).Normalized();
		Size = new Vector2(0.1f, 0.1f);
		velocity = directionVector * Stats.defaultStats.BulletVelocity;
	}

	public override void Logic()
	{
		if (!Active) return;

		Vector2 stepVector = directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		if (Map.GetCollision(this, stepVector))
			Hit(Position + stepVector);

		List<Actor> actorsHit = Map.GetActorRadius<Actor>(Position, 0.2f, owner);

		if (actorsHit.Count > 0)
			Hit(actorsHit[0]);

		base.Logic();
	}
}