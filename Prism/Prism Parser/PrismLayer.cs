using System.Collections.Generic;

namespace Prism.Parser
{
	public class PrismLayer
	{
		string name;
		List<PrismMesh> meshList = new List<PrismMesh>();

		public string Name
		{
			get { return name; }
		}

		public List<PrismMesh> Meshes
		{
			get { return meshList; }
		}

		public PrismLayer(string name)
		{
			this.name = name;
		}

		public void AddMesh(PrismMesh m)
		{
			meshList.Add(m);
		}
	}
}