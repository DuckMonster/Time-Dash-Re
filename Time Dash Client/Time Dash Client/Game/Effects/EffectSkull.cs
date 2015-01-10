using OpenTK;
using System;
using TKTools;

public class EffectSkull : Effect
{
	public static readonly Texture
		skullTexture = new Texture("Res/skull.png");

	Mesh skullMesh, spikeMesh;
	Vector2 position;
	float rotation;

	Color color;

	Timer effectTimer = new Timer(1f, false);

	public EffectSkull(Vector2 position, Color c, Map m)
		: base(m)
	{
		this.position = position;

		Random rng = new Random();
		rotation = (float)(rng.NextDouble() * 2 - 1) * 30f;

		color = c;

		skullMesh = Mesh.Box;
		spikeMesh = Mesh.Box;

		skullMesh.Texture = skullTexture;
		spikeMesh.Vertices = new Vector2[] {
			new Vector2(-0.5f, 0f),
			new Vector2(-0.3f, 0.5f),
			new Vector2(-0.3f, -0.5f),
			new Vector2(0.5f, 0f)
		};
	}

	public override void Dispose()
	{
		skullMesh.Dispose();
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

		color.A = 1 - effectTimer.PercentageDone;

		skullMesh.Color = color;
		spikeMesh.Color = color;

		float f = 1 - TKMath.Exp(effectTimer.PercentageDone, 5);

		for (int i = 0; i < 4; i++)
		{
			spikeMesh.Reset();
			spikeMesh.Translate(position);
			spikeMesh.Rotate(rotation + 90 * i);
			spikeMesh.Scale(1 + 2 * f, 2 - 2 * f);
			spikeMesh.Translate(f * 2, 0);
			spikeMesh.Draw();
		}

		float pos = (1 - TKMath.Exp(effectTimer.PercentageDone, 3)) * 3f,
			rot = 20 * effectTimer.PercentageDone;

		skullMesh.Reset();

		skullMesh.Translate(position + new Vector2(0, pos));
		skullMesh.Rotate(rotation + rot);
		skullMesh.Scale(3f);

		skullMesh.Draw();
	}
}