using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

public partial class TilePicker : EditorUIControl
{
	TextureSet.Tile selectedTile;
	public TextureSet.Tile SelectedTile
	{
		get { return selectedTile; }
	}

	class TextureItem : ListViewItem
	{
		public TextureSet textureSet;
		List<TileItem> tileItems = new List<TileItem>();

		public List<TileItem> TileItems
		{
			get { return tileItems; }
		}

		public TextureItem(TextureSet s)
			: base(s.Name)
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
			this.ImageKey = imageIndex.ToString();
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

	public override void UpdateUI()
	{
		Stopwatch watch = Stopwatch.StartNew();
		RecreateList();
		watch.Stop();

		DebugForm.debugString = "Recreating list took " + watch.ElapsedMilliseconds + " milliseconds!";
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

	TextureItem AddTexture(TextureSet s)
	{
		return textureList.Items.Add(new TextureItem(s)) as TextureItem;
	}

	public void RemoveTexture(TextureSet s) { RemoveTexture(FindTextureItem(s)); }
	void RemoveTexture(TextureItem t)
	{
		textureList.Items.Remove(t);

		TileItem[] buff = t.TileItems.ToArray();

		foreach (TileItem item in buff)
			RemoveTile(item);
	}

	public void AddTile(TextureSet.Tile t)
	{
		TextureItem ti = FindTextureItem(t.TextureSet);
		if (ti != null)
		{
			tileList.LargeImageList.Images.Add(currentImageIndex.ToString(), t.TileBitmap);
			TileItem newItem = new TileItem(t, currentImageIndex);

			ti.AddTileItem(newItem);
			if (ti.Selected)
				tileList.Items.Add(newItem);

			currentImageIndex++;
		}
	}

	public void RemoveTile(TextureSet.Tile t) { RemoveTile(FindTileItem(t)); }
	void RemoveTile(TileItem item)
	{
		if (item == null) return;

		item.parent.TileItems.Remove(item);
		if (tileList.Items.Contains(item))
			tileList.Items.Remove(item);

		tileList.LargeImageList.Images.RemoveByKey(item.ImageKey);
	}

	public void RecreateList()
	{
		Editor e = Editor;

		foreach (TextureItem tex in textureList.Items)
		{
			if (!e.textureSetList.Contains(tex.textureSet))
				RemoveTexture(tex);
		}

		foreach (TextureSet s in e.textureSetList)
		{
			if (FindTextureItem(s) == null) AddTexture(s);

			foreach (TextureSet.Tile t in s)
				if (FindTileItem(t) == null) AddTile(t);
		}

		UpdateList();
	}

	public void UpdateList()
	{
		foreach (TextureItem t in textureList.Items)
		{
			t.Update();
			foreach (TileItem ti in t.TileItems)
				ti.Update();
		}

		UpdateTileView();
	}

	private void TextureSelected(object sender, EventArgs e)
	{
		foreach (TextureItem t in textureList.Items)
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

		ICollection collection;

		if (textureList.SelectedItems.Count > 0)
			collection = textureList.SelectedItems;
		else
			collection = textureList.Items;


		foreach (TextureItem t in collection)
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