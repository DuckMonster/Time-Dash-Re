using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class TilePicker : Form
{
	public static TextureSet.Tile selectedTile;

	class TextureItem : ListViewItem
	{
		public TextureSet textureSet;
		List<TileItem> tileItems = new List<TileItem>();

		public List<TileItem> TileItems
		{
			get { return tileItems; }
		}

		public TextureItem(TextureSet s)
			:base(s.Name)
		{
			this.textureSet = s;
		}

		public void AddTileItem(TileItem t)
		{
			tileItems.Add(t);
			t.parent = this;
		}

		public void Update()
		{
			this.Text = textureSet.Name;
		}
	}
	class TileItem : ListViewItem
	{
		public TextureSet.Tile tile;
		public TextureItem parent;

		public TileItem(TextureSet.Tile t, int imageIndex)
			: base(t.Name)
		{
			this.tile = t;
			this.ImageIndex = imageIndex;
		}

		public void Update()
		{
			this.Text = tile.Name;
		}
	}

	int currentImageIndex = 0;

	public TilePicker()
	{
		InitializeComponent();
		tileList.LargeImageList = new ImageList();
		tileList.LargeImageList.ImageSize = new Size(86, 86);
	}

	TextureItem FindTextureItem(TextureSet s)
	{
		foreach (TextureItem t in textureList.Items)
			if (t.textureSet == s) return t;

		return null;
	}
	TileItem FindTileItem(TextureSet.Tile t)
	{
		foreach (TileItem ti in tileList.Items)
			if (ti.tile == t) return ti;

		return null;
	}

	public void AddTexture(TextureSet s)
	{
		textureList.Items.Add(new TextureItem(s));
	}

	public void RemoveTexture(TextureSet s)
	{
		ListViewItem t = FindTextureItem(s);
		textureList.Items.Remove(t);
	}

	public void AddTile(TextureSet.Tile t)
	{
		TextureItem ti = FindTextureItem(t.TextureSet);
		if (ti != null)
		{
			tileList.LargeImageList.Images.Add(t.TileBitmap);
			TileItem newItem = new TileItem(t, currentImageIndex);

			ti.TileItems.Add(newItem);
			if (ti.Selected)
				tileList.Items.Add(newItem);

			currentImageIndex++;
		}
	}

	public void RemoveTile(TextureSet.Tile t)
	{
		TileItem item = FindTileItem(t);

		item.parent.TileItems.Remove(item);
		if (tileList.Items.Contains(item))
			tileList.Items.Remove(item);
	}

	public void RecreateList()
	{
		tileList.Groups.Clear();
		tileList.LargeImageList.Images.Clear();
		currentImageIndex = 0;

		Editor e = Editor.CurrentEditor;

		foreach (TextureSet s in e.textureSetList)
		{
			AddTexture(s);
			foreach (TextureSet.Tile t in s)
				AddTile(t);
		}
	}

	public void UpdateList()
	{
		foreach (TextureItem t in textureList.Items)
		{
			t.Update();
			foreach (TileItem ti in t.TileItems)
				ti.Update();
		}
	}

	private void TextureSelected(object sender, EventArgs e)
	{
		foreach(TextureItem t in textureList.Items)
			t.Font = new Font(t.Font, t.Selected ? FontStyle.Bold : FontStyle.Regular);

		UpdateTileView();
	}

	private void TileSelected(object sender, EventArgs e)
	{
		if (tileList.SelectedItems.Count > 0)
			selectedTile = (tileList.SelectedItems[0] as TileItem).tile;
	}

	void UpdateTileView()
	{
		tileList.Items.Clear();

		foreach (TextureItem t in textureList.SelectedItems)
			foreach (TileItem ti in t.TileItems)
				if (ti.Text.Contains(filterBox.Text))
					tileList.Items.Add(ti);
	}

	private void FilterChanged(object sender, EventArgs e)
	{
		UpdateTileView();
	}

	private void OnClose(object sender, FormClosingEventArgs e)
	{
		this.Hide();
		e.Cancel = true;
	}
}