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

	public EffectSkull(Vector2 position, Map m)
		: base(m)
	{
		mesh = Mesh.Box;

		mesh.Texture = skullTexture;

		mesh.Reset();
		mesh.Translate(position);

		Random rng = new Random();
		rotation = (float)(rng.NextDouble() * 2 - 1) * 0.8f;

		mesh.Rotate(rotation);
		mesh.Scale(2f);
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

		effectTimer.Logic();
	}

	public override void Draw()
	{
		if (effectTimer.IsDone) return;

		mesh.Color = new Color(0.8f, 0.8f, 0.8f, 1 - effectTimer.PercentageDone);
		mesh.Draw();
	}
}