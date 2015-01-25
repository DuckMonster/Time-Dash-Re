using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TKTools;

namespace MapScene
{
	public class Scene : IDisposable
	{
		Map map;
		public List<EnvTileset> tilesetList = new List<EnvTileset>();
		public List<EnvTemplate> templateList = new List<EnvTemplate>();
		public List<EnvSolid> solidList = new List<EnvSolid>();
		public List<EnvObject> objectList = new List<EnvObject>();

		public float width = 40f, height = 40;

		public float Width
		{
			get
			{
				return width;
			}
		}

		public float Height
		{
			get
			{
				return height;
			}
		}

		public Scene(string filename, Map map)
		{
			this.map = map;
			LoadMap(filename);
		}

		public void Dispose()
		{
			foreach (EnvTileset t in tilesetList)
				t.Dispose();

			foreach (EnvTemplate t in templateList)
				t.Dispose();

			foreach (EnvSolid t in solidList)
				t.Dispose();

			foreach (EnvObject t in objectList)
				t.Dispose();

			tilesetList.Clear();
			templateList.Clear();
			solidList.Clear();
			objectList.Clear();
		}

		public void LoadMap(string filename)
		{
			using (BinaryReader reader = new BinaryReader(new FileStream("Maps/" + filename + ".tdm", FileMode.Open)))
			{
				int nmbrOfTilesets = reader.ReadInt32();
				for (int i = 0; i < nmbrOfTilesets; i++)
					tilesetList.Add(new EnvTileset(reader));

				int nmbrOfTemplates = reader.ReadInt32();
				for (int i = 0; i < nmbrOfTemplates; i++)
					templateList.Add(new EnvTemplate(reader, this));

				int nmbrOfLayers = reader.ReadInt32();
				for (int i = 0; i < nmbrOfLayers; i++)
				{
					float depth = 0;

					if (i == 0)
					{
						int nmbrOfObjects = reader.ReadInt32();

						for (int j = 0; j < nmbrOfObjects; j++)
							solidList.Add(new EnvSolid(reader, this));
					}
					else
					{
						depth = reader.ReadSingle();

						int nmbrOfObjects = reader.ReadInt32();

						for (int j = 0; j < nmbrOfObjects; j++)
							objectList.Add(new EnvObject(reader, depth, this));
					}
				}
			}
		}

		public bool GetCollision(Vector2 pos, Vector2 size)
		{
			Vector2 sizex = new Vector2(size.X/2, 0);
			Vector2 sizey = new Vector2(0, size.Y/2);

			Polygon p = new Polygon(new Vector2[] {
				pos + sizex + sizey,
				pos - sizex + sizey,
				pos - sizex - sizey,
				pos + sizex - sizey
			});

			foreach(EnvSolid solid in solidList)
				if (solid.GetCollision(p)) return true;

			return false;
		}

		public void Logic()
		{
		}

		public void Draw()
		{
			foreach (EnvSolid solid in solidList)
				solid.Draw();

			foreach (EnvObject obj in objectList)
				obj.Draw();
		}
	}

	public class EnvObject : IDisposable
	{
		Scene scene;
		Polygon polygon;
		Mesh mesh;

		float depth;

		public EnvObject(BinaryReader reader, float depth, Scene s)
		{
			scene = s;
			this.depth = depth;

			EnvTemplate t = s.templateList[reader.ReadInt32()];

			mesh = t.Mesh;

			polygon = new Polygon(new Vector2[] {
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle())
			});

			mesh.Vertices = polygon;
		}

		public void Dispose()
		{
			mesh.Dispose();
		}

		public void Draw()
		{
			mesh.Draw();
		}
	}

	public class EnvSolid
	{
		Scene scene;
		Polygon polygon;

		Mesh mesh;

		public EnvSolid(BinaryReader reader, Scene s)
		{
			scene = s;
			polygon = new Polygon(new Vector2[] {
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle())
			});

			mesh = Mesh.Box;
			mesh.Vertices = polygon;
		}

		public void Dispose()
		{
			mesh.Dispose();
		}

		public bool GetCollision(Polygon p)
		{
			return polygon.Intersects(p);
		}

		public void Draw()
		{
			mesh.Draw();
		}
	}

	public class EnvTemplate : IDisposable
	{
		Scene scene;
		int tilesetIndex;
		RectangleF uv;

		public Texture Texture
		{
			get
			{
				return scene.tilesetList[tilesetIndex].Texture;
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

		public EnvTemplate(BinaryReader reader, Scene s)
		{
			scene = s;
			tilesetIndex = reader.ReadInt32();
			uv = new RectangleF(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		}

		public void Dispose()
		{

		}
	}

	public class EnvTileset : IDisposable
	{
		Texture texture;

		public Texture Texture
		{
			get
			{
				return texture;
			}
		}

		public EnvTileset(BinaryReader reader)
		{
			using (FileStream str = new FileStream("temp", FileMode.Create))
			{
				byte[] buffer = new byte[reader.ReadInt32()];
				reader.Read(buffer, 0, buffer.Length);

				str.Write(buffer, 0, buffer.Length);
			}

			texture = new Texture("temp");
			File.Delete("temp");
		}

		public void Dispose()
		{
			texture.Dispose();
		}
	}

	public class EnvLayer
	{
		float depth;

		public float Depth
		{
			get
			{
				return depth;
			}
			set
			{
				depth = value;
			}
		}

		public EnvLayer(BinaryReader reader)
		{
			Depth = depth;
		}
	}
}