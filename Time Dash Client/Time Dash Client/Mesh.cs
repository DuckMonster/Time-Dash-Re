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
			Mesh m = new Mesh(PrimitiveType.TriangleStrip);
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
			Mesh m = new Mesh(PrimitiveType.TriangleStrip);
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

	PrimitiveType primitiveType;

	VBO<Vector2> vertexBuffer, uvBuffer;
	List<Vector2> vertexList = new List<Vector2>(), uvList = new List<Vector2>();
	Matrix4 modelMatrix = Matrix4.Identity;

	Color color = Color.White;
	bool fillColor = false;
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

	public bool FillColor
	{
		get
		{
			return fillColor;
		}
		set
		{
			fillColor = value;
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

	public ShaderProgram Shader
	{
		get
		{
			return Map.defaultShader;
		}
	}

	public Mesh(PrimitiveType pt)
	{
		primitiveType = pt;

		vertexBuffer = new VBO<Vector2>();
		uvBuffer = new VBO<Vector2>();
	}
	public Mesh(Vector2[] vertices, PrimitiveType pt)
	{
		primitiveType = pt;

		vertexBuffer = new VBO<Vector2>();
		uvBuffer = new VBO<Vector2>();

		Vertices = vertices;
	}
	public Mesh(Vector2[] vertices, Vector2[] uvs, PrimitiveType pt)
	{
		primitiveType = pt;

		vertexBuffer = new VBO<Vector2>();
		uvBuffer = new VBO<Vector2>();

		Vertices = vertices;
		UV = uvs;
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
		modelMatrix = Matrix4.CreateTranslation(new Vector3(pos.X, pos.Y, 0)) * modelMatrix;
	}

	public void Scale(float s) { Scale(new Vector2(s, s)); }
	public void Scale(float x, float y) { Scale(new Vector2(x, y)); }
	public void Scale(Vector2 s)
	{
		CALCULATIONS++;
		modelMatrix = Matrix4.CreateScale(new Vector3(s.X, s.Y, 1)) * modelMatrix;
	}

	public void Rotate(float a)
	{
		CALCULATIONS++;

		a = TKTools.TKMath.ToRadians(a);
		modelMatrix = Matrix4.CreateRotationZ(a) * modelMatrix;
	}

	public void Draw()
	{
		DRAW_CALLS++;

		Shader.Use();

		Shader["vertexPosition"].SetValue(vertexBuffer);
		Shader["vertexUV"].SetValue(uvBuffer);
		Shader["model"].SetValue(modelMatrix);
		Shader["color"].SetValue(color);
		Shader["fillColor"].SetValue(fillColor);

		if (texture != null)
		{
			texture.Bind();
			Shader["usingTexture"].SetValue(true);
		}
		else
		{
			Shader["usingTexture"].SetValue(false);
		}

		GL.DrawArrays(primitiveType, 0, vertexList.Count);
	}

	public void Draw(Tileset ts) { Draw(ts, ts.X, ts.Y); }
	public void Draw(Tileset ts, int x, int y)
	{
		DRAW_CALLS++;

		ShaderProgram shad = Tileset.tileProgram;

		shad["vertexPosition"].SetValue(vertexBuffer);
		shad["vertexUV"].SetValue(uvBuffer);
		shad["model"].SetValue(modelMatrix);
		shad["color"].SetValue(color);
		shad["fillColor"].SetValue(fillColor);

		ts.Upload(x, y);

		GL.DrawArrays(primitiveType, 0, vertexList.Count);
	}
}