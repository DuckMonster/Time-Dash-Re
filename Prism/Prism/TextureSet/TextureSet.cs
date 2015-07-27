using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TKTools;

public class TextureSet : IEnumerable, IDisposable
{
	public class TileEnum : IEnumerator
	{
		Tile[] tiles;
		int index = -1;

		public TileEnum(Tile[] tiles)
		{
			this.tiles = tiles;
		}

		public bool MoveNext()
		{
			index++;

			return index < tiles.Length;
		}

		public void Reset()
		{
			index = -1;
		}

		public object Current
		{
			get { return tiles[index]; }
		}
	}

	public class Tile : IDisposable, IEquatable<Tile>
	{
		string name;

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				Program.tilePicker.UpdateList();
			}
		}

		TextureSet set;
		RectangleF uv;
		float aspectRatio;

		Bitmap tileBitmap;

		public TextureSet TextureSet
		{
			get { return set; }
		}

		public Texture Texture
		{
			get { return set.Texture; }
		}

		public RectangleF UV
		{
			get { return uv; }
		}

		public RectangleF PreviewRectangleF
		{
			get
			{
				RectangleF rect = UV;
				rect.Y = 1 - rect.Y - rect.Height;

				return rect;
			}
		}

		public float AspectRatio
		{
			get { return aspectRatio; }
		}

		public Bitmap TileBitmap
		{
			get { return tileBitmap; }
		}

		public Tile(string name, TextureSet set, RectangleF uv)
		{
			this.name = name;
			this.set = set;
			this.uv = uv;
			this.aspectRatio = uv.Width / uv.Height;

			CreateTileBitmap();
		}

		void CreateTileBitmap()
		{
			Rectangle srcRect = GetPreviewRectangle(set.Bitmap.Size), destRect;
			int padding;

			if (aspectRatio >= 1)
			{
				tileBitmap = new Bitmap(srcRect.Width, (int)(srcRect.Height * aspectRatio));
				padding = (int)(((srcRect.Height * aspectRatio) - srcRect.Height) / 2);
				destRect = new Rectangle(0, padding, srcRect.Width, srcRect.Height);
            }
			else
			{
				tileBitmap = new Bitmap((int)(srcRect.Width / aspectRatio), srcRect.Height);
				padding = (int)(((srcRect.Width / aspectRatio) - srcRect.Width) / 2);
				destRect = new Rectangle(padding, 0, srcRect.Width, srcRect.Height);
			}

			Graphics g = Graphics.FromImage(tileBitmap);

			g.DrawImage(set.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);

			g.Dispose();
		}

		public void Dispose()
		{
			tileBitmap.Dispose();
			tileBitmap = null;
		}

		public Rectangle GetPreviewRectangle(Size size)
		{
			RectangleF rect = PreviewRectangleF;
			return new Rectangle(
				(int)(rect.X * size.Width),
				(int)(rect.Y * size.Height),
				(int)(rect.Width * size.Width),
				(int)(rect.Height * size.Height)
				);
		}

		public bool Equals(Tile t)
		{
			return t.TextureSet == TextureSet && t.uv == uv;
		}
	}

	List<Tile> tileList = new List<Tile>();

	string name;

	public string Name
	{
		get { return name; }
		set
		{
			name = value; Program.tilePicker.UpdateList();
		}
	}

	Bitmap bitmap;
	Texture texture;

	public Bitmap Bitmap
	{
		get { return bitmap; }
	}
	public Texture Texture
	{
		get { return texture; }
	}

	public List<Tile> Tiles
	{
		get { return tileList; }
	}

	public TextureSet(string name, Bitmap b)
	{
		this.name = name;

		bitmap = b;
		texture = new Texture();
		texture.UploadBitmap(b);
	}

	public void Dispose()
	{
		bitmap.Dispose();
		bitmap = null;

		foreach (Tile t in Tiles)
			t.Dispose();

		Tiles.Clear();
	}

	public void AddTile(string name, RectangleF uv) { AddTile(new Tile(name, this, uv)); }
	public void AddTile(Tile t)
	{
		tileList.Add(t);
		Program.tilePicker.AddTile(t);
	}

	public void RemoveTile(Tile t)
	{
		tileList.Remove(t);
		Program.tilePicker.RemoveTile(t);

		t.Dispose();
	}

	public IEnumerator GetEnumerator()
	{
		return new TileEnum(tileList.ToArray());
	}
}