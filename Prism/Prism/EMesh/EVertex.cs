using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class EVertex : IDisposable
{
	static readonly float HoverRange = 0.15f;

	Editor editor;
	EMesh mesh;

	Vector2 position, uv;
	ColorHSL hslShift = new ColorHSL(0f, 1f, 1f);

	Vertex vertex;

	Model2D rectModel;

	MouseWatch mouse = Editor.mouse;

	public Vector2 Position
	{
		get { return position; }
		set
		{
			position = value;
			vertex.SetAttribute("vertexPosition", new Vector3(position));
		}
	}
	public Vector2 UV
	{
		get { return uv; }
		set
		{
			uv = value;
			vertex.SetAttribute("vertexUV", uv);
		}
	}

	public ColorHSL HSL
	{
		get { return hslShift; }
		set
		{
			hslShift = value;
			UploadHSL();
		}
	}

	public float Hue
	{
		get { return hslShift.H; }
		set
		{
			hslShift.H = value;
			UploadHSL();
		}
	}

	public float Saturation
	{
		get { return hslShift.S; }
		set
		{
			hslShift.S = value;
			UploadHSL();
		}
	}

	public float Lightness
	{
		get { return hslShift.L; }
		set
		{
			hslShift.L = value;
			UploadHSL();
		}
	}

	public VertexBrush Brush { get { return new VertexBrush(hslShift, 0, 0, 0); } }

	public bool Enabled
	{
		get { return mesh.Enabled; }
	}

	public bool Hovered
	{
		get
		{
			if (editor.selectionBox.Active)
				return editor.selectionBox.Intersects(this);

			return (mouse.Position.Xy - Position).Length <= HoverRange;
		}
	}

	public bool Selected
	{
		get { return editor.SelectedVertices.Contains(this); }
		set
		{
			if (value)
				editor.Select(new EVertex[] { this });
			else
				editor.Deselect(new EVertex[] { this });
		}
	}

	public EVertex(EMesh mesh, Vertex vertex, Editor editor)
	{
		this.editor = editor;
		this.mesh = mesh;
		this.vertex = vertex;

		Position = Vector2.Zero;
		UV = Vector2.Zero;
		UploadHSL();

		rectModel = (Model2D)Model.CreateFromPrimitive(MeshPrimitive.Quad);
	}

	public void CopyFrom(EVertex v)
	{
		Position = v.Position;
		HSL = v.HSL;
	}

	void UploadHSL()
	{
		vertex.SetAttribute("vertexHSL", HSL.ToVector);
	}

	public void Paint(ColorHSL brush, float weight = 1f)
	{
		ColorHSL c = ColorHSL.Blend(HSL, brush, weight);
		HSL = c;
	}

	public bool Intersects(Polygon p)
	{
		if (!Enabled) return false;

		if (p.pointList.Count == 1)
			return (p.pointList[0].Xy - position).LengthFast < HoverRange;
		else
			return p.Intersects(new Vector3(position));
	}

	public void Dispose()
	{
		rectModel.Dispose();
	}

	public void Draw()
	{
		rectModel.Color = Selected ? Color.White : Color.Yellow;

		rectModel.Reset();

		rectModel.Translate(Position);
		rectModel.Scale(0.015f * editor.editorCamera.Position.Z);

		if (Selected)
			GL.LineWidth(2f);

		rectModel.Draw(PrimitiveType.LineLoop);

		GL.LineWidth(1f);

		if (Hovered)
		{
			rectModel.Color = new Color(1f, 1f, 1f,
				(mouse[MouseButton.Left] && !editor.DisableSelect && !editor.selectionBox.Active) ? 0.8f : 0.4f);

			rectModel.Scale(0.6f);
			rectModel.Draw(PrimitiveType.Polygon);
		}
	}
}