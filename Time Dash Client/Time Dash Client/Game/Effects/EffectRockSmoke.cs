using OpenTK;
using System;
using TKTools;
using TKTools.Mathematics;

public class EffectRockSmoke : Effect
{
	static Random rng = new Random();

	Vector2 position;
	Vector2 velocity;

	Timer effectTimer = new Timer(0.9f, false);
	Timer smokeTimer = new Timer(0.03f, true);

	bool ignoreCollision = true;

	float smokeSize = 1f;

	public EffectRockSmoke(Vector2 position, float direction, float size, Map m)
		: base(m)
	{
		this.position = position;
		position += velocity * Game.delta * 4;

		smokeSize = (float)rng.NextDouble() * 0.5f + 0.5f;
		smokeSize *= size;

		velocity = TKMath.GetAngleVector(direction) * 25 * smokeSize;

		effectTimer.Reset(smokeSize);
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
			Remove();

		base.Logic();

		if (ignoreCollision && !map.GetCollision(position, new Vector2(0.4f, 0.4f)))
			ignoreCollision = false;

		effectTimer.Logic();

		velocity.Y -= Stats.defaultStats.Gravity * 0.7f * Game.delta;
		velocity.X -= velocity.X * 0.8f * Game.delta;

		if (!ignoreCollision)
		{
			if (map.GetCollision(position + new Vector2(velocity.X, 0) * Game.delta, new Vector2(0.4f, 0.4f)))
				velocity.X *= -0.5f;
			if (map.GetCollision(position + new Vector2(0, velocity.Y) * Game.delta, new Vector2(0.4f, 0.4f)))
				velocity.Y *= -0.5f;
		}

		position += velocity * Game.delta;

		smokeTimer.Logic();
		if (smokeTimer.IsDone)
		{
			Vector2 pos = new Vector2(((float)rng.NextDouble() * 2) - 1, ((float)rng.NextDouble() * 2) - 1) * 0.1f;
			float dir = (float)rng.NextDouble() * 360f;
			float size = 1.5f * (1f - effectTimer.PercentageDone) + (float)rng.NextDouble() * 0.2f;
			float color = 1f - ((float)rng.NextDouble() * 0.1f);

			size *= smokeSize;
			pos *= size;

			map.AddEffect(new EffectSmoke(position + pos, size, 0.8f, dir, 0.4f, EffectSmoke.defaultColor * color, map));

			smokeTimer.Reset(MathHelper.Clamp((1f - velocity.Length / 10f), 0.03f, 1f) * size * 0.5f);
		}
	}

	public override void Draw()
	{
	}
}