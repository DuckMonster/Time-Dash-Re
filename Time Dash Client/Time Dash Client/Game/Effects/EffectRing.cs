using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools;

public class EffectRing : Effect
{
	public static readonly Texture ringTexture = new Texture("Res/circlebig.png");
	Timer ringTimer;

	Vector2 position;
	Mesh mesh;

	float diameter = 4f, ringWidth = 0.9f;

	public EffectRing(Vector2 pos, float size, float dur, Map m)
		: base(m)
	{
		position = pos;
		diameter = size;
		ringWidth = diameter * 0.3f;
		ringTimer = new Timer(dur, false);

		mesh = Mesh.Box;
		mesh.Texture = ringTexture;
	}

	public override void Dispose()
	{
		mesh.Dispose();
		base.Dispose();
	}

	public override void Logic()
	{
		if (ringTimer.IsDone)
		{
			Remove();
			return;
		}

		ringTimer.Logic();
	}

	public override void Draw()
	{
		if (ringTimer.IsDone) return;

		float r = 1 - TKMath.Exp(ringTimer.PercentageDone, 8), r2 = 1 - TKMath.Exp(ringTimer.PercentageDone, 6);

		GL.Enable(EnableCap.StencilTest);


		//MASK
		GL.StencilMask(0xFF);
		GL.Clear(ClearBufferMask.StencilBufferBit);

		GL.StencilFunc(StencilFunction.Never, 1, 0xFF);
		GL.StencilOp(StencilOp.Replace, StencilOp.Replace, StencilOp.Replace);

		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(r2 * diameter);

		mesh.Color = Color.White;
		mesh.Draw();


		//RING
		GL.StencilMask(0x00);
		GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);

		mesh.Reset();
		mesh.Translate(position);
		mesh.Scale(r * diameter);

		mesh.Draw();


		GL.Disable(EnableCap.StencilTest);
	}
}