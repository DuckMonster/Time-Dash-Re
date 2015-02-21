using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;

public static class EffectCone
{	
	static Random rng = new Random();

	public static void CreateSmokeCone(Vector2 position, float direction, Map map)
	{
		//Splatters
		for (int i = 0; i < 4; i++)
		{
			Vector2 pos = new Vector2(
				((float)rng.NextDouble()) * 2f - 1f,
				((float)rng.NextDouble()) * 2f - 1f) * 0.4f;

			float dir = ((float)rng.NextDouble() * 2f - 1f) * 45f + direction;
			float size = 1f - ((float)rng.NextDouble() * .5f);
			float color = 0.8f - ((float)rng.NextDouble() * 0.3f);
			float velo = 5f + (15f - size * 3f) * (float)rng.NextDouble();

			map.AddEffect(new EffectSmoke(position + pos, size, 1.2f, dir, velo, EffectSmoke.defaultColor * color, map));
		}

		//Cloud
		for (int i = 0; i < 4; i++)
		{
			Vector2 pos = new Vector2(
				((float)rng.NextDouble()) * 2f - 1f,
				((float)rng.NextDouble()) * 2f - 1f) * 0.8f;

			float dir = (float)rng.NextDouble() * 360f;
			float size = 2f - ((float)rng.NextDouble() * 1.5f);
			float color = 0.8f - ((float)rng.NextDouble() * 0.3f);
			float velo = 0.2f + 0.6f * (float)rng.NextDouble();

			map.AddEffect(new EffectSmoke(position + pos, size, 1.2f, dir, velo, EffectSmoke.defaultColor * color, map));
		}
	}

	public static void CreateBloodCone(Vector2 position, float direction, float spread, Map map)
	{
		//Splatters
		for (int i = 0; i < 10; i++)
		{
			Vector2 pos = new Vector2(
				((float)rng.NextDouble()) * 2f - 1f,
				((float)rng.NextDouble()) * 2f - 1f) * 0.4f;

			float dir = ((float)rng.NextDouble() - .5f) * spread + direction;
			float size = 0.4f - ((float)rng.NextDouble() * .3f);
			float color = 0.8f - ((float)rng.NextDouble() * 0.3f);
			float velo = 10f + (30f - size * 3f) * (float)rng.NextDouble();

			map.AddEffect(new EffectBlood(position + pos, size, TKMath.GetAngleVector(dir) * velo, map));
		}
	}
}