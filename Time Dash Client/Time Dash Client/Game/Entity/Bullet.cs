using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;

public class Bullet : Entity
{
	Player owner;
	public int id;

	Vector2 directionVector;
	Vector2 bulletSize;
	float direction;

	bool active = true;

	public Bullet(Player p, int id, Vector2 bsize, Vector2 target, Map m)
		: base(p.position, m)
	{
		owner = p;
		this.id = id;
		directionVector = (target - position).Normalized();
		direction = TKMath.GetAngle(directionVector);

		size = new Vector2(0.1f, 0.1f);
		bulletSize = bsize;
	}

	public override void Logic()
	{
		if (!active) return;

		base.Logic();

		Vector2 stepVector = directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		if (map.GetCollision(this, stepVector))
		{
			Vector2 collidePos;
			map.RayTraceCollision(position, position + stepVector, new Vector2(0.1f, 0.1f), out collidePos);
			position = collidePos;

			Hit();
		} else position += directionVector * Stats.defaultStats.BulletVelocity * Game.delta;
	}

	public void Hit()
	{
		EffectCone.CreateSmokeCone(position, direction - 180, 0.4f, 4, 2, map);
		map.AddEffect(new EffectRing(position, 2f, 0.5f, Color.White, map));

		//Random rng = new Random();

		//if (rng.NextDouble() < 0.3)
		//{
		//	float dir = ((float)rng.NextDouble() - 0.5f) * 120f;
		//	map.AddEffect(new EffectRockSmoke(position, direction - 180 + dir, 0.4f, map));
		//}

		active = false;
	}

	public override void Draw()
	{
		if (!active) return;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Rotate(direction);
		mesh.Scale(bulletSize);

		mesh.Draw();
	}
}