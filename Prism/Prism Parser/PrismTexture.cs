using System.Collections.Generic;
using System.Drawing;

namespace Prism.Parser
{
	public class PrismTexture
	{
		public class Tile
		{
			string name;
			public string Name
			{
				get { return name; }
			}

			RectangleF uv;
			public RectangleF UV
			{
				get { return uv; }
			}

			PrismTexture texture;
			public PrismTexture Texture
			{
				get { return texture; }
			}

			public int Index
			{
				get { return texture.Tiles.IndexOf(this); }
			}

			public Tile(string name, RectangleF uv, PrismTexture texture)
			{
				this.name = name;
				this.uv = uv;
				this.texture = texture;
			}
		}

		string name;
		List<Tile> tileList = new List<Tile>();
		Bitmap bitmap;

		public string Name
		{
			get { return name; }
		}

		public List<Tile> Tiles
		{
			get { return tileList; }
		}

		public Bitmap Bitmap
		{
			get { return bitmap; }
		}

		public PrismTexture(string name, Bitmap bitmap)
		{
			this.name = name;
			this.bitmap = bitmap;
		}

		public void AddTile(Tile t)
		{
			tileList.Add(t);
		}
	}
}