using OpenTK;
using System.Drawing;
using System.IO;

namespace Prism.Parser
{
	public class PrismFile
	{
		#region SAVING
		public static void SaveMapTo(PrismMap map, string filename)
		{
			using (BinaryWriter file = new BinaryWriter(new FileStream(filename, FileMode.Create)))
			{
				//WRITE FILE STRUCTURE VERSION !!!!!!!!!!!!!!!!!!
				file.Write(map.fileStructureVersion);
				//----------------------------------------------

				//SAVING STUFF
				SaveMapInfo(map, file);
				SaveTextures(map, file);
				SaveLayers(map, file);

				file.Flush();
			}
		}

		static void SaveMapInfo(PrismMap map, BinaryWriter file)
		{
			//file.Write(map.fileStructureVersion);
		}

		static void SaveTextures(PrismMap map, BinaryWriter file)
		{
			file.Write(map.Textures.Count);

			foreach(PrismTexture t in map.Textures)
			{
				file.Write(t.Name);

				byte[] imageData;

				ImageConverter converter = new ImageConverter();
				imageData = (byte[])converter.ConvertTo(t.Bitmap, typeof(byte[]));

				file.Write(imageData.Length);
				file.Write(imageData);

				file.Write(t.Tiles.Count);

				foreach(PrismTexture.Tile tile in t.Tiles)
				{
					file.Write(tile.Name);
					file.Write(tile.UV.X);
					file.Write(tile.UV.Y);
					file.Write(tile.UV.Width);
					file.Write(tile.UV.Height);
				}
			}
		}
		static void SaveLayers(PrismMap map, BinaryWriter file)
		{
			file.Write(map.Layers.Count);
			foreach(PrismLayer l in map.Layers)
			{
				file.Write(l.Name);
				file.Write(l.Meshes.Count);
				foreach(PrismMesh m in l.Meshes)
				{
					file.Write(m.VertexPosition.Length);

					for (int i = 0; i < m.VertexPosition.Length; i++)
					{
						file.Write(m.VertexPosition[i].X);
						file.Write(m.VertexPosition[i].Y);
					}

					if (m.Tile != null)
					{
						file.Write(true);

						PrismTexture tex = m.Tile.Texture;
						file.Write(map.Textures.IndexOf(tex));
						file.Write(tex.Tiles.IndexOf(m.Tile));
					}
					else file.Write(false);
				}
			}
		}
		#endregion

		#region LOADING
		public static PrismMap LoadMapFrom(string filename)
		{
			using (BinaryReader file = new BinaryReader(new FileStream(filename, FileMode.Open)))
			{
				PrismMap map = new PrismMap(file.ReadUInt32());
				
				LoadTextures(map, file);
				LoadLayers(map, file);

				return map;
			}
		}

		static void LoadTextures(PrismMap map, BinaryReader file)
		{
			#region VERSION 0+
			if (map.fileStructureVersion >= 0)
			{
				int n = file.ReadInt32();

				for(int i=0; i<n; i++)
				{
					string name = file.ReadString();

					int bitmapBytes = file.ReadInt32();
					Bitmap bitmap = new Bitmap(new MemoryStream(file.ReadBytes(bitmapBytes)));

					PrismTexture set = new PrismTexture(name, bitmap);

					int tileN = file.ReadInt32();
					for(int j=0; j<tileN; j++)
					{
						string tileName = file.ReadString();
						RectangleF rect = new RectangleF(file.ReadSingle(), file.ReadSingle(), file.ReadSingle(), file.ReadSingle());

						set.AddTile(new PrismTexture.Tile(tileName, rect, set));
					}

					map.AddTexture(set);
				}
			}
			#endregion
		}
		static void LoadLayers(PrismMap map, BinaryReader file)
		{
			#region VERSION 0+
			{
				int layerN = file.ReadInt32();
				for(int i=0; i<layerN; i++)
				{
					PrismLayer l = new PrismLayer(file.ReadString());
					int meshN = file.ReadInt32();

					for(int j=0; j<meshN; j++)
					{
						int vertexN = file.ReadInt32();
						Vector2[] vp = new Vector2[vertexN];

						for(int k=0; k<vertexN; k++)
							vp[k] = new Vector2(file.ReadSingle(), file.ReadSingle());

						PrismTexture.Tile tile = null;
						if (file.ReadBoolean())
						{
							int indexOfTexture = file.ReadInt32(),
								indexOfTile = file.ReadInt32();
							tile = map.Textures[indexOfTexture].Tiles[indexOfTile];
						}

						PrismMesh m = new PrismMesh(vp, tile);
						l.AddMesh(m);
					}

					map.AddLayer(l);
				}
			}
			#endregion
		}
		#endregion
	}
}