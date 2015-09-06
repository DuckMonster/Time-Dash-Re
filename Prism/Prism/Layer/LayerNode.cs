using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class LayerNode : IDisposable
{
	protected Editor editor;

	LayerNode parent;
	public LayerNode Parent
	{
		get { return parent; }
		set
		{
			if (parent != null)
				parent.Nodes.Remove(this);

			if (value != null)
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

	bool visible = true;
	public bool Visible
	{
		get
		{
			if (parent != null)
				return parent.Visible && visible;
			else return visible;
		}
		set { visible = value; }
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
	public IEnumerable<Layer> LayersInverted
	{
		get
		{
			if (this is Layer)
				yield return (this as Layer);

			for (int i = nodes.Count - 1; i >= 0; i--)
				foreach (Layer l in nodes[i].LayersInverted)
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

			if (value == -1)
				parent.Nodes.Add(this);
			else
				parent.Nodes.Insert(value, this);
		}
	}

	public LayerNode(string name, Editor e)
	{
		this.editor = e;
		this.name = name;
	}
	public LayerNode(LayerNode copy)
	{
		editor = copy.editor;
		name = copy.name + " (copy)";

		foreach(LayerNode n in copy.Nodes)
		{
			LayerNode copyNode;

			if (n is Layer)
				copyNode = new Layer(n as Layer);
			else if (n is LayerFolder)
				copyNode = new LayerFolder(n as LayerFolder);
			else
				copyNode = new LayerNode(n);

			copyNode.SetParent(this, -1);
		}
	}

	public virtual void Dispose()
	{
		foreach (LayerNode n in Nodes)
			n.Dispose();
	}

	public void Remove()
	{
		Parent = null;
		Dispose();
	}

	public void SetParent(LayerNode node, int index)
	{
		Parent = node;
		Index = index;
	}

	public virtual void Logic() { }
	public virtual void Draw() { }

	public virtual LayerNode GetCopy()
	{
		return new LayerNode(this);
	}

	public override string ToString()
	{
		string str = "{";
		foreach (LayerNode n in Nodes)
			str += n.ToString() + ", ";

		return str + "}";
	}
}