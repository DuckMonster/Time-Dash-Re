using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.IO;
using TKTools;
namespace MapEditor
{
	public class EditorObject : IDisposable
	{
		protected Editor editor;
		protected Vertex[] vertices = new Vertex[4];
		public Mesh mesh;

		public Layer layer;

		public Template template;

		public bool Hovered
		{
			get
			{
				if (!Active || editor.CurrentManipulator.Active || editor.CurrentManipulator.Hovered || editor.Paused || editor.preview) return false;

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

		public bool Active
		{
			get
			{
				return layer.Active || editor.preview;
			}
		}

		public virtual Color Color
		{
			get
			{
				if (layer.ButtonHovered)
					return new Color(1, 1, 0, Active ? 1f : 0.4f);
				else
					return new Color(1, 1, 1, Active ? 1f : 0.4f);
			}
		}

		public EditorObject(Layer layer, Editor e)
		{
			editor = e;
			this.layer = layer;

			mesh = Mesh.Box;
			for (int i = 0; i < mesh.Vertices.Length; i++)
				vertices[i] = new Vertex(mesh.Vertices[i], mesh.UV[i], editor);
		}

		public EditorObject(Layer layer, EditorObject copy, Editor e)
		{
			editor = e;
			this.layer = layer;
			template = copy.template;

			mesh = new Mesh(copy.mesh.Vertices, copy.mesh.UV, PrimitiveType.Quads);

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = new Vertex(copy.vertices[i].Position, copy.vertices[i].UV, e);

			mesh.Texture = copy.mesh.Texture;

			if (template != null)
				template.references.Add(this);
		}

		public EditorObject(Layer layer, Template template, Editor e)
		{
			editor = e;
			this.layer = layer;

			LoadTemplate(template);
		}

		public EditorObject(Layer layer, BinaryReader reader, Editor e)
		{
			editor = e;
			this.layer = layer;

			int templateID = reader.ReadInt32();
			LoadTemplate(e.templateList[templateID]);

			foreach (Vertex v in Vertices)
			{
				v.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			}
		}

		public void LoadTemplate(Template t)
		{
			template = t;

			mesh = t.Mesh;

			for (int i = 0; i < vertices.Length; i++)
				vertices[i] = new Vertex(mesh.Vertices[i], mesh.UV[i], editor);

			t.references.Add(this);
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

			Vector2 center = mesh.Polygon.Center;
			for (int i = 0; i < vectorList.Length; i++)
			{
				vectorList[i] = (vectorList[i] - center) * 1.015f + center;
			}

			//mesh.Vertices = vectorList;
		}

		public virtual void Logic()
		{
			foreach (Vertex v in vertices)
				v.Logic();
			UpdateMesh();
		}

		public void Select()
		{
			editor.Select(Vertices);
		}

		public virtual void WriteToFile(BinaryWriter writer)
		{
			writer.Write(template.ID);
			foreach (Vertex v in Vertices)
			{
				writer.Write(v.Position.X);
				writer.Write(v.Position.Y);
			}
		}

		public virtual void Draw()
		{
			if (layer.Z < editor.ActiveLayer.Z && !editor.preview) return;

			GL.Enable(EnableCap.DepthTest);
			mesh.Color = Color;
			mesh.UsingTexture = true;
			mesh.Reset();
			mesh.Translate(0, 0, -layer.Z);
			mesh.Draw();
			GL.Disable(EnableCap.DepthTest);

			if (Hovered)
			{
				mesh.UsingTexture = false;
				mesh.Color = new Color(1, 1, 1, 0.2f);

				mesh.Draw();
			}

			if (Active && !editor.preview) DrawVertices();
		}

		public void DrawVertices()
		{
			foreach (Vertex v in vertices)
				v.Draw();
		}
	}
}