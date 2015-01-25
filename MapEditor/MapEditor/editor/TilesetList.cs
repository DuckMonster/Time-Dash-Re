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
			bool tempFile = false;

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

			public Tileset(Texture t, string file, bool temp = false)
			{
				texture = t;
				filename = file;
				tempFile = temp;
			}

			public void Dispose()
			{
				texture.Dispose();
				if (tempFile)
					File.Delete(filename);
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

		public void LoadTileset(string path, bool temp = false)
		{
			Texture text = new Texture(path);
			tilesetList.Add(new Tileset(text, path, temp));
		}

		public void RemoveTileset(Tileset t)
		{
			if (tilesetList.Contains(t))
			{
				tilesetList.Remove(t);
				t.Dispose();
			}
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
			{
				//writer.Write(t.FileName);
				byte[] buffer;
				using (FileStream str = new FileStream(t.FileName, FileMode.Open))
				{
					buffer = new byte[str.Length];
					str.Read(buffer, 0, buffer.Length);
				}

				writer.Write(buffer.Length);
				writer.Write(buffer, 0, buffer.Length);
			}
		}

		public void ReadFromFile(BinaryReader reader)
		{
			int nmbr = reader.ReadInt32();
			for (int i = 0; i < nmbr; i++)
			{
				string newFile = "tmp" + i;

				using (FileStream str = new FileStream(newFile, FileMode.Create))
				{
					byte[] buffer = new byte[reader.ReadInt32()];
					reader.Read(buffer, 0, buffer.Length);

					str.Write(buffer, 0, buffer.Length);
				}

				LoadTileset(newFile, true);
			}
		}

		public Tileset this[int index]
		{
			get
			{
				if (index < 0 || index >= tilesetList.Count) return null;
				return tilesetList[index];
			}
		}
	}
}