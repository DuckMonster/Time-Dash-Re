using System;
using System.Drawing;
using Tao.FreeGlut;
using OpenGL;

class Mesh : IDisposable
{
	public static int DRAW_CALLS = 0;
	public static int CALCULATIONS = 0;

	VBO<Vector2> vertexPosition;
	VBO<Vector2> vertexUV;

	Color color = Color.White;
	Texture texture;

	Matrix4 matrix = Matrix4.Identity;

	ShaderProgram program = Game.defaultShader;
	BeginMode beginMode;

	#region Primitives
	public static Mesh Box
	{
		get
		{
			return new Mesh(
			new Vector2[] {
				new Vector2(-0.5, 0.5),
				new Vector2(0.5, 0.5),
				new Vector2(-0.5, -0.5),
				new Vector2(0.5, -0.5)
			},
			new Vector2[] {
				new Vector2(0, 1),
				new Vector2(1, 1),
				new Vector2(0, 0),
				new Vector2(1, 0)
			},
			BeginMode.TriangleStrip
			);
		}
	}
	#endregion

	public Mesh(Vector2[] vPos, Vector2[] vUV, BeginMode bm = BeginMode.Triangles)
	{
		Garbage.Add(this);

		SetVertexPositions(vPos);
		SetVertexUVs(vUV);
		SetBeginMode(bm);
	}

	public void Dispose()
	{
		if (vertexPosition != null) vertexPosition.Dispose();
		if (vertexUV != null) vertexUV.Dispose();
		if (texture != null) texture.Dispose();
	}

	public void SetVertexPositions(Vector2[] vertices)
	{
		if (vertexPosition != null) vertexPosition.Dispose();
		vertexPosition = new VBO<Vector2>(vertices);
	}

	public void SetVertexUVs(Vector2[] vertices)
	{
		if (vertexUV != null) vertexUV.Dispose();
		vertexUV = new VBO<Vector2>(vertices);
	}

	public void SetColor(Color color)
	{
		this.color = color;
	}

	public void SetTexture(Texture tex)
	{
		if (texture != null) texture.Dispose();
		texture = tex;
	}

	public void SetBeginMode(BeginMode bm)
	{
		beginMode = bm;
	}

	public void Reset()
	{
		matrix = Matrix4.Identity;
	}

	public void Translate(float x, float y) { Translate(new Vector2(x, y)); }
	public void Translate(Vector2 pos) 
	{
		CALCULATIONS++;
		matrix = Matrix4.CreateTranslation(new Vector3(pos.x, pos.y, 0)) * matrix;
	}

	public void Scale(float s) { Scale(new Vector2(s, s)); }
	public void Scale(float x, float y) { Scale(new Vector2(x, y)); }
	public void Scale(Vector2 pos)
	{
		CALCULATIONS++;
		matrix = Matrix4.CreateScaling(new Vector3(pos.x, pos.y, 1)) * matrix;
	}

	public void Rotate(float angle)
	{
		CALCULATIONS++;
		matrix = Matrix4.CreateRotationZ((float)(Math.PI / 180.0) * angle) * matrix;
	}

	public void PrepareHandles()
	{
		Gl.BindBufferToShaderAttribute(vertexPosition, program, "vertexPosition");
		Gl.BindBufferToShaderAttribute(vertexUV, program, "vertexUV");
	}

	public void Draw()
	{
		DRAW_CALLS++;

		program.Use();

		PrepareHandles();

		program["u_color"].SetValue(new Vector4(color.R / 255.0, color.G / 255.0, color.B / 255.0, color.A / 255.0));
		program["model_matrix"].SetValue(matrix);

		if (texture != null)
		{
			program["texture"].SetValue(0);
			program["textureEnabled"].SetValue(true);
			Gl.BindTexture(texture);
		}
		else
		{
			program["textureEnabled"].SetValue(false);
		}

		Gl.DrawArrays(beginMode, 0, vertexPosition.Count);
	}
}