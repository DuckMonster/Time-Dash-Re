using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

public class EffectLine : Effect
{
	Timer effectTimer;
	Mesh mesh;

	Vector2 origin, target;
	float startWidth, endWidth;

	public EffectLine(Vector2 pos1, Vector2 pos2, float startWidth, float endWidth, float duration, Color c, Map m)
		: base(m)
	{
		origin = pos1;
		target = pos2;

		this.startWidth = startWidth;
		this.endWidth = endWidth;

		mesh = new Mesh(PrimitiveType.TriangleStrip);
		mesh.Vertices = new Vector2[] {
			new Vector2(-0.5f, -0.5f * startWidth),
			new Vector2(-0.5f, 0.5f * startWidth),
			new Vector2(0.5f, -0.5f * endWidth),
			new Vector2(0.5f, 0.5f * endWidth)
		};

		mesh.Color = c;
		mesh.Reset();

		Vector2 lenVector = target - origin;

		mesh.Translate(origin);
		mesh.Rotate(TKMath.GetAngle(lenVector));
		mesh.Scale(new Vector2(lenVector.Length, 1));
		mesh.Translate(0.5f, 0);

		effectTimer = new Timer(duration, false);
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
		{
			Remove();
		}

		effectTimer.Logic();

		float w = TKMath.Exp(effectTimer.PercentageDone, 5);

		mesh.Vertices = new Vector2[] {
			new Vector2(-0.5f, -0.5f * startWidth * w),
			new Vector2(-0.5f, 0.5f * startWidth * w),
			new Vector2(0.5f, -0.5f * endWidth * w),
			new Vector2(0.5f, 0.5f * endWidth * w)
		};
	}

	public override void Draw()
	{
		mesh.Draw();
	}
}