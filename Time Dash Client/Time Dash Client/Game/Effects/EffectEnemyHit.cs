using OpenTK;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectEnemyHit : Effect
{
	Mesh mesh;

	Timer effectTimer = new Timer(0.4f, false);

	public EffectEnemyHit(SYCreep enemy, float dir, float size, Map m)
		: base(m)
	{
		size = MathHelper.Clamp(size, 0.2f, 2f);

		mesh = new Mesh(enemy.sprite.Mesh.Vertices, enemy.sprite.Mesh.UV);

		mesh.FillColor = true;
		mesh.Texture = enemy.sprite.Texture;

		mesh.ModelMatrix = enemy.sprite.Mesh.ModelMatrix;
		mesh.Scale(1.2f);

		/*
		Random rng = new Random();
		Vector2 ringPos = new Vector2((float)rng.NextDouble() - 0.5f,
			(float)rng.NextDouble() - 0.5f);
		map.AddEffect(new EffectRing(enemy.position + ringPos, 2f + 2f * size, 0.7f, Color.White, map));
		*/ //NO RING PLZ

		Vector2 spikeDir = TKMath.GetAngleVector(dir) * 2 * size;

		map.AddEffect(new EffectSpike(enemy.Position - spikeDir, enemy.Position + spikeDir,
			2f * size, 0.4f, Color.White, map));
		map.AddEffect(new EffectImpactQuad(enemy.Position, size, map));
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