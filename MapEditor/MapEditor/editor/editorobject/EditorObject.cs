using OpenTK;
using OpenTK.Input;
using System;
using System.Diagnostics;
using TKTools;
namespace MapEditor
{
	public class EditorObject : IDisposable
	{
		//static Texture tempTexture = new Texture("res/portal.png");

		Editor editor;
		Vertex[] vertices = new Vertex[4];
		public Mesh mesh;

		public bool Hovered
		{
			get
			{
				if (editor.CurrentManipulator.Active || editor.CurrentManipulator.Hovered) return false;

				foreach (Vertex v in vertices)
					if (v.Hovered) return false;

				return GetCollision(MouseInput.Current, Vector2.Zero);
			}
		}

		public bool Selected
		{
			get
			{
				foreach (Vertex v in vertices)
					if (!v.Selected) return false;

				return true;
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
				vertices[i] = new Vertex(mesh.Vertices[i], mesh.UV[i], editor);
		}

		public EditorObject(EditorObject copy, Editor e)
		{
			editor = e;

			mesh = new Mesh(copy.mesh.Vertices, copy.mesh.UV, OpenTK.Graphics.OpenGL.PrimitiveType.Quads);

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = new Vertex(copy.vertices[i].Position, copy.vertices[i].UV, e);

			mesh.Texture = copy.mesh.Texture;
		}

		public EditorObject(Template template, Editor e)
		{
			editor = e;

			mesh = template.Mesh;

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = new Vertex(mesh.Vertices[i], mesh.UV[i], e);
		}

		public void Dispose()
		{
			mesh.Dispose();
			foreach (Vertex v in Vertices)
			{
				v.Dispose();
			}
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
			editor.Select(Vertices);
		}

		public void Draw()
		{
			mesh.Color = Color.White;
			mesh.UsingTexture = true;
			mesh.Draw();

			if (Hovered)
			{
				mesh.UsingTexture = false;
				mesh.Color = new Color(1, 1, 1, 0.2f);

				mesh.Draw();
			}

			foreach (Vertex v in vertices)
				v.Draw();
		}
	}
}