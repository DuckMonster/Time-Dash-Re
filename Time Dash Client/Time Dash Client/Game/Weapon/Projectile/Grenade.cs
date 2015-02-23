using OpenTK;
using System;
using TKTools;
public class Grenade : Projectile
{
	Timer smokeTimer = new Timer(0.01f, true);

	public Grenade(Player owner, int id, Vector2 target, Map map)
		: base(owner, id, map)
	{
		size = new Vector2(0.4f, 0.4f);
		velocity = (target - position).Normalized() * 30f;
	}

	public override void Logic()
	{
		if (!Active) return;

		velocity.Y -= Stats.defaultStats.Gravity * 1f * Game.delta;

		Vector2 stepVector = velocity * Game.delta;

		if (map.GetCollision(this, stepVector))
		{
			Vector2 collidePos;
			map.RayTraceCollision(position, position + stepVector, size, out collidePos);
			position = collidePos;

			Hit();
		}

		smokeTimer.Logic();
		if (smokeTimer.IsDone)
		{
			Random rng = new Random();
			float dir = (float)rng.NextDouble() * 360f;
			Vector2 offset = new Vector2((float)rng.NextDouble() - 0.5f, (float)rng.NextDouble() - 0.5f) * 0.3f;

			map.AddEffect(new EffectSmoke(position + offset, 0.4f, 0.5f, dir, 0.4f, EffectSmoke.defaultColor, map));
			smokeTimer.Reset();
		}

		base.Logic();
	}

	public override void Hit()
	{
		map.AddEffect(new EffectRing(position, 6f, 1.2f, Color.White, map));
		EffectCone.CreateSmokeCone(position, TKMath.GetAngle(velocity), 1.2f, 4, 5, map);

		Random rng = new Random();

		for(int i=0; i<2; i++) {
			float size = 0.5f + (float)rng.NextDouble() * 0.3f;
			float dir = ((float)rng.NextDouble() - 0.5f) * 45f;

			map.AddEffect(new EffectRockSmoke(position, TKMath.GetAngle(velocity) + 180 + dir, size, map));
		}

		base.Hit();
	}

	public override void Draw()
	{
		if (!Active) return;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Rotate(TKMath.GetAngle(velocity));
		mesh.Scale(size);
		mesh.Scale(1f + velocity.Length / 20f, 1f - velocity.Length / 60f);

		mesh.Draw();
	}
}