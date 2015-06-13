using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

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

		mesh = new Mesh(mesh.Vertices = new Vector3[] {
			new Vector3(-0.5f, -0.5f * startWidth, 0f),
			new Vector3(-0.5f, 0.5f * startWidth, 0f),
			new Vector3(0.5f, -0.5f * endWidth, 0f),
			new Vector3(0.5f, 0.5f * endWidth, 0f)
		});

		mesh.Color = c;
		mesh.Reset();

		Vector2 lenVector = target - origin;

		mesh.Translate(origin);
		mesh.RotateZ(TKMath.GetAngle(lenVector));
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

		mesh.Vertices2 = new Vector2[] {
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