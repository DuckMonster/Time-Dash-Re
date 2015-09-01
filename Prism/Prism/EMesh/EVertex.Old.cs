using OpenTK;
using OpenTK.Input;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class EVertex : IDisposable
{
	static readonly float HoverRange = 0.08f;

	Editor editor;
	Vertex vertex;

	EMesh mesh;
	Vector2 position;

	MouseWatch mouse = Editor.mouse;

	Model2D vertexModel;
	Vector3 color;

	public Vector2 Position
	{
		get { return position; }
		set
		{
			position = value;
			vertex.SetAttribute("vertexPosition", new Vector3(position));
			mesh.SetDirty();
		}
	}

	public Vector3 Color
	{
		get { return color; }
		set
		{
			color = value;
			vertex.SetAttribute("vertexColor", color);
			mesh.SetDirty();
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
			if (editor.SelectMode != SelectMode.Vertices || !Enabled) return false;

			if (editor.selectionBox.Active) return editor.selectionBox.Intersects(this);
			return ((mouse.Position.Xy - position).LengthFast < HoverRange);
		}
	}

	public bool Selected
	{
		get
		{
			return editor.SelectedVertices.Contains(this);
		} set
		{
			if (value)
				editor.Select(new EVertex[] { this });
			else
				editor.Deselect(new EVertex[] { this });
		}
	}

	public float Alpha
	{
		get
		{
			return Hovered ? 0.8f : 0.2f;
		}
	}

	public EVertex(Editor e, EMesh mesh, Vertex vertex, Vector2 position)
	{
		editor = e;

		this.mesh = mesh;
		this.vertex = vertex;
		this.position = position;

		Color = new Vector3(1f, 1f, 1f);

		vertexModel = new Model2D(new Vector2[] {
			new Vector2(-0.04f, -0.04f),
			new Vector2(0.04f, -0.04f),
			new Vector2(0.04f, 0.04f),
			new Vector2(-0.04f, 0.04f)
		});
	}

	public void Dispose()
	{
		vertexModel.Dispose();
	}

	public bool Intersects(Polygon p)
	{
		if (!Enabled) return false;

		if (p.pointList.Count == 1)
			return (p.pointList[0].Xy - position).LengthFast < HoverRange;
		else
			return p.Intersects(new Vector3(position));
	}

	public void Logic()
	{
	}

	public void Draw()
	{
		if (!Enabled) return;

		vertexModel.Color = new Color(1f, 1f, 1f, Alpha);

		vertexModel.Reset();
		vertexModel.Translate(position);
		vertexModel.Draw();

		if (Selected)
		{
			vertexModel.Color = TKTools.Color.Yellow;

			vertexModel.Scale(1.4f);
			vertexModel.Draw(OpenTK.Graphics.OpenGL.PrimitiveType.LineLoop);
		}
	}
}