﻿using OpenTK;
using TKTools;
using TKTools.Mathematics;

public class Bullet : Projectile
{
	Vector2 directionVector;
	Vector2 bulletSize;
	float direction;

	public Bullet(Actor a, int id, Vector2 bsize, Vector2 position, Vector2 target, Map m)
		: base(a, id, position, m)
	{
		directionVector = (target - position).Normalized();
		direction = TKMath.GetAngle(directionVector);

		velocity = directionVector * Stats.defaultStats.BulletVelocity;

		size = new Vector2(0.1f, 0.1f);
		bulletSize = bsize;
	}

	public override void Logic()
	{
		if (!Active) return;

		Vector2 stepVector = velocity * Game.delta;

		if (Map.GetCollision(this, stepVector))
		{
			Vector2 collidePos;
			Map.RayTraceCollision(position, position + stepVector, new Vector2(0.1f, 0.1f), out collidePos);
			position = collidePos;

			Hit();
		}
		else position += stepVector;
    }

	public override void OnHit(Actor a, Vector2 hitpos)
	{
		hitActor = a;
		base.OnHit(a, hitpos);
	}

	Actor hitActor = null;

	public override void Hit()
	{
		EffectCone.CreateSmokeCone(position, direction - 180, 0.4f, 0.5f, 4, 2, Map);
		if (hitActor == null) Map.AddEffect(new EffectRing(position, 2f, 0.5f, Color.White, Map));

		base.Hit();
	}

	public override void Draw()
	{
		if (!Active) return;

		sprite.Draw(position, bulletSize, direction);
	}
}