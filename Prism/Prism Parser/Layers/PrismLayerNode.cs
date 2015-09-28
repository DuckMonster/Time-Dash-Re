using System.Collections.Generic;
using System.IO;

namespace Prism.Parser
{
	public class PrismLayerNode
	{
		string name;
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		PrismLayerNode parent;
		public PrismLayerNode Parent
		{
			get { return parent; }
			set
			{
				if (parent != null)
					parent.Nodes.Remove(this);

				parent = value;
				if (parent != null)
					parent.Nodes.Add(this);
			}
		}

		List<PrismLayerNode> node = new List<PrismLayerNode>();
		public List<PrismLayerNode> Nodes
		{
			get { return node; }
		}

		public PrismLayerNode(string name, PrismLayerNode parent)
		{
			Name = name;
			Parent = parent;
		}

		public virtual void ReadFromBuffer(BinaryReader reader, PrismMap map)
		{
			int nodes = reader.ReadInt32();
			for (int i = 0; i < nodes; i++)
			{
				bool isLayer = reader.ReadBoolean();
				PrismLayerNode node;

				if (isLayer)
					node = new PrismLayer(reader.ReadString(), this);
				else
					node = new PrismLayerNode(reader.ReadString(), this);

				node.ReadFromBuffer(reader, map);
			}
		}

		public virtual void WriteToBuffer(BinaryWriter writer, PrismMap map)
		{
			if (parent != null)
			{
				writer.Write(false);
				writer.Write(name);
			}
			writer.Write(Nodes.Count);

			foreach (PrismLayerNode n in Nodes)
				n.WriteToBuffer(writer, map);
		}

		public override string ToString()
		{
			return name;
		}
	}
}