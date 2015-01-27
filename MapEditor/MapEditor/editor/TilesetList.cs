using System;
using System.Collections.Generic;
using TKTools;
using MapEditor;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MapEditor
{
	public class TilesetList : IDisposable
	{
		public class Tileset : IDisposable
		{
			public List<Template> references = new List<Template>();
			bool[,] opaquePixels;

			string filename;
			Texture texture;
			bool tempFile = false;
			int width, height;

			public int Width
			{
				get
				{
					return width;
				}
			}

			public int Height
			{
				get
				{
					return height;
				}
			}

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

				CreateOpaqueMap(file);
			}

			public void CreateOpaqueMap(string file)
			{
				using (Bitmap bmp = new Bitmap(file))
				{
					width = bmp.Width;
					height = bmp.Height;

					// Lock the bitmap's bits.  
					Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
					BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

					// Get the address of the first line.
					IntPtr ptr = bmpData.Scan0;

					// Declare an array to hold the bytes of the bitmap.
					int bytes = bmpData.Stride * bmp.Height;
					byte[] argbValues = new byte[bytes];

					// Copy the RGB values into the array.
					Marshal.Copy(ptr, argbValues, 0, bytes);

					opaquePixels = new bool[width, height];

					for (int x = 0; x < width; x++)
						for (int y = 0; y < height; y++)
						{
							int a = argbValues[(x * 4 + 3) + (width * 4 * y)];
							opaquePixels[x, y] = a > 0;
						}

					bmp.UnlockBits(bmpData);
				}
			}

			public void Dispose()
			{
				texture.Dispose();
				if (tempFile)
					File.Delete(filename);
			}

			public void GetOpaqueOffset(RectangleF source, out float left, out float right, out float up, out float down)
			{
				bool l = false, r = false, u = false, d = false;
				left = 0;
				right = 0;
				up = 0;
				down = 0;

				int srcX = (int)(Math.Floor(source.X * width));
				int srcY = (int)(Math.Floor(source.Y * height));
				int srcWidth = (int)(Math.Ceiling(source.Width * width));
				int srcHeight = (int)(Math.Ceiling(source.Height * height));

				for (int y = 0; y < srcHeight; y++)
				{
					if (u) break;

					for (int x = 0; x < srcWidth; x++)
					{
						if (!u && opaquePixels[srcX + x, srcY + y])
						{
							up = (float)y / height;
							u = true;
							break;
						}
					}
				}

				for (int y = srcHeight-1; y >= 0; y--)
				{
					if (d) break;

					for (int x = 0; x < srcWidth; x++)
					{
						if (!d && opaquePixels[srcX + x, srcY + y])
						{
							down = (float)(srcHeight - y) / height;
							d = true;
							break;
						}
					}
				}

				for (int x = srcWidth - 1; x >= 0; x--)
				{
					if (r) break;

					for (int y = 0; y < srcHeight; y++)
					{
						if (!r && opaquePixels[srcX + x, srcY + y])
						{
							right = (float)(srcWidth - x) / width;
							r = true;
							break;
						}
					}
				}

				for (int x = 0; x < srcWidth; x++)
				{
					if (l) break;

					for (int y = 0; y < srcHeight; y++)
					{
						if (!l && opaquePixels[srcX + x, srcY + y])
						{
							left = (float)x / width;
							l = true;
							break;
						}
					}
				}
			}

			public void LoadNewFile()
			{
				using (OpenFileDialog dialog = new OpenFileDialog())
				{
					dialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg";

					if (dialog.ShowDialog() == DialogResult.OK)
					{
						Dispose();

						filename = dialog.FileName;
						texture = new Texture(filename);
						CreateOpaqueMap(filename);
					}
				}
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

				foreach (Template temp in t.references)
				{
					editor.templateMenu.RemoveTemplate(temp);
					editor.DeleteTemplate(temp);
				}
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