using OpenTK;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TKTools;
using System;
using TKTools.AStar;

namespace Scene
{
	public class Scene : IDisposable
	{
		Map map;
		public List<EnvTileset> tilesetList = new List<EnvTileset>();
		public List<EnvTemplate> templateList = new List<EnvTemplate>();
		public List<EnvSolid> solidList = new List<EnvSolid>();
		public List<EnvObject> objectList = new List<EnvObject>();
		public List<EnvEvent> eventList = new List<EnvEvent>();

		public float width = 40f, height = 40;
		public Vector2 originOffset;

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

		}

		public void LoadMap(string filename)
		{
			using (BinaryReader reader = new BinaryReader(new FileStream("Maps/" + filename, FileMode.Open)))
			{
				reader.ReadString();
				reader.ReadInt32();

				if (reader.ReadBoolean())
				{
					byte[] buffer = new byte[reader.ReadInt32()];
					reader.Read(buffer, 0, buffer.Length);
				}

				int nmbrOfTilesets = reader.ReadInt32();
				for (int i = 0; i < nmbrOfTilesets; i++)
					tilesetList.Add(new EnvTileset(reader));

				int nmbrOfTemplates = reader.ReadInt32();
				for (int i = 0; i < nmbrOfTemplates; i++)
					templateList.Add(new EnvTemplate(reader, this));

				int nmbrOfEvents = reader.ReadInt32();
				for(int i=0; i<nmbrOfEvents; i++)
				{
					eventList.Add(new EnvEvent(
						reader.ReadInt32(),
						reader.ReadString(),
						new TKTools.Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
						));
				}

				int nmbrOfLayers = reader.ReadInt32();
				for (int i = 0; i < nmbrOfLayers; i++)
				{
					float depth = 0;

					if (i == 0)
					{
						int nmbrOfObjects = reader.ReadInt32();

						for (int j = 0; j < nmbrOfObjects; j++)
						{
							bool isEvent = reader.ReadBoolean();

							if (!isEvent)
								solidList.Add(new EnvSolid(reader, this));
							else
							{
								int id = reader.ReadInt32();
								int argNmbr = reader.ReadInt32();

								int[] argList = new int[argNmbr];
								for (int a = 0; a < argList.Length; a++)
									argList[a] = reader.ReadInt32();

								Polygon p = new Polygon(new Vector2[] {
								new Vector2(reader.ReadSingle(), reader.ReadSingle()),
								new Vector2(reader.ReadSingle(), reader.ReadSingle()),
								new Vector2(reader.ReadSingle(), reader.ReadSingle()),
								new Vector2(reader.ReadSingle(), reader.ReadSingle())
							});

								if (id != -1)
									map.SceneEvent(eventList.Find((x) => x.ID == id), argList, p);
							}
						}
					}
					else
					{
						depth = reader.ReadSingle();

						int nmbrOfObjects = reader.ReadInt32();

						for (int j = 0; j < nmbrOfObjects; j++)
							objectList.Add(new EnvObject(reader, depth, this));
					}
				}

				Polygon combinedPoly = new Polygon();
				List<Polygon> solids = new List<Polygon>();
				foreach (EnvSolid solid in solidList)
				{
					combinedPoly.AddPoint(solid.polygon);
					solids.Add(solid.polygon);
				}

				RectangleF rect = combinedPoly.Bounds;

				originOffset = new Vector2(rect.X, rect.Y) + new Vector2(rect.Width / 2, rect.Height / 2);

				width = rect.Width;
				height = rect.Height;

				AStar.CreateBuffer(solids, 0.5f);
			}
		}

		public bool GetCollision(Vector2 pos, Vector2 size)
		{
			Vector2 sizex = new Vector2(size.X / 2, 0);
			Vector2 sizey = new Vector2(0, size.Y / 2);

			Polygon p = new Polygon(new Vector2[] {
				pos + sizex + sizey,
				pos - sizex + sizey,
				pos - sizex - sizey,
				pos + sizex - sizey
			});

			foreach (EnvSolid solid in solidList)
				if (solid.GetCollision(p)) return true;

			return false;
		}

		public bool GetCollisionFast(Vector2 pos, Vector2 size)
		{
			Vector2 sizex = new Vector2(size.X / 2, 0);
			Vector2 sizey = new Vector2(0, size.Y / 2);

			pos = pos - sizex - sizey;

			RectangleF pbounds = new RectangleF(pos.X, pos.Y, size.X, size.Y);

			foreach (EnvSolid solid in solidList)
				if (solid.GetCollisionFast(pbounds)) return true;

			return false;
		}

		public void Logic()
		{
		}
	}

	public class EnvObject
	{
		Scene scene;
		public Polygon polygon;

		float depth;

		public EnvObject(BinaryReader reader, float depth, Scene s)
		{
			scene = s;
			this.depth = depth;

			EnvTemplate t = s.templateList[reader.ReadInt32()];

			polygon = new Polygon(new Vector2[] {
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle())
			});
		}
	}

	public class EnvSolid
	{
		Scene scene;
		public Polygon polygon;
		RectangleF rectangle;

		public EnvSolid(BinaryReader reader, Scene s)
		{
			scene = s;
			polygon = new Polygon(new Vector2[] {
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle()),
				new Vector2(reader.ReadSingle(), reader.ReadSingle())
			});

			rectangle = polygon.Bounds;
		}

		public bool GetCollision(Polygon p)
		{
			return p.Intersects(polygon);
		}

		public bool GetCollisionFast(RectangleF pb)
		{
			return (pb.X + pb.Width >= rectangle.X &&
				pb.X < rectangle.X + rectangle.Width &&
				pb.Y + pb.Height >= rectangle.Y &&
				pb.Y < rectangle.Y + rectangle.Height);
		}
	}

	public class EnvTemplate
	{
		Scene scene;
		int tilesetIndex;
		RectangleF uv;

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
	}

	public class EnvEvent
	{
		int id;
		string name;
		TKTools.Color color;

		public int ID
		{
			get { return id; }
		}

		public string Name
		{
			get { return name; }
		}

		public TKTools.Color Color
		{
			get { return color; }
		}

		public EnvEvent(int id, string name, TKTools.Color color)
		{
			this.id = id;
			this.name = name;
			this.color = color;
		}
	}

	public class EnvTileset
	{
		public EnvTileset(BinaryReader reader)
		{
			using (FileStream str = new FileStream("temp", FileMode.Create))
			{
				byte[] buffer = new byte[reader.ReadInt32()];
				reader.Read(buffer, 0, buffer.Length);

				str.Write(buffer, 0, buffer.Length);
			}

			File.Delete("temp");
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