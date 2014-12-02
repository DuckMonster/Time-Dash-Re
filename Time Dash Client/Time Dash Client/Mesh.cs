using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

class Mesh
{
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

			return m;
		}
	}

	#endregion

	VBO<Vector2> vertexBuffer;
	List<Vector2> vertexList = new List<Vector2>();
	Matrix4 modelMatrix = Matrix4.Identity;
	ShaderProgram program = Program.program;

	Color color = Color.White;
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

	public Mesh()
	{
		program.Use();
		vertexBuffer = new VBO<Vector2>();
	}

	public void Reset()
	{
		modelMatrix = Matrix4.Identity;
	}

	public void Translate(float x, float y) { Translate(new Vector2(x, y)); }
	public void Translate(Vector2 pos)
	{
		modelMatrix *= Matrix4.CreateTranslation(new Vector3(pos.X, pos.Y, 0));
	}

	public void Scale(float s) { Scale(new Vector2(s, s)); }
	public void Scale(Vector2 s)
	{
		modelMatrix *= Matrix4.CreateScale(new Vector3(s.X, s.Y, 1));
	}

	public void Rotate(float a)
	{
		modelMatrix *= Matrix4.CreateRotationZ(a);
	}

	public void Draw()
	{
		program.Use();

		program["vertexPosition"].SetValue(vertexBuffer);
		//program["model"].SetValue(modelMatrix);
		program["color"].SetValue(color);

		GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
	}
}