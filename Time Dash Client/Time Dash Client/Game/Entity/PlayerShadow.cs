using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using TKTools;

public class PlayerShadow : IDisposable
{
	public class ShadowPosition
	{
		public Vector2 position;
		public int tilex, tiley, direction;

		public ShadowPosition(Vector2 pos, Tileset t, int dir)
		{
			position = pos;
			tilex = t.X;
			tiley = t.Y;
			direction = dir;
		}
		public ShadowPosition(Vector2 pos, int tx, int ty, int dir)
		{
			position = pos;
			tilex = tx;
			tiley = ty;
			direction = dir;
		}

		public static implicit operator Vector2(ShadowPosition pos)
		{
			return pos.position;
		}
	}

	static Tileset shadowTileset = new Tileset(200, 160, "Res/jackShadowTileset.png");
	static Texture circleTexture = new Texture("Res/circlebig.png");

	Player player;
	Mesh mesh;
	Mesh circleMesh, arrowMesh;

	public float updateRate = 0.01f, updateTimer = 0f, bufferLength = 0.8f;

	ShadowPosition[] positionBuffer;
	int positionBufferIndex = 0;

	public ShadowPosition CurrentPosition
	{
		get
		{
			return positionBuffer[positionBufferIndex];
		}
	}

	public PlayerShadow(Player p, Mesh m)
	{
		player = p;
		mesh = new Mesh(PrimitiveType.Quads);
		mesh.Vertices = m.Vertices;
		mesh.UV = m.UV;

		positionBuffer = new ShadowPosition[(int)(bufferLength / updateRate)];
		for (int i = 0; i < positionBuffer.Length; i++)
			positionBuffer[i] = new ShadowPosition(p.position, p.playerTileset, p.dir);

		circleMesh = Mesh.Box;
		circleMesh.Texture = circleTexture;
		arrowMesh = new Mesh(PrimitiveType.TriangleStrip);

		arrowMesh.Vertices = new Vector2[] {
			new Vector2(-0.1f, 0f),
			new Vector2(0f, 0.5f),
			new Vector2(0f, -0.5f),
			new Vector2(1f, 0f)
		};
	}

	public void Dispose()
	{
		mesh.Dispose();
		arrowMesh.Dispose();
	}

	public void Logic()
	{
		updateTimer += Game.delta;

		while (updateTimer >= updateRate)
		{
			UpdateBuffer();
			updateTimer -= updateRate;
		}
	}

	public void UpdateBuffer()
	{
		positionBuffer[positionBufferIndex] = new ShadowPosition(player.position, player.playerTileset, player.dir);
		positionBufferIndex = (positionBufferIndex + 1) % positionBuffer.Length;
	}

	public void Draw()
	{
		Color c = player.Color;
		c.A = 0.8f;

		mesh.Color = c;
		mesh.FillColor = true;

		if (CurrentPosition != null)
		{
			mesh.Reset();

			mesh.Translate(CurrentPosition.position);
			mesh.Scale(player.size);
			mesh.Scale(-CurrentPosition.direction, 1);

			mesh.Draw(shadowTileset, CurrentPosition.tilex, CurrentPosition.tiley);
		}

		//DrawArrow();
	}

	public void DrawArrow()
	{
		ShadowPosition current = CurrentPosition;
		float direction = TKMath.GetAngle(player.position, current.position),
			distance = MathHelper.Clamp((player.position - current.position).Length, 0f, 6f) / 20f;

		Color c = player.Color;
		c.A = 0.4f;

		circleMesh.Color = c;
		arrowMesh.Color = c;

		#region Circle
		GL.Enable(EnableCap.StencilTest);

		GL.StencilMask(0xff);
		GL.Clear(ClearBufferMask.StencilBufferBit);

		GL.StencilFunc(StencilFunction.Never, 1, 0xff);
		GL.StencilOp(StencilOp.Replace, StencilOp.Keep, StencilOp.Keep);

		circleMesh.Reset();
		circleMesh.Translate(player.position);
		circleMesh.Rotate(direction);
		circleMesh.Translate(-distance, 0f);
		circleMesh.Scale(2.2f - distance * 1.5f);

		circleMesh.Draw();

		GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);
		GL.StencilOp(StencilOp.Incr, StencilOp.Keep, StencilOp.Incr);

		circleMesh.Reset();
		circleMesh.Translate(player.position);
		circleMesh.Scale(2f);

		circleMesh.Rotate(direction);

		circleMesh.Draw();

		#endregion
		distance = distance * 3f - 0.5f;

		GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);

		if (distance > 0)
		{
			arrowMesh.Reset();
			arrowMesh.Translate(player.position);
			arrowMesh.Rotate(direction);
			arrowMesh.Translate(0.9f, 0);
			arrowMesh.Scale(distance, 0.5f);

			arrowMesh.Draw();
		}

		GL.Disable(EnableCap.StencilTest);
	}
}