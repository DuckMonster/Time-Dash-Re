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
			PrismLayerNode node = map.RootLayerNode;
			node.WriteToBuffer(file, map);
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
				map.RootLayerNode.ReadFromBuffer(file, map);
			}
			#endregion
		}
		#endregion
	}
}