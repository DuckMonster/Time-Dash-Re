using OpenTK;
using System;
using TKTools;

public class EffectExplosion
{
	public static void CreateExplosion(Vector2 position, float size, Map map)
	{
		Random rng = new Random();

		for (int i = 0; i < 3; i++)
		{
			float dir = (float)rng.NextDouble() * 360f;
			float s = (float)rng.NextDouble() * 0.1f + 0.4f;
			s *= size;

			map.AddEffect(new EffectRockSmoke(position, dir, s, map));
		}

		EffectCone.CreateSmokeCone(position, 360, size, 2f, 14, 10, map);
		map.AddEffect(new EffectRing(position, 7f * size, 1.2f * size, Color.White, map));
	}
}