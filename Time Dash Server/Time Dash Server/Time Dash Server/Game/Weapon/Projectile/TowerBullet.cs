using OpenTK;
using System.Collections.Generic;
using TKTools;

public class TowerBullet : Projectile
{
	Actor hitActor = null;

	public TowerBullet(Actor owner, Vector2 position, Vector2 target, float damage, Map map)
		:base(owner, position, damage, map)
	{
		Size = new Vector2(0.8f, 0.8f);
		velocity = (target - position).Normalized() * 60f;
	}

	public override void Logic()
	{
		if (!Active) return;

		//velocity += velocity * 10f * Game.delta;

		Vector2 stepVector = velocity * Game.delta;

		if (Map.GetCollision(Position + stepVector, Size))
		{
			Vector2 collidePos;
			Map.RayTraceCollision(Position, Position + stepVector, Size, out collidePos);
			Hit(collidePos);
		}

		List<Actor> actorsHit = Map.GetActorRadius<Actor>(Position, 0.5f, owner);

		if (actorsHit.Count > 0)
		{
			Hit(actorsHit[0]);
		}

		base.Logic();
	}

	public override void Hit(Actor a)
	{
		hitActor = a;

		a.Hit(Damage, TKMath.GetAngle(velocity), this, 50f);
		Hit(a.Position);
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
			a.Hit(Damage / 2, TKMath.GetAngle(position, a.Position), owner, 25f);
	}
}