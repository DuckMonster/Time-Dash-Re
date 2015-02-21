using OpenTK;
using System;
using TKTools;

public class EffectPlayerHit : Effect
{
	Tileset tileset;
	int tilex, tiley;
	Mesh mesh;

	Timer effectTimer = new Timer(0.4f, false);

	public EffectPlayerHit(Player p, float dir, Map m)
		: base(m)
	{
		tilex = p.playerTileset.X;
		tiley = p.playerTileset.Y;

		tileset = p.playerTileset;

		mesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
		mesh.Vertices = p.mesh.Vertices;
		mesh.UV = p.mesh.UV;

		mesh.Reset();

		mesh.Translate(p.position);
		mesh.Scale(p.size);
		mesh.Scale(2f);

		mesh.Scale(p.dir, 1);

		mesh.FillColor = true;

		EffectCone.CreateBloodCone(p.position, dir, 45f, map);
		EffectCone.CreateBloodCone(p.position, dir, 360f, map);

		Random rng = new Random();

		Vector2 spikeDir = TKMath.GetAngleVector((float)rng.NextDouble() * 360f);

		map.AddEffect(new EffectSpike(p.position - spikeDir, p.position + spikeDir,
			1f, 0.4f, Color.Red, map));
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

		Color color = new Color(1f, 0, 0, 1f - effectTimer.PercentageDone);
		mesh.Color = color;

		effectTimer.Logic();

		base.Logic();
	}

	public override void Draw()
	{
		tileset.X = tilex;
		tileset.Y = tiley;
		mesh.Draw(tileset);
	}
}