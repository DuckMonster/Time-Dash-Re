﻿using OpenTK;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectPlayerHit : Effect
{
	Tileset tileset;
	Mesh mesh;

	Timer effectTimer = new Timer(0.4f, false);

	public EffectPlayerHit(Player p, float dir, float size, Map m)
		: base(m)
	{
		size = MathHelper.Clamp(size, 0.2f, 2f);

		tileset = new Tileset(p.playerTileset);

		mesh = new Mesh(p.sprite.Mesh.Vertices, p.sprite.Mesh.UV);
		mesh.Tileset = p.playerTileset;

		mesh.Reset();

		mesh.Translate(p.Position);
		mesh.Scale(p.Size);
		mesh.Scale(2f);

		mesh.Scale(p.dir, 1);

		mesh.FillColor = true;

		EffectCone.CreateBloodCone(p.Position, dir, 45f, (int)(20 * size), map);
		EffectCone.CreateBloodCone(p.Position, dir, 360f, (int)(10 * size), map);

		/*
		Random rng = new Random();
		Vector2 ringPos = new Vector2((float)rng.NextDouble() - 0.5f,
			(float)rng.NextDouble() - 0.5f);
		map.AddEffect(new EffectRing(p.position + ringPos, 2f + 2f * size, 0.7f, Color.White, map));
		*/ //NO RING PLZ

		Vector2 spikeDir = TKMath.GetAngleVector(dir) * 2 * size;

		map.AddEffect(new EffectSpike(p.Position - spikeDir, p.Position + spikeDir,
			2f * size, 0.4f, Color.White, map));
	}

	public override void Dispose()
	{
		base.Dispose();
		mesh.Dispose();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
			Remove();

		Color color = new Color(1, 1, 1, 1f - effectTimer.PercentageDone);
		mesh.Color = color;

		effectTimer.Logic();

		base.Logic();
	}

	public override void Draw()
	{
		mesh.Draw();
	}
}