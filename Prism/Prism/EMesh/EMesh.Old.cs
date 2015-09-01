using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections;
using SYS = System.Drawing;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class EMesh : IEnumerable, IDisposable
{
	public static ShaderProgram MeshProgram;
	public static void CompileMeshProgram()
	{
		string vertex = ShaderProgram.ReadFile("EMesh/meshProgramVertex.cpp");
		string fragment = ShaderProgram.ReadFile("EMesh/meshProgramFragment.cpp");

		MeshProgram = new ShaderProgram(vertex, fragment);
	}

	Editor editor;
	Layer layer;

	public Layer Layer
	{
		get { return layer; }
		set { SetLayer(value); }
	}

	EVertex[] vertices = new EVertex[4];
	Mesh mesh;
	Model previewModel;
	Polygon polygon;

	TextureSet.Tile tile;

	KeyboardWatch keyboard = Editor.keyboard;

	bool dirty = true;

	public EVertex[] Vertices
	{
		get { return vertices; }
	}

	public TextureSet.Tile Tile
	{
		get { return tile; }
		set
		{
			tile = value;
			previewModel.Texture = tile.Texture;
			SetDirty();
		}
	}

	public bool Enabled
	{
		get
		{
			if (layer == null || !Visible) return false;
			return layer.Enabled;
		}
	}

	public bool Visible
	{
		get { return layer.Visible; }
	}

	public Vector3[] VertexPosition
	{
		get
		{
			Vector3[] p = new Vector3[vertices.Length];
			for (int i = 0; i < p.Length; i++)
				p[i] = new Vector3(vertices[i].Position);

			return p;
		}
	}
	public Vector2[] VertexUV
	{
		get
		{
			if (tile == null)
				return new Vector2[] { Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };

			SYS.RectangleF rect = tile.UV;

			Vector2 p = new Vector2(rect.X, rect.Y), px = new Vector2(rect.Width, 0), py = new Vector2(0, rect.Height);
			return new Vector2[] { p, p + px, p + px + py, p + py };
		}
	}

	public bool Hovered
	{
		get
		{
			if (editor.DisableSelect) return false;
			foreach (EVertex v in vertices)
				if (v.Hovered) return false;

			return editor.HoveredMeshes.Contains(this);
		}
	}

	public float Alpha
	{
		get
		{
			float s = Selected ? 0.2f : 0f;

			if (Hovered)
			{
				if (Editor.mouse[MouseButton.Left]) return s + 0.5f;
				else return s + 0.2f;
			}
			else return s;
		}
	}

	public bool Selected
	{
		get
		{
			return Array.TrueForAll(vertices, (x) => x.Selected);
		}
		set
		{
			if (value)
				editor.Select(new EMesh[] { this });
			else
				editor.Deselect(new EMesh[] { this });
		}
	}

	public EMesh(Layer l, Editor e)
	{
		editor = e;
		layer = l;

		mesh = new Mesh(MeshProgram);
		mesh.GetAttribute<Vector3>("vertexPosition").Data = new Vector3[]
		{
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f),
		};

		previewModel = new Model();
		polygon = new Polygon();

		vertices[0] = new EVertex(editor, this, mesh.Vertices[0], new Vector2(-0.5f, -0.5f));
		vertices[1] = new EVertex(editor, this, mesh.Vertices[1], new Vector2(0.5f, -0.5f));
		vertices[2] = new EVertex(editor, this, mesh.Vertices[2], new Vector2(0.5f, 0.5f));
		vertices[3] = new EVertex(editor, this, mesh.Vertices[3], new Vector2(-0.5f, 0.5f));

		vertices[0].Color = new Vector3(0f, 0f, 0f);
		vertices[1].Color = new Vector3(0f, 0f, 0f);
		vertices[2].Color = new Vector3(1f, 1f, 1f);
		vertices[3].Color = new Vector3(1f, 1f, 1f);
	}
	public EMesh(TextureSet.Tile t, Layer l, Editor e)
		: this(l, e)
	{
		if (t != null)
		{
			Tile = t;
			previewModel.Texture = t.Texture;
		}
	}
	public EMesh(SYS.RectangleF rectangle, Layer l, Editor e)
		: this(l, e)
	{
		SetVertexPosition(rectangle);
	}
	public EMesh(SYS.RectangleF rectangle, TextureSet.Tile t, Layer l, Editor e)
		: this(t, l, e)
	{
		SetVertexPosition(rectangle);
	}
	public EMesh(EMesh copy, Layer l, Editor e)
		:this(copy.tile, l, e)
	{
		for(int i=0; i<vertices.Length; i++)
			vertices[i].Position = copy.vertices[i].Position;
	}
	public EMesh(Prism.Parser.PrismMesh prismMesh, Layer l, Editor e)
		:this(l, e)
	{
		for (int i = 0; i < prismMesh.VertexPosition.Length; i++)
			vertices[i].Position = prismMesh.VertexPosition[i];
	}

	public void Dispose()
	{
		previewModel.Dispose();
	}

	public void SetVertexPosition(SYS.RectangleF rect)
	{
		vertices[0].Position = new Vector2(rect.X, rect.Y);
		vertices[1].Position = new Vector2(rect.X + rect.Width, rect.Y);
		vertices[2].Position = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
		vertices[3].Position = new Vector2(rect.X, rect.Y + rect.Height);

		SetDirty();
	}

	public void Remove()
	{
		editor.RemoveMesh(this);
	}

	public void SetLayer(Layer l)
	{
		layer = l;
	}

	public void SetDirty()
	{
		dirty = true;
	}

	public bool Intersects(Polygon p)
	{
		if (!Enabled) return false;
		return polygon.Intersects(p);
	}

	public void UpdateMesh()
	{
		polygon.pointList.Clear();
		polygon.pointList.AddRange(VertexPosition);

		previewModel.VertexPosition = VertexPosition;
		previewModel.VertexUV = VertexUV;

		mesh.GetAttribute<Vector2>("vertexUV").Data = VertexUV;

		dirty = false;
	}

	public void Logic()
	{
		/*
		if (keyboard.KeyPressed(Key.Minus) && Selected)
			layer.MoveMesh(this, 1);
		if (keyboard.KeyPressed(Key.Slash) && Selected)
			layer.MoveMesh(this, -1);*/
	}

	public void Draw()
	{
		if (dirty) UpdateMesh();
		if (!Visible || previewModel.Texture == null) return;

		if (tile != null)
		{
			tile.Texture.Bind();
			mesh.Draw();
		}
	}

	public void DrawUI()
	{
		if (dirty) UpdateMesh();

		if (!Visible) return;

		if (Enabled)
		{
			previewModel.TextureEnabled = false;

			if (OptionsForm.options.MeshBorders)
			{
				previewModel.Color = (Selected ? Color.Yellow : Color.White) * new Color(1f, 1f, 1f, OptionsForm.options.MeshBorderOpacity);
				previewModel.Draw(PrimitiveType.LineLoop);
			}

			previewModel.Color = new Color(1f, 1f, 1f, Alpha);
			previewModel.Draw();
		}

		if (editor.SelectMode == SelectMode.Vertices)
			foreach (EVertex v in vertices)
				v.Draw();
	}

	public IEnumerator GetEnumerator()
	{
		foreach (EVertex v in vertices)
			yield return v;
	}
}