using System;
using System.Drawing;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

public class Environment
{
	public static float TILE_SIZE = 1.0f;

	enum TileType
	{
		Empty,
		Solid
	}

	class Tile
	{
		Environment environment;
		int x, y;

		public int X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}
		public int Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}
		public Vector2 World
		{
			get
			{
				return new Vector2(x * Environment.TILE_SIZE, y * Environment.TILE_SIZE);
			}
			set
			{
				x = (int)(value.X / Environment.TILE_SIZE);
				y = (int)(value.Y / Environment.TILE_SIZE);
			}
		}

		public int Index
		{
			get
			{
				return x + (y * environment.width);
			}
			set
			{
				x = value % environment.width;
				y = value / environment.width;
			}
		}

		public bool Collision
		{
			get
			{
				TileType type = Type;
				return type == TileType.Solid;
			}
		}

		public TileType Type
		{
			get
			{
				if (Index < 0 || Index >= environment.tileList.Length) return TileType.Solid;
				else return environment.tileList[Index];
			}
			set
			{
				if (Index >= 0 || Index < environment.tileList.Length)
					environment.tileList[Index] = value;
			}
		}

		public Tile(int index, Environment env)
		{
			environment = env;

			Index = index;
		}
		public Tile(int x, int y, Environment env)
		{
			environment = env;

			X = x;
			Y = y;
		}
		public Tile(Vector2 pos, Environment env)
		{
			environment = env;

			World = pos;
		}

		public static bool operator true(Tile t)
		{
			return t.Collision;
		}
		public static bool operator false(Tile t)
		{
			return !t.Collision;
		}
	}

	Map map;
	TileType[] tileList;
	int width, height;

	public Environment(Map m)
	{
		map = m;
		LoadMap();
	}

	void LoadMap()
	{
		using (Bitmap bmp = (Bitmap)Image.FromFile("Res/map.png"))
		{
			width = bmp.Width;
			height = bmp.Height;

			tileList = new TileType[width * height];

			List<Vector2> vertexList = new List<Vector2>();
			List<Vector2> vertexUVList = new List<Vector2>();

			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
				{
					uint data = (uint)bmp.GetPixel(x, height - 1 - y).ToArgb();
					Tile tile = new Tile(x, y, this);

					if (data == 0xFF000000)
						tile.Type = TileType.Solid;
					else 
						tile.Type = TileType.Empty;
				}
		}
	}

	public bool GetCollision(Entity e) { return GetCollision(e.position, e.size); }
	public bool GetCollision(Vector2 pos, Vector2 size)
	{
		Vector2 sizex = new Vector2(size.X / 2, 0),
			sizey = new Vector2(0, size.Y / 2);

		if (new Tile(pos + sizex + sizey, this)) return true;
		if (new Tile(pos + sizex - sizey, this)) return true;
		if (new Tile(pos - sizex + sizey, this)) return true;
		if (new Tile(pos - sizex - sizey, this)) return true;
		if (new Tile(pos + sizex, this)) return true;
		if (new Tile(pos - sizex, this)) return true;

		return false;
	}

	public void Logic()
	{
	}
}