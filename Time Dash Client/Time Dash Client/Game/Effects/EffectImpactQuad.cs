using OpenTK;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectImpactQuad : Effect
{
	static Random rng = new Random();

	Timer effectTimer = new Timer(0.4f, false);

	Mesh spikeMesh;
	Vector2 position;
	float size;
	float rotation;

	public EffectImpactQuad(Vector2 position, float size, Map map)
		:base(map)
	{
		this.position = position;
		this.size = size;

		spikeMesh = new Mesh(new Vector3[] {
			new Vector3(0, 0f, 0f),
			new Vector3(0.3f, -0.4f, 0f),
			new Vector3(0.3f, 0.4f, 0f),
			new Vector3(1f, 0f, 0f)
		});

		rotation = 360f * (float)rng.NextDouble();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
		{
			Remove();
			return;
		}

		base.Logic();
		effectTimer.Logic();
	}

	public override void Draw()
	{
		float f = 1f - TKMath.Exp(effectTimer.PercentageDone, 5f);

		for(int i=0; i<4; i++)
		{
			spikeMesh.Reset();

			spikeMesh.Translate(position);
			spikeMesh.RotateZ(rotation + 90f * i);

			spikeMesh.Translate(-0.3f + 2 * f * size, 0);
			spikeMesh.Scale(size);
			spikeMesh.Scale(1.2f - f * 0.7f, 1f - f);

			spikeMesh.Draw();
		}
	}
}