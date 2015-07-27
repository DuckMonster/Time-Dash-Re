using OpenTK;
using System.Collections.Generic;

namespace Prism.Parser
{
	public class PrismMesh
	{
		Vector2[] vertexPosition;
		PrismTexture.Tile tile;

		public Vector2[] VertexPosition
		{
			get { return vertexPosition; }
		}
		public PrismTexture.Tile Tile
		{
			get { return tile; }
		}

		public PrismMesh(IList<Vector2> vertexPosition, PrismTexture.Tile tile)
		{
			this.vertexPosition = new Vector2[vertexPosition.Count];

			for (int i = 0; i < vertexPosition.Count; i++)
				this.vertexPosition[i] = vertexPosition[i];

			this.tile = tile;
		}
	}
}