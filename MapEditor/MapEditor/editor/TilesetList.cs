using System;
using System.Collections.Generic;
using TKTools;
using MapEditor;
using System.IO;

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
		List<string> tilesetLoadBuffer = new List<string>();
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

		void LoadTilesetBuffer()
		{
			while (tilesetLoadBuffer.Count > 0)
			{
				LoadTilesetFromBuffer(tilesetLoadBuffer[0]);
				tilesetLoadBuffer.RemoveAt(0);
			}
		}

		void LoadTilesetFromBuffer(string path)
		{
			Texture text = new Texture(path);
			tilesetList.Add(new Tileset(text, path));
		}

		public void LoadTileset(string path)
		{
			if (!File.Exists(path)) return;
			tilesetLoadBuffer.Add(path);
		}

		public void Logic()
		{
			LoadTilesetBuffer();
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