using OpenTK;
using System.Collections.Generic;
using System.Drawing;

namespace Prism.Parser
{
	public class PrismMap
	{
		internal readonly uint fileStructureVersion;

		List<PrismLayer> layerList = new List<PrismLayer>();
		List<PrismTexture> textureList = new List<PrismTexture>();

		public List<PrismLayer> Layers
		{
			get { return layerList; }
		}
		public List<PrismTexture> Textures
		{
			get { return textureList; }
		}

		public PrismMap(uint fileStructureVersion)
		{
			this.fileStructureVersion = fileStructureVersion;
		}

		public void AddLayer(PrismLayer l)
		{
			layerList.Add(l);
		}

		public void AddTexture(PrismTexture t)
		{
			textureList.Add(t);
		}
	}
}