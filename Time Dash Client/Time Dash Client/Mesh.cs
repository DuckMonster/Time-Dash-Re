using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using TKTools;

public class Mesh : IDisposable
{
	public static int CALCULATIONS = 0, DRAW_CALLS = 0;

	#region Primitives

	public static Mesh Triangle
	{
		get
		{
			Mesh m = new Mesh();
			m.Vertices = new Vector2[] {
				new Vector2(-0.5f, -0.5f),
				new Vector2(0.5f, -0.5f),
				new Vector2(0f, 0.5f)
			};

			return m;
		}
	}

	public static Mesh Box
	{
		get
		{
			Mesh m = new Mesh();
			m.Vertices = new Vector2[] {
				new Vector2(-0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new Vector2(-0.5f, -0.5f),
				new Vector2(0.5f, -0.5f)
			};
			m.UV = new Vector2[] {
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f)
			};

			return m;
		}
	}

	#endregion

	VBO<Vector2> vertexBuffer, uvBuffer;
	List<Vector2> vertexList = new List<Vector2>(), uvList = new List<Vector2>();
	Matrix4 modelMatrix = Matrix4.Identity;
	ShaderProgram program = Game.defaultShader;

	Color color = Color.White;
	Texture texture;

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
		}
	}

	public Vector2[] Vertices
	{
		set
		{
			vertexList.Clear();
			vertexList.AddRange(value);

			vertexBuffer.UploadData(vertexList.ToArray());
		}
		get
		{
			return vertexList.ToArray();
		}
	}
	public Vector2[] UV
	{
		set
		{
			uvList.Clear();
			uvList.AddRange(value);

			uvBuffer.UploadData(uvList.ToArray());
		}
		get
		{
			return uvList.ToArray();
		}
	}
	public Texture Texture
	{
		set
		{
			texture = value;
		}
		get
		{
			return texture;
		}
	}

	public Mesh()
	{
		vertexBuffer = new VBO<Vector2>();
		uvBuffer = new VBO<Vector2>();
	}

	public void Dispose()
	{
		if (vertexBuffer != null) vertexBuffer.Dispose();
		if (uvBuffer != null) uvBuffer.Dispose();
	}

	public void Reset()
	{
		modelMatrix = Matrix4.Identity;
	}

	public void Translate(float x, float y) { Translate(new Vector2(x, y)); }
	public void Translate(Vector2 pos)
	{
		CALCULATIONS++;
		modelMatrix *= Matrix4.CreateTranslation(new Vector3(pos.X, pos.Y, 0));
	}

	public void Scale(float s) { Scale(new Vector2(s, s)); }
	public void Scale(Vector2 s)
	{
		CALCULATIONS++;
		modelMatrix *= Matrix4.CreateScale(new Vector3(s.X, s.Y, 1));
	}

	public void Rotate(float a)
	{
		CALCULATIONS++;
		modelMatrix *= Matrix4.CreateRotationZ(a);
	}

	public void Draw() { Draw(Vector2.Zero); }
	public void Draw(float x, float y) { Draw(new Vector2(x, y)); }
	public void Draw(Vector2 position)
	{
		DRAW_CALLS++;

		program.Use();

		program["vertexPosition"].SetValue(vertexBuffer);
		program["vertexUV"].SetValue(uvBuffer);
		program["model"].SetValue(modelMatrix);
		program["color"].SetValue(color);
		program["position"].SetValue(position);

		if (texture != null)
		{
			texture.Bind();
			program["usingTexture"].SetValue(true);
		}
		else
		{
			program["usingTexture"].SetValue(false);
		}

		GL.DrawArrays(PrimitiveType.TriangleStrip, 0, vertexList.Count);
	}
}