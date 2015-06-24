using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class EMesh : IEnumerable
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

	EVertex[] vertices = new EVertex[4];
	Mesh mesh;
	Polygon polygon;

	Vector3[] VertexPositions
	{
		get
		{
			Vector3[] p = new Vector3[vertices.Length];
			for (int i = 0; i < p.Length; i++)
				p[i] = new Vector3(vertices[i].Position);

			return p;
		}
	}
	Vector2[] VertexUV
	{
		get
		{
			Vector2[] uv = new Vector2[vertices.Length];
			for (int i = 0; i < uv.Length; i++)
				uv[i] = vertices[i].UV;

			return uv;
		}
	}

	public bool Hovered
	{
		get
		{
			if (editor.BlockSelect) return false;
			foreach (EVertex v in vertices)
				if (v.Hovered) return false;

			if (editor.DragSelecting)
				return polygon.Intersects(editor.SelectArea);
			else
				return polygon.Intersects(Editor.mouse.Position);
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
			return editor.selectedMeshes.Contains(this);
		}
	}

	public EMesh(Vector2 origin, Editor e)
	{
		editor = e;

		mesh = new Mesh();
		polygon = new Polygon();

		vertices[0] = new EVertex(editor, this, origin + new Vector2(-0.5f, -0.5f), new Vector2(0, 0));
        vertices[1] = new EVertex(editor, this, origin + new Vector2(0.5f, -0.5f), new Vector2(1, 0));
		vertices[2] = new EVertex(editor, this, origin + new Vector2(0.5f, 0.5f), new Vector2(1, 1));
		vertices[3] = new EVertex(editor, this, origin + new Vector2(-0.5f, 0.5f), new Vector2(0, 1));

		UpdateMesh();
	}

	public bool Intersects(Polygon p)
	{
		return polygon.Intersects(p);
	}

	public void UpdateMesh()
	{
		polygon.pointList.Clear();
		polygon.pointList.AddRange(VertexPositions);

		mesh.Vertices = VertexPositions;
		mesh.UV = VertexUV;
	}

	public void Select()
	{
		editor.SelectMesh(this);
	}

	public void Logic()
	{
	}

	public void Draw()
	{
		mesh.Color = Selected ? Color.Yellow : Color.White;
		mesh.Draw(PrimitiveType.LineLoop);

		mesh.Color = new Color(1f, 1f, 1f, Alpha);
		mesh.Draw();

		if (editor.VertexSelection)
			foreach (EVertex v in vertices)
				v.Draw();
	}

	public IEnumerator GetEnumerator()
	{
		return new VertexEnum(this);
	}
}