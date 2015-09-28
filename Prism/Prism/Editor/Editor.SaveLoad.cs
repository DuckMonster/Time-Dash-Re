using OpenTK;
using Prism.Parser;
using System.Collections.Generic;
using System.Windows.Forms;

public partial class Editor
{
	string filename = null;
	public string Filename
	{
		get { return filename; }
	}

	public void Save()
	{
		PrismMap map = new PrismMap(FileStructureVersion);
		Dictionary<TextureSet.Tile, PrismTexture.Tile> tileList = new Dictionary<TextureSet.Tile, PrismTexture.Tile>();

		foreach (TextureSet s in textureSetList)
		{
			PrismTexture set = new PrismTexture(s.Name, s.Bitmap);
			foreach (TextureSet.Tile t in s)
			{
				PrismTexture.Tile tile = new PrismTexture.Tile(t.Name, t.UV, set);
				tileList.Add(t, tile);
				set.AddTile(tile);
			}

			map.AddTexture(set);
		}

		map.RootLayerNode = rootLayer.GetPrismNode(null, tileList);

		if (filename == null)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = "Prism Map (*.pm)|*.pm";

			if (dialog.ShowDialog() == DialogResult.OK)
				filename = dialog.FileName;
			else
				return;
		}

		PrismFile.SaveMapTo(map, filename);
	}
	public void LoadFrom(string filename)
	{
		this.filename = filename;
		
		PrismMap map = PrismFile.LoadMapFrom(filename);

		textureSetList.Clear();
		rootLayer.Nodes.Clear();

		Dictionary<PrismTexture.Tile, TextureSet.Tile> tileList = new Dictionary<PrismTexture.Tile, TextureSet.Tile>();

		foreach(PrismTexture t in map.Textures)
		{
			TextureSet set = new TextureSet(t.Name, t.Bitmap);
			foreach(PrismTexture.Tile tile in t.Tiles)
			{
				TextureSet.Tile newTile = new TextureSet.Tile(tile.Name, set, tile.UV);
				tileList.Add(tile, newTile);
				set.AddTile(newTile);
			}

			CreateTextureSet(set);
		}

		rootLayer = new LayerNode(map.RootLayerNode, tileList, this);
	}
}