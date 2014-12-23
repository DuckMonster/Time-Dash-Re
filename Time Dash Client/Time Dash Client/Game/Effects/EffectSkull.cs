using OpenTK;
using System;
using TKTools;

public class EffectSkull : Effect
{
	public static readonly Texture skullTexture = new Texture("Res/skull.png");

	Mesh mesh;
	Vector2 position;
	float rotation;

	Timer effectTimer = new Timer(1f, false);

	public EffectSkull(Vector2 position, Color c, Map m)
		: base(m)
	{
		mesh = Mesh.Box;

		mesh.Texture = skullTexture;

		mesh.Reset();
		mesh.Translate(position);

		Random rng = new Random();
		rotation = (float)(rng.NextDouble() * 2 - 1) * 30f;

		mesh.Rotate(rotation);
		mesh.Scale(2f);
		mesh.Color = c;
	}

	public override void Dispose()
	{
		mesh.Dispose();
		base.Dispose();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
		{
			Remove();
			return;
		}

		mesh.Translate(0, 1f * Game.delta);

		effectTimer.Logic();
	}

	public override void Draw()
	{
		if (effectTimer.IsDone) return;

		Color c = mesh.Color;
		c.A = 1 - effectTimer.PercentageDone;

		mesh.Color = c;
		mesh.Draw();
	}
}