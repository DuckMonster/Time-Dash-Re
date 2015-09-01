using OpenTK;
using OpenTK.Input;
using System;
using TKTools;
using TKTools.Context.Input;

public class Manipulator
{
	protected Editor editor;

	public virtual Vector2 Position
	{
		get
		{
			/*
			EVertex []v = editor.SelectedVertices.ToArray();
			Polygon p = new Polygon();
			foreach (EVertex ev in v)
				p.AddPoint(new Vector3(ev.Position));

			return p.Center;*/

			if (pivot != null)
				return pivot.Value;

			if (pivotVertex != null)
				return pivotVertex.Position;

			Vector2 pos = Vector2.Zero;

			foreach (EVertex ev in editor.SelectedVertices)
				pos += ev.Position;

			return pos / editor.SelectedVertices.Count;
		}
	}

	public bool Visible
	{
		get
		{
			if (editor.SelectedVertices.Count == 0) return false;

			return true;
		}
	}

	public virtual bool Active
	{
		get { return false; }
	}

	public virtual bool Hovered
	{
		get
		{
			return false;
		}
	}

	public Vector2 NormalY
	{
		get
		{
			if (editor.SelectedVertices.Count < 2) return new Vector2(0, 1);
			else return (editor.SelectedVertices[1].Position - editor.SelectedVertices[0].Position).PerpendicularLeft.Normalized();
		}
	}

	public Vector2 NormalX
	{
		get
		{
			return NormalY.PerpendicularRight;
		}
	}

	Vector2? pivot;
	EVertex pivotVertex;

	protected MouseWatch mouse = Editor.mouse;
	protected KeyboardWatch keyboard = Editor.keyboard;

	public Manipulator(Editor e)
	{
		editor = e;
	}

	public virtual void Logic()
	{
	}
	
	public void UpdatePivot()
	{
		if (keyboard[Key.D] && !keyboard[Key.LControl])
		{
			if (keyboard[Key.LControl])
				pivot = new Vector2((float)Math.Round(mouse.Position.X), (float)Math.Round(mouse.Position.Y));
			else if (keyboard[Key.C] && editor.SelectedVertices.Count > 0)
			{
				pivot = null;

				EVertex closestVertex = editor.SelectedVertices[0];
				foreach (EVertex v in editor.SelectedVertices)
				{
					if ((v.Position - mouse.Position.Xy).Length < (closestVertex.Position - mouse.Position.Xy).Length)
						closestVertex = v;
				}

				pivotVertex = closestVertex;
			}
			else
				pivot = mouse.Position.Xy;
		}
	}

	public void ResetPivot()
	{
		pivot = null;
		pivotVertex = null;
	}

	public virtual void Draw()
	{

	}
}