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
	class VertexEnum : IEnumerator
	{
		EVertex[] vertices;
		int index = -1;

		public VertexEnum(EMesh e)
		{
			vertices = e.vertices;
		}

		public bool MoveNext()
		{
			index++;
			return (index < vertices.Length);
		}

		public void Reset()
		{
			index = -1;
		}

		public object Current
		{
			get { return vertices[index]; }
		}
	}

	Editor editor;
	Layer layer;

	public Layer Layer
	{
		get { return layer; }
	}

	EVertex[] vertices = new EVertex[4];
	Mesh mesh;
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
			mesh.Texture = tile.Texture;
			SetDirty();
		}
	}

	public bool Enabled
	{
		get
		{
			if (layer == null || !Visible) return false;
			return layer.Active;
		}
	}

	public bool Visible
	{
		get { return layer.Visible; }
	}

	public Vector3[] VertexPositions
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
			return Array.TrueForAll<EVertex>(vertices, (x) => x.Selected);
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
		mesh = new Mesh();
		polygon = new Polygon();

		vertices[0] = new EVertex(editor, this, new Vector2(-0.5f, -0.5f));
		vertices[1] = new EVertex(editor, this, new Vector2(0.5f, -0.5f));
		vertices[2] = new EVertex(editor, this, new Vector2(0.5f, 0.5f));
		vertices[3] = new EVertex(editor, this, new Vector2(-0.5f, 0.5f));
	}
	public EMesh(TextureSet.Tile t, Layer l, Editor e)
		: this(l, e)
	{
		Tile = t;
		mesh.Texture = t.Texture;
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
	public EMesh(Prism.Parser.PrismMesh prismMesh, Layer l, Editor e)
		:this(l, e)
	{
		for (int i = 0; i < prismMesh.VertexPosition.Length; i++)
			vertices[i].Position = prismMesh.VertexPosition[i];
	}

	public void Dispose()
	{
		mesh.Dispose();
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
		polygon.pointList.AddRange(VertexPositions);

		mesh.Vertices = VertexPositions;
		mesh.UV = VertexUV;

		dirty = false;
	}

	public void Logic()
	{
		if (keyboard.KeyPressed(Key.Minus) && Selected)
			layer.MoveMesh(this, 1);
		if (keyboard.KeyPressed(Key.Slash) && Selected)
			layer.MoveMesh(this, -1);
	}

	public void Draw()
	{
		if (dirty) UpdateMesh();
		if (!Visible || mesh.Texture == null) return;

		mesh.TextureEnabled = true;

		mesh.Color = new Color(1f, 1f, 1f, (Enabled || !OptionsForm.options.FocusLayer) ? 1f : OptionsForm.options.LayerOpacity);
		mesh.Draw();
	}

	public void DrawUI()
	{
		if (dirty) UpdateMesh();

		if (!Visible) return;

		if (Enabled)
		{
			mesh.TextureEnabled = false;

			if (OptionsForm.options.MeshBorders)
			{
				mesh.Color = (Selected ? Color.Yellow : Color.White) * new Color(1f, 1f, 1f, OptionsForm.options.MeshBorderOpacity);
				mesh.Draw(PrimitiveType.LineLoop);
			}

			mesh.Color = new Color(1f, 1f, 1f, Alpha);
			mesh.Draw();
		}

		if (editor.SelectMode == SelectMode.Vertices)
			foreach (EVertex v in vertices)
				v.Draw();
	}

	public IEnumerator GetEnumerator()
	{
		return new VertexEnum(this);
	}
}