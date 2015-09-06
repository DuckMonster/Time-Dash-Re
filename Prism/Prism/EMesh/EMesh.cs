using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class EMesh : IEnumerable, IDisposable
{
	#region Program
	public static ShaderProgram MeshProgram;
	public static void CompileProgram()
	{
		if (MeshProgram != null) return;

		MeshProgram = new ShaderProgram(
			ShaderProgram.ReadFile("EMesh/meshProgramVertex.cpp"),
			ShaderProgram.ReadFile("EMesh/meshProgramFragment.cpp"));
	}
	#endregion

	public IEnumerator GetEnumerator() { foreach (EVertex v in Vertices) yield return v; }

	Editor editor;
	Layer layer;

	MouseWatch mouse = Editor.mouse;

	public Layer Layer
	{
		get { return layer; }
		set
		{
			if (layer != null)
				layer.Meshes.Remove(this);

			layer = value;

			if (layer != null)
				layer.Meshes.Add(this);

			Program.outlinerForm.UpdateUI();
		}
	}

	Mesh mesh;
	TextureSet.Tile tile;

	List<EVertex> vertices = new List<EVertex>();
	public List<EVertex> Vertices
	{
		get { return vertices; }
	}

	public TextureSet.Tile Tile
	{
		get { return tile; }
		set
		{
			tile = value;
			if (tile == null) return;

			SetVerticesUV(tile.UV);
		}
	}

	public int Index
	{
		get { return layer.Meshes.IndexOf(this); }
		set
		{
			layer.Meshes.Remove(this);

			if (value == -1 || value >= layer.Meshes.Count)
				layer.Meshes.Add(this);
			else
				layer.Meshes.Insert(value, this);
		}
	}

	public bool Visible
	{
		get { return layer.Visible; }
	}

	public bool Enabled
	{
		get { return Visible && layer.Enabled; }
	}

	public bool Hovered
	{
		get
		{
			if (editor.DisableSelect) return false;
			return editor.HoveredMeshes.Contains(this);
		}
	}

	public bool Selected
	{
		get
		{
			return vertices.TrueForAll((v) => v.Selected);
		}
		set
		{
			if (value)
				editor.Select(Vertices);
			else
				editor.Deselect(Vertices);
		}
	}

	public EMesh(Layer layer, Editor editor)
	{
		this.editor = editor;
		this.Layer = layer;
		mesh = new Mesh(MeshProgram, 4);

		vertices.Add(new EVertex(this, mesh.Vertices[0], editor));
		vertices.Add(new EVertex(this, mesh.Vertices[1], editor));
		vertices.Add(new EVertex(this, mesh.Vertices[2], editor));
		vertices.Add(new EVertex(this, mesh.Vertices[3], editor));

		SetVertices(new RectangleF(-0.5f, -0.5f, 1f, 1f));
	}
	public EMesh(RectangleF rect, Layer l, Editor e)
		: this(l, e)
	{
		SetVertices(rect);
	}
	public EMesh(RectangleF rect, TextureSet.Tile tile, Layer l, Editor e)
		: this(l, e)
	{
		SetVertices(rect);
		Tile = tile;
	}
	public EMesh(EMesh copy, Layer l, Editor e)
		: this(l, e)
	{
		for (int i = 0; i < vertices.Count; i++)
			vertices[i].CopyFrom(copy.Vertices[i]);

		Tile = copy.tile;
	}
	public EMesh(Prism.Parser.PrismMesh copy, Layer l, Editor e)
		: this(l, e)
	{
		for (int i = 0; i < vertices.Count; i++)
			vertices[i].Position = copy.VertexPosition[i];
	}

	public void Dispose()
	{
		mesh.Dispose();
	}

	public void Remove()
	{
		Selected = false;
		Layer = null;
	}

	public void SetVertices(RectangleF rect)
	{
		vertices[0].Position = new Vector2(rect.X, rect.Y);
		vertices[1].Position = new Vector2(rect.X + rect.Width, rect.Y);
		vertices[2].Position = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
		vertices[3].Position = new Vector2(rect.X, rect.Y + rect.Height);
	}
	public void SetVerticesUV(RectangleF uv)
	{
		vertices[0].UV = new Vector2(uv.X, uv.Y);
		vertices[1].UV = new Vector2(uv.X + uv.Width, uv.Y);
		vertices[2].UV = new Vector2(uv.X + uv.Width, uv.Y + uv.Height);
		vertices[3].UV = new Vector2(uv.X, uv.Y + uv.Height);
	}

	public bool Intersects(Polygon p)
	{
		if (!Enabled) return false;
		return p.Intersects(new Polygon(mesh.GetAttribute<Vector3>("vertexPosition").Data));
	}

	public void Logic() { }

	public void Draw()
	{
		if (!Visible) return;
		if (Enabled)
		{
			GL.Enable(EnableCap.StencilTest);
			GL.StencilFunc(StencilFunction.Always, Index, 0xff);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
		}

		MeshProgram["enableTexture"].SetValue(tile != null);

		if (!Enabled && OptionsForm.options.FocusLayer)
			MeshProgram["uniColor"].SetValue(TKTools.Color.White * OptionsForm.options.LayerOpacity);
		else
			MeshProgram["uniColor"].SetValue(TKTools.Color.White);

		if (tile != null)
			tile.Texture.Bind();
		else
			MeshProgram["uniColor"].SetValue(new TKTools.Color(1f, 1f, 1f, 0f));

		mesh.Draw(PrimitiveType.Polygon);

		GL.Disable(EnableCap.StencilTest);
	}

	public void DrawUI()
	{
		if (!Visible) return;
		if (!Enabled) return;

		GL.Enable(EnableCap.StencilTest);
		GL.StencilFunc(StencilFunction.Gequal, Index, 0xff);
		GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);

		MeshProgram["enableTexture"].SetValue(false);
		MeshProgram["uniColor"].SetValue((Selected ? TKTools.Color.White : TKTools.Color.Yellow) * new TKTools.Color(1f, 1f, 1f, OptionsForm.options.MeshBorderOpacity));

		if (Selected)
			GL.LineWidth(2f);

		if (OptionsForm.options.MeshBorders || Hovered || Selected)
			mesh.Draw(PrimitiveType.LineLoop);

		GL.LineWidth(1f);

		if (Hovered)
		{
			MeshProgram["uniColor"].SetValue(new TKTools.Color(1f, 1f, 1f,
				(mouse[OpenTK.Input.MouseButton.Left] && !editor.DisableSelect && !editor.selectionBox.Active) ? 0.3f : 0.2f));
			mesh.Draw(PrimitiveType.Polygon);
		}

		//GL.Disable(EnableCap.StencilTest);

		if (editor.SelectMode == SelectMode.Vertices)
			foreach (EVertex v in vertices)
				v.Draw();
	}
}