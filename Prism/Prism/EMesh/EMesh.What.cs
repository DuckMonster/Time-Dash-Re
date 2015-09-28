using OpenTK;
using OpenTK.Graphics.OpenGL;
using Prism.Parser;
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
	MeshDesign design;

	List<EVertex> vertices = new List<EVertex>();
	public List<EVertex> Vertices
	{
		get { return vertices; }
	}

	public MeshDesign Design
	{
		get { return design; }
		set
		{
			design = value;
			if (design.Type == MeshDesign.DesignType.Tile)
				SetVerticesUV(design.Tile.UV);
		}
	}

	public TextureSet.Tile Tile
	{
		get { return design.Tile; }
	}

	public TKTools.Color Color
	{
		get { return design.Color; }
	}

	public MeshDesign.DesignType DesignType
	{
		get { return design.Type; }
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
	public EMesh(RectangleF rect, MeshDesign design, Layer l, Editor e)
		: this(l, e)
	{
		SetVertices(rect);
		Design = design;
	}
	public EMesh(EMesh copy, Layer l, Editor e)
		: this(l, e)
	{
		for (int i = 0; i < vertices.Count; i++)
			vertices[i].CopyFrom(copy.Vertices[i]);

		Design = copy.Design;
	}
	public EMesh(PrismMesh copy, Dictionary<PrismTexture.Tile, TextureSet.Tile> tiles, Layer l, Editor e)
		: this(l, e)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i].Position = copy.VertexPosition[i];
			vertices[i].HSL = copy.VertexColor[i];
		}

		if (copy.Tile != null)
			design = new MeshDesign(tiles[copy.Tile]);
		else
			design = new MeshDesign(TKTools.Color.Red);
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

		TKTools.Color c = Design.Tile == null ? Color : TKTools.Color.White;
		MeshProgram["enableTexture"].SetValue(Design.Tile != null);

		if (!Enabled && OptionsForm.options.FocusLayer)
			MeshProgram["uniColor"].SetValue(c * OptionsForm.options.LayerOpacity);
		else
			MeshProgram["uniColor"].SetValue(c);

		if (Design.Tile != null)
			Design.Tile.Texture.Bind();

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

		TKTools.Color color = Selected ? TKTools.Color.White : TKTools.Color.Yellow;
		if (!(Selected || Hovered))
			color *= new TKTools.Color(1f, 1f, 1f, OptionsForm.options.MeshBorderOpacity);

		MeshProgram["uniColor"].SetValue(color);

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