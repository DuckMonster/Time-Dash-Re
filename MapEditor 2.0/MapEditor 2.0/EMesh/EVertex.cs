using OpenTK;
using OpenTK.Input;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class EVertex
{
	static readonly float HoverRange = 0.08f;

	Editor editor;

	EMesh mesh;
	Vector2 position;
	Vector2 uv;

	MouseWatch mouse = Editor.mouse;

	Mesh vertexMesh;

	public Vector2 Position
	{
		get { return position; }
		set { position = value; }
	}

	public Vector2 UV
	{
		get { return uv; }
		set { uv = value; }
	}

	public bool Hovered
	{
		get
		{
			if (!editor.VertexSelection) return false;
			return ((mouse.Position.Xy - position).LengthFast < HoverRange);
		}
	}

	public bool Selected
	{
		get
		{
			return editor.SelectedVertices.Contains(this);
		}
	}

	public float Alpha
	{
		get
		{
			float selectedAlpha = Selected ? 0.4f : 0.2f;
			if (Hovered)
				return selectedAlpha + (mouse[MouseButton.Left] ? 4f : 2f);
			else
				return selectedAlpha;
		}
	}

	public EVertex(Editor e, EMesh mesh, Vector2 position, Vector2 uv)
	{
		editor = e;

		this.mesh = mesh;
		this.position = position;
		this.uv = uv;

		vertexMesh = new Mesh(new Vector2[] {
			new Vector2(-0.04f, -0.04f),
			new Vector2(0.04f, -0.04f),
			new Vector2(0.04f, 0.04f),
			new Vector2(-0.04f, 0.04f)
		});
	}

	public bool Intersects(Polygon p)
	{
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
		if (editor.VertexSelection)
		{
			vertexMesh.Color = new Color(1f, 1f, 1f, Alpha);

			vertexMesh.Reset();
			vertexMesh.Translate(position);
			vertexMesh.Draw();
		}
	}
}