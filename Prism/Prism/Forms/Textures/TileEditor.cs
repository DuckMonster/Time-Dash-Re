using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class TileEditor : Form
{
	class TileItem : ListViewItem
	{
		TextureSet.Tile tile;
		public TextureSet.Tile Tile
		{
			get { return tile; }
		}

		public TileItem(TextureSet.Tile tile)
			:base(tile.Name)
		{
			this.tile = tile;
		}
	}
	class FillHelper
	{
		Bitmap bitmap;

		public FillHelper(Bitmap b)
		{
			bitmap = new Bitmap(b, new Size(b.Width, b.Height));
		}

		public void Clear()
		{
		}

		public RectangleF FillFromPoint(PointF startPoint)
		{
			int startx = (int)(startPoint.X * bitmap.Width), starty = (int)(startPoint.Y * bitmap.Height);

			BitmapData pixelData = bitmap.LockBits(
				new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			IntPtr ptr = pixelData.Scan0;
			int bytes = Math.Abs(pixelData.Stride) * bitmap.Height;

			byte[] argb = new byte[bytes];
			Marshal.Copy(ptr, argb, 0, bytes);

			bool found;

			int u, r, d, l;
			u = r = d = l = 0;

			do
			{
				found = false;

				if (FindPixelX(startx - l, startx + r, starty - u, argb))
				{
					u++;
					found = true;
				}

				if (FindPixelX(startx - l, startx + r, starty + d, argb))
				{
					d++;
					found = true;
				}

				if (FindPixelY(starty - u, starty + d, startx - l, argb))
				{
					l++;
					found = true;
				}

				if (FindPixelY(starty - u, starty + d, startx + r, argb))
				{
					r++;
					found = true;
				}
			} while (found);

			bitmap.UnlockBits(pixelData);

			return GetRectangle(starty - u, startx + r, starty + d, startx - l);
		}

		bool FindPixelX(int xstart, int xend, int y, byte[] colorArray)
		{
			for (int x = xstart; x <= xend; x++)
				if (GetPixelAlpha(x, y, colorArray) > 0f)
					return true;

			return false;
		}
		bool FindPixelY(int ystart, int yend, int x, byte[] colorArray)
		{
			for (int y = ystart; y <= yend; y++)
				if (GetPixelAlpha(x, y, colorArray) > 0f)
					return true;

			return false;
		}

		float GetPixelAlpha(int x, int y, byte[] colorArray)
		{
			if (x < 0 || x >= bitmap.Width || y < 0 || y >= bitmap.Height) return 0f;

			int index = (x + (bitmap.Width * y)) * 4 + 3;
			return colorArray[index];
		}

		RectangleF GetRectangle(float up, float right, float down, float left)
		{
			RectangleF rect = new RectangleF(left, up, right - left, down - up);

			rect.X /= bitmap.Width;
			rect.Y /= bitmap.Height;
			rect.Width /= bitmap.Width;
			rect.Height /= bitmap.Height;

			return rect;
		}
	}

	TextureForm.TextureItem item;
	FillHelper fillHelper;

	Rectangle filledRectangle = Rectangle.Empty;
	RectangleF filledUV = RectangleF.Empty;

	public TileEditor(TextureForm.TextureItem item)
	{
		InitializeComponent();

		this.item = item;
		fillHelper = new FillHelper(item.Bitmap);

		foreach (TextureSet.Tile t in item.TextureSet)
			tileList.Items.Add(new TileItem(t));
	}

	protected void AddButtonClicked(object caller, EventArgs a)
	{
		if (filledUV.Size.IsEmpty) return;
		TextureSet.Tile t = new TextureSet.Tile("Untitled", item.TextureSet, filledUV);

		item.TextureSet.AddTile(t);
		ListViewItem newItem = tileList.Items.Add(new TileItem(t));
		newItem.BeginEdit();
	}

	protected void RemoveButtonClicked(object caller, EventArgs a)
	{
		foreach(TileItem t in tileList.SelectedItems)
		{
			item.TextureSet.RemoveTile(t.Tile);
			tileList.Items.Remove(t);
		}
	}

	protected void TileNameChanged(object caller, LabelEditEventArgs a)
	{
		if (a.Label == null) return;

		TileItem item = (TileItem)tileList.Items[a.Item];
		TextureSet.Tile t = item.Tile;
		t.Name = a.Label;

		tileList.SelectedItems.Clear();
		item.Selected = true;
	}

	public void FindFill(PointF p)
	{
		uvBox.Cursor = Cursors.WaitCursor;

		fillHelper.Clear();
		RectangleF rec = fillHelper.FillFromPoint(p);

		uvBox.Cursor = Cursors.Hand;

		filledRectangle = new Rectangle(
			(int)(rec.X * uvBox.Size.Width),
			(int)(rec.Y * uvBox.Size.Height),
			(int)(rec.Width * uvBox.Size.Width),
			(int)(rec.Height * uvBox.Size.Height)
			);

		filledUV = new RectangleF(rec.X, 1 - rec.Y - rec.Height, rec.Width, rec.Height);

		addButton.Enabled = !filledRectangle.Size.IsEmpty;
	}

	public void UVBoxMouseMove(object sender, MouseEventArgs e)
	{
	}

	public void UVBoxMouseClick(object sender, MouseEventArgs e)
	{
		PointF p = e.Location;
		p.X /= uvBox.Size.Width;
		p.Y /= uvBox.Size.Height;

		FindFill(p);
		uvBox.Refresh();
	}

	public void UVBoxPaint(object sender, PaintEventArgs e)
	{
		Graphics g = e.Graphics;

		g.DrawImage(item.Bitmap, new Rectangle(new Point(0, 0), uvBox.Size));
		g.DrawRectangle(Pens.Red, filledRectangle);
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Close();
	}
}
