using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;

public class EffectSmokeCone : Effect
{
	Timer effectTimer = new Timer(4f, false);

	public EffectSmokeCone(Vector2 position, float direction, Map m)
		: base(m)
	{
		Random rng = new Random();

		for (int i = 0; i < 4; i++)
		{
			Vector2 pos = new Vector2(
				((float)rng.NextDouble()) * 2f - 1f,
				((float)rng.NextDouble()) * 2f - 1f) * 0.4f;

			float dir = ((float)rng.NextDouble() * 2f - 1f) * 45f + direction;
			float size = 2f - ((float)rng.NextDouble() * 1.5f);
			float color = 0.8f - ((float)rng.NextDouble() * 0.3f);

			map.AddEffect(new EffectSmoke(position + pos, size, 1.2f, dir, (20f - size * 4f), EffectSmoke.defaultColor * color, m));
		}

		for (int i = 0; i < 4; i++)
		{
			Vector2 pos = new Vector2(
				((float)rng.NextDouble()) * 2f - 1f,
				((float)rng.NextDouble()) * 2f - 1f) * 0.8f;

			float dir = (float)rng.NextDouble() * 360f;
			float size = 2f - ((float)rng.NextDouble() * 1.5f);
			float color = 0.8f - ((float)rng.NextDouble() * 0.3f);

			map.AddEffect(new EffectSmoke(position + pos, size, 1.2f, dir, 0.4f, EffectSmoke.defaultColor * color, m));
		}
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
			Remove();

		effectTimer.Logic();
	}

	public override void Draw()
	{
	}
}