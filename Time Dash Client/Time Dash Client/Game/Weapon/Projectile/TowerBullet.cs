using OpenTK;
using System;
using TKTools;
using TKTools.Mathematics;

public class TowerBullet : Projectile
{
	float angle;

	public TowerBullet(Actor owner, int id, Vector2 position, Vector2 target, Map map)
		:base(owner, id, position, map)
	{
		size = new Vector2(0.8f, 0.8f);
		velocity = (target - position).Normalized() * 60f;
		angle = TKMath.GetAngle(velocity);
	}

	public override void Logic()
	{
		if (!Active) return;

		//velocity += velocity * 10f * Game.delta;

		Vector2 stepVector = velocity * Game.delta;

		if (Map.GetCollision(this, stepVector))
		{
			Vector2 collidePos;
			Map.RayTraceCollision(position, position + stepVector, size, out collidePos);
			position = collidePos;

			Hit();
		}

		base.Logic();
	}

	public override void Hit()
	{
		Map.AddEffect(new EffectRing(position, 6f, 1.8f, Color.White, Map));
		EffectCone.CreateSmokeCone(position, TKMath.GetAngle(velocity), 1.4f, 1f, 12, 8, Map);

		Random rng = new Random();

		for (int i = 0; i < 2; i++)
		{
			float size = 0.5f + (float)rng.NextDouble() * 0.2f;
			float dir = ((float)rng.NextDouble() - 0.5f) * 45f;

			Map.AddEffect(new EffectRockSmoke(position, TKMath.GetAngle(velocity) + 180 + dir, size, Map));
		}

		base.Hit();
	}

	public override void Draw()
	{
		sprite.Draw(Position, Size * (1f + velocity.Length * 0.05f), angle);
	}
}