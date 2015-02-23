using OpenTK;
using OpenTK.Graphics.OpenGL;
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
		public List<EnvLayer> layerList = new List<EnvLayer>();

		Texture backgroundTexture;

		public float width = 40f, height = 40;
		public Vector2 originOffset;

		Mesh backgroundMesh;

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
			backgroundMesh = Mesh.Box;
			backgroundMesh.Orthographic = true;
			backgroundMesh.Translate(0.5f, -0.5f);

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

			foreach (EnvLayer l in layerList)
				l.Dispose();

			tilesetList.Clear();
			templateList.Clear();
			solidList.Clear();
			objectList.Clear();
			layerList.Clear();

			backgroundMesh.Dispose();
			backgroundTexture.Dispose();
		}

		public void LoadMap(string filename)
		{
			using (BinaryReader reader = new BinaryReader(new FileStream("Maps/" + filename, FileMode.Open)))
			{
				reader.ReadString();
				reader.ReadInt32();

				if (reader.ReadBoolean())
				{
					using (FileStream str = new FileStream("temp", FileMode.Create))
					{
						byte[] buffer = new byte[reader.ReadInt32()];
						reader.Read(buffer, 0, buffer.Length);

						str.Write(buffer, 0, buffer.Length);
					}

					backgroundTexture = new Texture("temp");
					backgroundMesh.Texture = backgroundTexture;

					File.Delete("temp");
				}

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

					if (i == 0)	//SOLIDS LAYER
					{
						int nmbrOfObjects = reader.ReadInt32();

						for (int j = 0; j < nmbrOfObjects; j++)
						{
							int type = reader.ReadInt32();

							if (type == 0)
								solidList.Add(new EnvSolid(reader, this));
							else
							{
								Polygon p = new Polygon(new Vector2[] {
								new Vector2(reader.ReadSingle(), reader.ReadSingle()),
								new Vector2(reader.ReadSingle(), reader.ReadSingle()),
								new Vector2(reader.ReadSingle(), reader.ReadSingle()),
								new Vector2(reader.ReadSingle(), reader.ReadSingle())
							});

								map.SceneZone(type, p);
							}
						}
					}
					else
					{
						depth = reader.ReadSingle();
						EnvLayer layer = new EnvLayer(depth);

						int nmbrOfObjects = reader.ReadInt32();

						for (int j = 0; j < nmbrOfObjects; j++)
						{
							EnvObject obj = new EnvObject(reader, depth, this);
							objectList.Add(obj);
							layer.AddObject(obj);
						}

						layer.CreateCombinedMesh();
						layerList.Add(layer);
					}
				}

				Polygon combinedPoly = new Polygon();

				foreach (EnvSolid solid in solidList)
					combinedPoly.AddPoint(solid.polygon);

				RectangleF rect = combinedPoly.Bounds;

				originOffset = new Vector2(rect.X, rect.Y) + new Vector2(rect.Width / 2, rect.Height / 2);

				width = rect.Width;
				height = rect.Height;

				layerList.Sort(delegate(EnvLayer x, EnvLayer y)
				{
					if (x.depth == y.depth) return 0;
					else if (x.depth < y.depth) return 1;
					else return -1;
				});
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

			RectangleF pbounds = p.Bounds;

			foreach(EnvSolid solid in solidList)
				if (solid.GetCollision(p, pbounds)) return true;

			return false;
		}

		public void Logic()
		{
		}

		public void Draw()
		{
			if (backgroundTexture != null)
				backgroundMesh.Draw();

			//foreach (EnvSolid solid in solidList)
			//	solid.Draw();

			foreach (EnvLayer layer in layerList)
				layer.Draw();
		}
	}

	public class EnvObject : IDisposable
	{
		Scene scene;
		public Polygon polygon;
		public Mesh mesh;
		public EnvTemplate template;

		public float depth;

		public EnvObject(BinaryReader reader, float depth, Scene s)
		{
			scene = s;
			this.depth = depth;

			template = s.templateList[reader.ReadInt32()];

			mesh = template.Mesh;

			mesh.Vertices3D = new Vector3[] {
				new Vector3(reader.ReadSingle(), reader.ReadSingle(), -depth),
				new Vector3(reader.ReadSingle(), reader.ReadSingle(), -depth),
				new Vector3(reader.ReadSingle(), reader.ReadSingle(), -depth),
				new Vector3(reader.ReadSingle(), reader.ReadSingle(), -depth)
			};
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
		public Polygon polygon;
		RectangleF rectangle;
		Vector2 center;

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

			rectangle = polygon.Bounds;
			center = polygon.Center;
		}

		public void Dispose()
		{
			mesh.Dispose();
		}

		public bool GetCollision(Polygon p, RectangleF pb)
		{
			//return (pb.X + pb.Width >= rectangle.X &&
			//	pb.X < rectangle.X + rectangle.Width &&
			//	pb.Y + pb.Height >= rectangle.Y &&
			//	pb.Y < rectangle.Y + rectangle.Height);

			return p.Intersects(polygon);
		}

		public void Draw()
		{
			mesh.Draw();
		}
	}

	public class EnvTemplate : IDisposable
	{
		Scene scene;
		public int tilesetIndex;
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

	public class EnvLayer : IDisposable
	{
		List<EnvObject> objects = new List<EnvObject>();
		List<Mesh> combinedMeshes = new List<Mesh>();

		public float depth;

		public EnvLayer(float depth)
		{
			this.depth = depth;
		}

		public void Dispose()
		{
			foreach (Mesh m in combinedMeshes)
				m.Dispose();
		}

		public void AddObject(EnvObject obj)
		{
			objects.Add(obj);
		}

		public void CreateCombinedMesh()
		{
			List<Texture> textureList = new List<Texture>();
			List<List<Vector3>> verticesList = new List<List<Vector3>>();
			List<List<Vector2>> uvList = new List<List<Vector2>>();

			foreach (EnvObject obj in objects)
			{
				Texture t = obj.template.Texture;

				if (!textureList.Contains(t))
				{
					textureList.Add(t);
					verticesList.Add(new List<Vector3>());
					uvList.Add(new List<Vector2>());
				}

				int id = textureList.IndexOf(t);
				verticesList[id].AddRange(obj.mesh.Vertices3D);
				uvList[id].AddRange(obj.mesh.UV);
			}

			for (int i = 0; i < textureList.Count; i++)
			{
				Mesh mesh = new Mesh(PrimitiveType.Quads);
				mesh.Vertices3D = verticesList[i].ToArray();
				mesh.UV = uvList[i].ToArray();

				mesh.Texture = textureList[i];

				if (depth > 0f)
				{
					mesh.FillColor = true;
					mesh.Color = new TKTools.Color(1f * (depth / 50f), 1 * (depth / 50f), 1 * (depth / 50f));
				}

				combinedMeshes.Add(mesh);
			}
		}

		public void Draw()
		{
			foreach (Mesh m in combinedMeshes)
			{
				m.Draw();
			}
		}
	}
}