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
	float hue = 0f, saturation = 1f, lightness = 1f;

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

	public float Hue
	{
		get { return hue; }
		set
		{
			hue = value;
			UploadHSL();
		}
	}

	public float Saturation
	{
		get { return saturation; }
		set
		{
			saturation = value;
			UploadHSL();
		}
	}

	public float Lightness
	{
		get { return lightness; }
		set
		{
			lightness = value;
			UploadHSL();
		}
	}

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

	void UploadHSL()
	{
		vertex.SetAttribute<Vector3>("vertexHSL", new Vector3(hue, saturation, lightness));
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
	}

	public void Draw()
	{
		rectModel.Color = Selected ? Color.White : Color.Yellow;

		rectModel.Reset();

		rectModel.Translate(Position);
		rectModel.Scale(0.1f);

		rectModel.Draw(PrimitiveType.LineLoop);

		if (Hovered)
		{
			rectModel.Color = new Color(1f, 1f, 1f, 
				(mouse[MouseButton.Left] && !editor.DisableSelect && !editor.selectionBox.Active) ? 0.8f : 0.4f);

			rectModel.Scale(0.6f);
			rectModel.Draw(PrimitiveType.Polygon);
		}
	}
}