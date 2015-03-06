using OpenTK;
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

	public Bullet(Player p, int id, float damage, Vector2 target, Map m)
		: base(p, id, damage, m)
	{
		directionVector = (target - p.position).Normalized();
		size = new Vector2(0.1f, 0.1f);
		velocity = directionVector * Stats.defaultStats.BulletVelocity;
	}

	public override void Logic()
	{
		if (!Active) return;

		Vector2 stepVector = directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		if (Map.GetCollision(this, stepVector))
			Hit(position + stepVector);

		List<Actor> actorsHit = Map.GetActorRadius<Actor>(position, 0.2f, owner);

		if (actorsHit.Count > 0)
			Hit(actorsHit[0]);

		base.Logic();
	}
}