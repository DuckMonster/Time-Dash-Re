using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

public class EffectSpike : Effect
{
	Timer effectTimer;
	Mesh mesh;

	Vector2 origin, target;

	public EffectSpike(Vector2 pos1, Vector2 pos2, float width, float duration, Color c, Map m)
		: base(m)
	{
		origin = pos1;
		target = pos2;

		mesh = new Mesh(PrimitiveType.TriangleStrip);
		mesh.Vertices = new Vector2[] {
			new Vector2(-0.5f, 0f),
			new Vector2(-0.2f, 0.5f * width),
			new Vector2(-0.2f, -0.5f * width),
			new Vector2(0.5f, 0f)
		};

		mesh.Color = c;

		effectTimer = new Timer(duration, false);
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
		{
			Remove();
		}

		effectTimer.Logic();
	}

	public override void Draw()
	{
		mesh.Reset();

		Vector2 lenVector = target - origin;

		float w = TKMath.Exp(effectTimer.PercentageDone, 5);

		mesh.Translate(origin);
		mesh.Rotate(TKMath.GetAngle(lenVector));
		mesh.Scale(new Vector2(lenVector.Length, w));
		mesh.Translate(0.5f, 0);


		mesh.Draw();
	}
}