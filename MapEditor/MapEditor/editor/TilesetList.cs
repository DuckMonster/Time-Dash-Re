using System;
using System.Collections.Generic;
using TKTools;
using MapEditor;
using System.IO;
using System.Windows.Forms;

namespace MapEditor
{
	public class TilesetList : IDisposable
	{
		public class Tileset : IDisposable
		{
			string filename;
			Texture texture;

			public Texture Texture
			{
				get
				{
					return texture;
				}
			}

			public string FileName
			{
				get
				{
					return filename;
				}
			}

			public Tileset(Texture t, string file)
			{
				texture = t;
				filename = file;
			}

			public void Dispose()
			{
				texture.Dispose();
			}
		}

		public List<Tileset> tilesetList = new List<Tileset>();
		Editor editor;

		public int Count
		{
			get
			{
				return tilesetList.Count;
			}
		}

		public TilesetList(Editor e)
		{
			editor = e;
		}

		public void Dispose()
		{
			foreach (Tileset t in tilesetList)
				t.Dispose();

			tilesetList.Clear();
		}

		public void LoadTileset(string path)
		{
			Texture text = new Texture(path);
			tilesetList.Add(new Tileset(text, path));
		}

		public void PromptLoad()
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";
				dialog.Multiselect = true;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					string[] files = dialog.FileNames;
					foreach (string fn in files)
						LoadTileset(fn);
				}
			}
		}

		public void Logic()
		{
		}

		public void WriteToFile(BinaryWriter writer)
		{
			writer.Write(tilesetList.Count);
			foreach (Tileset t in tilesetList)
				writer.Write(t.FileName);
		}

		public void ReadFromFile(BinaryReader reader)
		{
			int nmbr = reader.ReadInt32();
			for (int i = 0; i < nmbr; i++)
				LoadTileset(reader.ReadString());
		}

		public Texture this[int index]
		{
			get
			{
				if (index < 0 || index >= tilesetList.Count) return null;
				return tilesetList[index].Texture;
			}
		}
	}
}