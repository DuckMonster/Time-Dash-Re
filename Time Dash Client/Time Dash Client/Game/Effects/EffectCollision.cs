using OpenTK;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

class EffectCollision : Effect
{
	Vector2 position;

	Mesh spikeMesh, lineMesh;
	Timer effectTimer = new Timer(0.5f, false);
	float rotation, lineRotation;

	public EffectCollision(Player a, Player b, Map m)
		: base(m)
	{
		position = a.Position + (b.Position - a.Position) * 0.5f;
		lineRotation = TKMath.GetAngle(a.Position, b.Position);
		rotation = lineRotation + 45;

		m.AddEffect(new EffectRing(position, 4f, 0.7f, Color.White, m));

		spikeMesh = new Mesh();
		lineMesh = new Mesh();

		spikeMesh.Vertices2 = new Vector2[] {
			new Vector2(-0.5f, 0f),
			new Vector2(-0.3f, 0.5f),
			new Vector2(-0.3f, -0.5f),
			new Vector2(0.5f, 0f)
		};

		lineMesh.Vertices2 = new Vector2[] {
			new Vector2(0, 0.5f),
			new Vector2(-0.4f, 0f),
			new Vector2(0.4f, 0f),
			new Vector2(0, -0.5f)
		};
	}

	public override void Dispose()
	{
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

		float f = 1-TKMath.Exp(effectTimer.PercentageDone, 5);

		for (int i = 0; i < 4; i++)
		{
			spikeMesh.Reset();
			spikeMesh.Translate(position);
			spikeMesh.RotateZ(rotation + 90 * i);
			spikeMesh.Scale(1 + 1 * f, 1 - f);
			spikeMesh.Translate(f * 2, 0);
			spikeMesh.Draw();
		}

		lineMesh.Reset();
		lineMesh.Translate(position);
		lineMesh.RotateZ(lineRotation);
		lineMesh.Scale(1 - f, 1 + 1.3f * f);
		lineMesh.Scale(4f);
		lineMesh.Draw();
	}
}