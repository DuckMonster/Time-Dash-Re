using OpenTK;
using TKTools;

public class Manipulator
{
	protected Editor editor;

	public Vector2 Position
	{
		get
		{
			EVertex []v = editor.SelectedVertices.ToArray();
			Polygon p = new Polygon();
			foreach (EVertex ev in v)
				p.AddPoint(new Vector3(ev.Position));

			return p.Center;
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

	public Manipulator(Editor e)
	{
		editor = e;
	}

	public virtual void Logic()
	{

	}

	public virtual void Draw()
	{

	}
}