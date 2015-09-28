using System.Collections.Generic;
using System.IO;

namespace Prism.Parser
{
	public class PrismLayer : PrismLayerNode
	{
		List<PrismMesh> meshList = new List<PrismMesh>();
		public List<PrismMesh> Meshes
		{
			get { return meshList; }
		}

		public PrismLayer(string name, PrismLayerNode parent)
			:base(name, parent)
		{
		}

		public void AddMesh(PrismMesh m)
		{
			meshList.Add(m);
		}

		public override void ReadFromBuffer(BinaryReader reader, PrismMap map)
		{
			int meshes = reader.ReadInt32();

			for(int i=0; i< meshes; i++)
			{
				PrismMesh mesh = new PrismMesh(reader, map);
				Meshes.Add(mesh);
			}
		}

		public override void WriteToBuffer(BinaryWriter writer, PrismMap map)
		{
			writer.Write(true);
			writer.Write(Name);
			writer.Write(Meshes.Count);

			foreach (PrismMesh m in Meshes)
				m.WriteToBuffer(writer, map);
		}
	}
}