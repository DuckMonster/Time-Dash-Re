using OpenTK;
using System.Collections.Generic;
using System.IO;
using TKTools;

namespace Prism.Parser
{
	public class PrismMesh
	{
		Vector2[] vertexPosition;
		ColorHSL[] vertexColor;

		PrismTexture.Tile tile;

		public Vector2[] VertexPosition
		{
			get { return vertexPosition; }
			set { vertexPosition = value; }
		}
		public ColorHSL[] VertexColor
		{
			get { return vertexColor; }
			set { vertexColor = value; }
		}
		public PrismTexture.Tile Tile
		{
			get { return tile; }
		}

		public PrismMesh(IList<Vector2> vertexPosition, IList<ColorHSL> vertexColor, PrismTexture.Tile tile)
		{
			this.vertexPosition = new Vector2[vertexPosition.Count];
			this.vertexColor = new ColorHSL[vertexColor.Count];

			for (int i = 0; i < vertexPosition.Count; i++)
			{
				this.vertexPosition[i] = vertexPosition[i];
				this.vertexColor[i] = vertexColor[i];
			}

			this.tile = tile;
		}
		public PrismMesh(BinaryReader reader, PrismMap map)
		{
			ReadFrombuffer(reader, map);
		}

		public void ReadFrombuffer(BinaryReader reader, PrismMap map)
		{
			int vertices = reader.ReadInt32();
			vertexPosition = new Vector2[vertices];
			vertexColor = new ColorHSL[vertices];

			for(int i=0; i< vertices; i++)
			{
				vertexPosition[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
				vertexColor[i] = new ColorHSL(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
			}

			if (reader.ReadBoolean())
				tile = map.Textures[reader.ReadInt32()].Tiles[reader.ReadInt32()];
		}

		public void WriteToBuffer(BinaryWriter writer, PrismMap map)
		{
			writer.Write(vertexPosition.Length);

			for(int i=0; i<vertexPosition.Length; i++)
			{
				writer.Write(vertexPosition[i].X);
				writer.Write(vertexPosition[i].Y);
				writer.Write(vertexColor[i].H);
				writer.Write(vertexColor[i].S);
				writer.Write(vertexColor[i].L);
			}

			writer.Write(Tile != null);
			if (Tile != null)
			{
				writer.Write(map.Textures.IndexOf(Tile.Texture));
				writer.Write(Tile.Index);
			}
		}
	}
}