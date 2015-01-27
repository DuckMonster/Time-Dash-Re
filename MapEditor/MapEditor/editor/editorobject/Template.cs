using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TKTools;

namespace MapEditor
{
	public class Template : IDisposable
	{
		public List<EditorObject> references = new List<EditorObject>();

		Editor editor;

		int id;
		int tilesetIndex;
		RectangleF uv;

		Mesh displayMesh;

		public int ID
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}

		public Texture Texture
		{
			get
			{
				return editor.tilesetList[tilesetIndex].Texture;
			}
		}

		public Mesh Mesh
		{
			get
			{
				Mesh m = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
				Vector2 size = Size / 2;

				m.Vertices = new Vector2[] {
					new Vector2(-size.X, size.Y),
					new Vector2(size.X, size.Y),
					new Vector2(size.X, -size.Y),
					new Vector2(-size.X, -size.Y)
				};

				m.UV = new Vector2[] {
					new Vector2(uv.X, uv.Y),
					new Vector2(uv.X + uv.Width, uv.Y),
					new Vector2(uv.X + uv.Width, uv.Y + uv.Height),
					new Vector2(uv.X, uv.Y + uv.Height)
				};

				m.Texture = Texture;

				return m;
			}
		}

		public Vector2 Size
		{
			get
			{
				float ratio = uv.Height / uv.Width;

				if (uv.Width > uv.Height)
					return new Vector2(1f, ratio);
				else
					return new Vector2(1 / ratio, 1f);
			}
		}

		public Template(int tilesetIndex, RectangleF uv, int id, Editor e)
		{
			editor = e;

			this.id = id;
			this.tilesetIndex = tilesetIndex;
			this.uv = uv;

			editor.tilesetList[tilesetIndex].references.Add(this);

			displayMesh = Mesh;
			displayMesh.UIElement = true;
		}

		public Template(BinaryReader reader, int id, Editor e)
		{
			editor = e;

			this.id = id;

			tilesetIndex = reader.ReadInt32();
			uv = new RectangleF(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

			editor.tilesetList[tilesetIndex].references.Add(this);

			displayMesh = Mesh;
			displayMesh.UIElement = true;
		}

		public void Dispose()
		{
			displayMesh.Dispose();
		}

		public void WriteToFile(BinaryWriter writer)
		{
			writer.Write(tilesetIndex);
			writer.Write(uv.X);
			writer.Write(uv.Y);
			writer.Write(uv.Width);
			writer.Write(uv.Height);
		}

		public void Draw(Vector2 position) { Draw(position, 1f); }
		public void Draw(Vector2 position, float size) { Draw(position, new Vector2(size, size)); }
		public void Draw(Vector2 position, Vector2 size)
		{
			displayMesh.Reset();

			displayMesh.Translate(position);
			displayMesh.Scale(size);

			displayMesh.Draw();
		}
	}
}
