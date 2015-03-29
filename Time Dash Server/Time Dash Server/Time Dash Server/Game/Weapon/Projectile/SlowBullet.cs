using OpenTK;
using System.Collections.Generic;

public class SlowBullet : Projectile
{
	Vector2 directionVector;

	public SlowBullet(Actor owner, Vector2 position, Vector2 target, float damage, Map m)
		:base(owner, position, damage, m)
	{
		directionVector = (target - position).Normalized();
		velocity = directionVector * 5f;
	}

	public override void Logic()
	{
		if (!Active) return;

		Vector2 stepVector = velocity * Game.delta;

		if (Map.GetCollision(this, stepVector))
			Hit(Position + stepVector);

		List<SYPlayer> actorsHit = Map.GetActorRadius<SYPlayer>(Position, 0.2f, owner);

		if (actorsHit.Count > 0)
			Hit(actorsHit[0]);

		base.Logic();
	}
}