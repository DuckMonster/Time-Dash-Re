using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;

public class Bullet : Projectile
{
	Vector2 directionVector;
	Vector2 bulletSize;
	float direction;

	public Bullet(Player p, int id, Vector2 bsize, Vector2 target, Map m)
		: base(p, id, m)
	{
		directionVector = (target - position).Normalized();
		direction = TKMath.GetAngle(directionVector);

		size = new Vector2(0.1f, 0.1f);
		bulletSize = bsize;
	}

	public override void Logic()
	{
		if (!Active) return;

		Vector2 stepVector = directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		if (map.GetCollision(this, stepVector))
		{
			Vector2 collidePos;
			map.RayTraceCollision(position, position + stepVector, new Vector2(0.1f, 0.1f), out collidePos);
			position = collidePos;

			Hit();
		} else position += directionVector * Stats.defaultStats.BulletVelocity * Game.delta;
	}

	public override void Hit()
	{
		EffectCone.CreateSmokeCone(position, direction - 180, 0.4f, 4, 2, map);
		map.AddEffect(new EffectRing(position, 2f, 0.5f, Color.White, map));

		base.Hit();
	}

	public override void Draw()
	{
		if (!Active) return;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Rotate(direction);
		mesh.Scale(bulletSize);

		mesh.Draw();
	}
}