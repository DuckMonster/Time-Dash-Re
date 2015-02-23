using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using TKTools;

public class CircleBar : IDisposable
{
	static Texture circleTexture = new Texture("Res/circlebig.png");

	Mesh stencilMesh, circleMesh;

	float progress = 1f;
	float size, barsize;

	float startAngle, fillAngle;

	public float Progress
	{
		get
		{
			return progress;
		}
		set
		{
			float p = MathHelper.Clamp(value, 0, 1);
			progress = p;
		}
	}

	public float Size
	{
		get
		{
			return size;
		}
		set
		{
			size = value;
		}
	}

	public CircleBar(float size, float barsize, float startAngle, float fillAngle)
	{
		this.size = size;
		this.barsize = barsize;

		circleMesh = Mesh.Box;
		circleMesh.Texture = circleTexture;

		this.startAngle = startAngle;
		this.fillAngle = fillAngle;

		stencilMesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleFan);
	}

	public void Dispose()
	{
		stencilMesh.Dispose();
		circleMesh.Dispose();
	}

	public void Logic()
	{
		List<Vector2> vectorList = new List<Vector2>(10);
		float angle = progress * fillAngle,
			part = fillAngle / 8;

		vectorList.Add(new Vector2(0, 0));
		vectorList.Add(TKMath.GetAngleVector(startAngle));

		for (int i = 0; i < 9; i++)
		{
			if (Math.Abs(angle) > Math.Abs(part * i))
				vectorList.Add(TKMath.PolarPointVector(part * i + startAngle, 4));
			else
			{
				vectorList.Add(TKMath.PolarPointVector(angle + startAngle, 4));
				break;
			}
		}

		stencilMesh.Vertices = vectorList.ToArray();
	}

	public void Draw(Vector2 position, Color c)
	{
		circleMesh.Color = c;

		GL.Enable(EnableCap.StencilTest);

		GL.StencilMask(0xff);
		GL.Clear(ClearBufferMask.StencilBufferBit);

		GL.StencilFunc(StencilFunction.Never, 1, 0xff);
		GL.StencilOp(StencilOp.Replace, StencilOp.Keep, StencilOp.Keep);

		stencilMesh.Reset();

		stencilMesh.Translate(position);
		stencilMesh.Scale(size);

		stencilMesh.Draw();

		GL.StencilOp(StencilOp.Decr, StencilOp.Keep, StencilOp.Keep);

		circleMesh.Reset();

		circleMesh.Translate(position);
		circleMesh.Scale(size - barsize);

		circleMesh.Draw();

		GL.StencilFunc(StencilFunction.Equal, 1, 0xff);

		circleMesh.Reset();

		circleMesh.Translate(position);
		circleMesh.Scale(size);

		circleMesh.Draw();

		GL.Disable(EnableCap.StencilTest);
	}
}