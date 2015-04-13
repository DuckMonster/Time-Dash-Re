﻿using System;
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
			Mesh m = new Mesh(PrimitiveType.Quads);
			m.Vertices = new Vector2[] {
				new Vector2(-0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, -0.5f),
				new Vector2(-0.5f, -0.5f)
			};
			m.UV = new Vector2[] {
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f)
			};

			return m;
		}
	}

	public static Mesh OrthoBox
	{
		get
		{
			Mesh m = new Mesh(PrimitiveType.Quads);
			m.Vertices = new Vector2[] {
				new Vector2(-0.5f, 0.5f),
				new Vector2(0.5f, 0.5f),
				new Vector2(0.5f, -0.5f),
				new Vector2(-0.5f, -0.5f)
			};
			m.UV = new Vector2[] {
				new Vector2(0f, 0f),
				new Vector2(1f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f)
			};

			m.Orthographic = true;

			return m;
		}
	}

	#endregion

	PrimitiveType primitiveType;

	VBO<Vector3> vertexBuffer;
	VBO<Vector2> uvBuffer;
	List<Vector3> vertexList = new List<Vector3>();
	List<Vector2> uvList = new List<Vector2>();
	Matrix4 modelMatrix = Matrix4.Identity;

	Color color = Color.White;
	bool fillColor = false;
	bool usingBlur = false;
	float blurIntensity = 0f;
	int blurRadius = 4;
	Texture texture;

	bool ortho = false;

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
			Vector3[] vect3D = new Vector3[value.Length];
			for (int i = 0; i < vect3D.Length; i++)
				vect3D[i] = new Vector3(value[i].X, value[i].Y, 0);

			Vertices3D = vect3D;
		}
		get
		{
			Vector3[] vect3D = Vertices3D;
			Vector2[] vect2D = new Vector2[vect3D.Length];

			for (int i = 0; i < vect2D.Length; i++)
				vect2D[i] = vect3D[i].Xy;

			return vect2D;
		}
	}
	public Vector3[] Vertices3D
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
			return Orthographic ? Game.hudShader : Game.defaultShader;
		}
	}

	public bool Orthographic
	{
		get
		{
			return ortho;
		}
		set
		{
			ortho = value;
		}
	}

	public bool UsingBlur
	{
		get { return usingBlur; }
		set { usingBlur = value; }
	}

	public float BlurIntensity
	{
		get { return blurIntensity; }
		set { blurIntensity = value; }
	}

	public int BlurRadius
	{
		get { return blurRadius; }
		set { blurRadius = value; }
	}

	public Matrix4 ModelMaxtrix
	{
		get { return modelMatrix; }
		set { modelMatrix = value; }
	}

	public Mesh(PrimitiveType pt)
	{
		primitiveType = pt;

		vertexBuffer = new VBO<Vector3>();
		uvBuffer = new VBO<Vector2>();
	}
	public Mesh(Vector2[] vertices, PrimitiveType pt)
	{
		primitiveType = pt;

		vertexBuffer = new VBO<Vector3>();
		uvBuffer = new VBO<Vector2>();

		Vertices = vertices;
	}
	public Mesh(Vector3[] vertices, PrimitiveType pt)
	{
		primitiveType = pt;

		vertexBuffer = new VBO<Vector3>();
		uvBuffer = new VBO<Vector2>();

		Vertices3D = vertices;
	}
	public Mesh(Vector2[] vertices, Vector2[] uvs, PrimitiveType pt)
	{
		primitiveType = pt;

		vertexBuffer = new VBO<Vector3>();
		uvBuffer = new VBO<Vector2>();

		Vertices = vertices;
		UV = uvs;
	}
	public Mesh(Vector3[] vertices, Vector2[] uvs, PrimitiveType pt)
	{
		primitiveType = pt;

		vertexBuffer = new VBO<Vector3>();
		uvBuffer = new VBO<Vector2>();

		Vertices3D = vertices;
		UV = uvs;
	}
	public Mesh(TextDrawer td)
	{
		primitiveType = PrimitiveType.TriangleStrip;
		Vector2 size = td.CanvasSize;

		float w = size.X, h = size.Y;

		if (w >= h)
		{
			w = w / h;
			h = 1f;
		}
		else
		{
			h = h / w;
			w = 1f;
		}

		vertexBuffer = new VBO<Vector3>();
		Vertices = new Vector2[] {
			new Vector2(-w, h)/2,
			new Vector2(w, h)/2,
			new Vector2(-w, -h)/2,
			new Vector2(w, -h)/2
		};

		uvBuffer = new VBO<Vector2>();
		UV = new Vector2[] {
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};

		Texture = td;
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
	public void Translate(Vector2 pos, float z = 0) { Translate(new Vector3(pos.X, pos.Y, z)); }
	public void Translate(float x, float y, float z) { Translate(new Vector3(x, y, z)); }
	public void Translate(Vector3 pos)
	{
		CALCULATIONS++;
		modelMatrix = Matrix4.CreateTranslation(pos) * modelMatrix;
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
			Shader["textureWidth"].SetValue(texture.Width);
			Shader["textureHeight"].SetValue(texture.Height);

			Shader["usingBlur"].SetValue(UsingBlur);

			if (usingBlur)
			{
				Shader["blurRadius"].SetValue(BlurRadius);
				Shader["blurIntensity"].SetValue(BlurIntensity);
			}
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