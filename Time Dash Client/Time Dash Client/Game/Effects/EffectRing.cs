using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectRing : Effect
{
	Timer ringTimer;

	Vector2 position;
	Sprite sprite;

	float diameter = 4f, ringWidth = 0.9f;

	public EffectRing(Vector2 pos, float size, float dur, Color c, Map m)
		: base(m)
	{
		position = pos;
		diameter = size;
		ringWidth = diameter * 0.3f;
		ringTimer = new Timer(dur, false);

		sprite = new Sprite(Art.Load("Res/circlebig.png"));
		sprite.Color = c;
	}

	public override void Dispose()
	{
		sprite.Dispose();
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

		sprite.Draw(position, r2 * diameter, 0f);

		//RING
		GL.StencilMask(0x00);
		GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);

		sprite.Draw(position, r * diameter, 0f);

		GL.Disable(EnableCap.StencilTest);
	}
}