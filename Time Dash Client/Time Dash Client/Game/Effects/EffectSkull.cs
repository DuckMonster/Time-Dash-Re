using OpenTK;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectSkull : Effect
{
	Sprite skullSprite;
	Mesh spikeMesh;

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

		skullSprite = new Sprite(Art.Load("Res/skull.png"));
		spikeMesh = new Mesh(new Vector3[] {
			new Vector3(-0.5f, 0f, 0f),
			new Vector3(-0.3f, 0.5f, 0f),
			new Vector3(0.5f, 0f, 0f),
			new Vector3(-0.3f, -0.5f, 0f)
		});
	}

	public override void Dispose()
	{
		skullSprite.Dispose();
		spikeMesh.Dispose();
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

		skullSprite.Color = color;
		spikeMesh.Color = color;

		float f = TKMath.Exp(effectTimer.PercentageDone);

		for (int i = 0; i < 4; i++)
		{
			spikeMesh.Reset();
			spikeMesh.Translate(position);
			spikeMesh.RotateZ(rotation + 90 * i);
			spikeMesh.Scale(1 + 2 * f, 2 - 2 * f);
			spikeMesh.Translate(f * 2, 0);
			spikeMesh.Draw();
		}

		float pos = TKMath.Exp(effectTimer.PercentageDone) * 3f,
			rot = 20 * effectTimer.PercentageDone;

		skullSprite.Draw(position + new Vector2(0, pos), 3f, rotation + rot);
	}
}