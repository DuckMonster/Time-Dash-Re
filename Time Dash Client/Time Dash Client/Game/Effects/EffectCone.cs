using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;
using TKTools.Mathematics;

public static class EffectCone
{	
	static Random rng = new Random();

	public static void CreateSmokeCone(Vector2 position, float direction, float smokesize, float time, int cloudnmbr, int debreenmbr, Map map)
	{
		//Splatters
		for (int i = 0; i < debreenmbr; i++)
		{
			Vector2 pos = new Vector2(
				((float)rng.NextDouble()) * 2f - 1f,
				((float)rng.NextDouble()) * 2f - 1f) * 0.4f * smokesize;

			float dir = ((float)rng.NextDouble() * 2f - 1f) * 45f + direction;
			float size = 1f - ((float)rng.NextDouble() * .5f);
			size *= smokesize;
			float color = 0.8f - ((float)rng.NextDouble() * 0.3f);
			float velo = 5f + (15f - size * 3f) * (float)rng.NextDouble() * smokesize;
			float t = 0.8f + (float)rng.NextDouble() * 0.5f;

			map.AddEffect(new EffectSmoke(position + pos, size, time * t, dir, velo, EffectSmoke.defaultColor * color, map));
		}

		//Cloud
		for (int i = 0; i < cloudnmbr; i++)
		{
			Vector2 pos = new Vector2(
				((float)rng.NextDouble()) * 2f - 1f,
				((float)rng.NextDouble()) * 2f - 1f) * 0.8f * smokesize;

			pos *= smokesize;

			float dir = (float)rng.NextDouble() * 360f;
			float size = 2f - ((float)rng.NextDouble() * 1.5f);
			size *= smokesize;
			float color = 0.8f - ((float)rng.NextDouble() * 0.3f);
			float velo = 0.2f + 0.6f * (float)rng.NextDouble();
			velo *= smokesize;
			float t = 0.8f + (float)rng.NextDouble() * 0.5f;

			map.AddEffect(new EffectSmoke(position + pos, size, time * t, dir, velo, EffectSmoke.defaultColor * color, map));
		}
	}

	public static void CreateBloodCone(Vector2 position, float direction, float spread, int nmbr, Map map)
	{
		//Splatters
		for (int i = 0; i < nmbr; i++)
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