using System.Collections.Generic;
using System.Collections.ObjectModel;

public class LayerNode
{
	Editor editor;

	LayerNode parent;
	public LayerNode Parent
	{
		get { return parent; }
		set
		{
			if (parent != null)
				parent.Nodes.Remove(this);

			value.Nodes.Add(this);

			parent = value;
		}
	}

	Collection<LayerNode> nodes = new Collection<LayerNode>();
	public Collection<LayerNode> Nodes
	{
		get { return nodes; }
	}

	string name;
	public string Name
	{
		get { return name; }
		set { name = value; }
	}

	public IEnumerable<Layer> Layers
	{
		get
		{
			if (this is Layer)
				yield return (this as Layer);

			foreach (LayerNode n in nodes)
				foreach (Layer l in n.Layers)
					yield return l;
		}
	}

	public int Index
	{
		get
		{
			if (parent == null) return -1;

			return parent.nodes.IndexOf(this);
		}
		set
		{
			if (parent == null) return;

			parent.Nodes.Remove(this);
			parent.Nodes.Insert(value, this);
		}
	}

	public LayerNode(string name, Editor e)
	{
		this.editor = e;
		this.name = name;
	}

	public virtual void Logic() { }
	public virtual void Draw() { }

	public override string ToString()
	{
		string str = "{";
		foreach (LayerNode n in Nodes)
			str += n.ToString() + ", ";

		return str + "}";
	}
}