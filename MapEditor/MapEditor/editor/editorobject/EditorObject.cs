using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using TKTools;
namespace MapEditor
{
	public class EditorObject
	{
		Editor editor;
		Vertex[] vertices = new Vertex[4];
		public Mesh mesh;

		public bool Hovered
		{
			get
			{
				foreach (Vertex v in vertices)
					if (v.Hovered) return false;

				return GetCollision(MouseInput.Current, Vector2.Zero);
			}
		}

		public bool GetCollision(Vector2 pos, Vector2 size)
		{
			return GetCollision(new Polygon(pos));
		}

		public bool GetCollision(Polygon p)
		{
			return mesh.Polygon.Intersects(p);
		}

		public Vertex[] Vertices
		{
			get
			{
				return vertices;
			}
		}

		public EditorObject(Editor e)
		{
			editor = e;

			mesh = Mesh.Box;
			for (int i = 0; i < mesh.Vertices.Length; i++)
				vertices[i] = new Vertex(mesh.Vertices[i], editor);
		}

		public void UpdateMesh()
		{
			Vector2[] vectorList = new Vector2[4];

			for (int i = 0; i < vertices.Length; i++)
				vectorList[i] = vertices[i].Position;

			mesh.Vertices = vectorList;
		}

		public void Logic()
		{
			foreach (Vertex v in vertices)
				v.Logic();
			UpdateMesh();
		}

		public void Select()
		{
			
		}

		public void Draw()
		{
			Color c;
			c = Color.White;

			if (Hovered) c.A = 1f;
			else c.A = 0.5f;

			mesh.Color = c;
			mesh.Draw();

			foreach (Vertex v in vertices)
				v.Draw();
		}
	}
}