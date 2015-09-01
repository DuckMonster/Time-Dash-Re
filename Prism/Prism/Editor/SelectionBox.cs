using OpenTK;
using OpenTK.Input;
using System.Collections.Generic;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class SelectionBox
{
	Vector2? origin;
	Vector2? target;

	Editor editor;
	Model model;

	MouseWatch mouse = Editor.mouse;

	Polygon polygon;

	public bool Active
	{
		get { return target != null; }
	}

	public Polygon Polygon
	{
		get
		{
			BakePolygon();
			return polygon;
		}
	}

	public SelectionBox(Editor e)
	{
		editor = e;
		model = new Model(new Vector3[] {
			new Vector3(0, 0, 0),
			new Vector3(1, 0, 0),
			new Vector3(1, 1, 0),
			new Vector3(0, 1, 0)
		});
	}

	public bool Intersects(EMesh m)
	{
		BakePolygon();

		return m.Intersects(polygon);
	}

	public bool Intersects(EVertex v)
	{
		BakePolygon();

		return v.Intersects(polygon);
	}

	void BakePolygon()
	{
		if (!Active) polygon = new Polygon(new Vector3[] { mouse.Position });
		else
		{
			Vector3 tx = new Vector3(target.Value - origin.Value) * new Vector3(1, 0, 0);
			Vector3 ty = new Vector3(target.Value - origin.Value) * new Vector3(0, 1, 0);

			polygon = new Polygon(new Vector3[] {
			new Vector3(origin.Value),
			new Vector3(origin.Value) + tx,
			new Vector3(origin.Value) + tx + ty,
			new Vector3(origin.Value) + ty
		});
		}
	}

	public List<EVertex> GetSelectedVertices()
	{
		BakePolygon();

		List<EVertex> list = new List<EVertex>();

		foreach (EMesh m in editor.Meshes)
			foreach (EVertex v in m)
				if (v.Intersects(polygon))
					list.Add(v);

		return list;
	}

	public List<EMesh> GetSelectedMeshes()
	{
		BakePolygon();

		List<EMesh> list = new List<EMesh>();

		foreach (Layer l in editor.activeLayers)
			for (int i = l.Meshes.Count - 1; i >= 0; i--)
				if (l.Meshes[i].Intersects(polygon))
				{
					list.Add(l.Meshes[i]);
					if (!Active) break;
				}

		return list;
	}

	public void Logic()
	{
		if (editor.form.Focused)
		{
			if (mouse.ButtonPressed(MouseButton.Left) && !editor.DisableSelect)
				origin = mouse.Position.Xy;

			if (mouse[MouseButton.Left] && origin != null)
			{
				if (target != null || (mouse.Position.Xy - origin.Value).LengthFast > 0.01f)
					target = mouse.Position.Xy;
			}

			if (!mouse[MouseButton.Left] && origin != null)
			{
				if (!editor.DisableSelect)
					editor.OnSelect();

				origin = null;
				target = null;
			}
		}
	}

	public void Draw()
	{
		if (Active)
		{
			model.Reset();
			model.Translate(origin.Value);
			model.Scale(mouse.Position.Xy - origin.Value);

			model.Color = new Color(1f, 1f, 1f, 0.1f);
			model.Draw();
			model.Color = Color.Yellow;
			model.Draw(OpenTK.Graphics.OpenGL.PrimitiveType.LineLoop);
		}
	}
}