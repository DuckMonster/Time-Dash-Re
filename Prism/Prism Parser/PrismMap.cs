using OpenTK;
using System.Collections.Generic;
using System.Drawing;

namespace Prism.Parser
{
	public class PrismMap
	{
		internal readonly uint fileStructureVersion;

		PrismLayerNode rootNode = new PrismLayerNode("base", null);
		List<PrismTexture> textureList = new List<PrismTexture>();

		public PrismLayerNode RootLayerNode
		{
			get { return rootNode; }
			set { rootNode = value; }
		}
		public List<PrismTexture> Textures
		{
			get { return textureList; }
		}

		public PrismMap(uint fileStructureVersion)
		{
			this.fileStructureVersion = fileStructureVersion;
		}

		public void AddTexture(PrismTexture t)
		{
			textureList.Add(t);
		}
	}
}