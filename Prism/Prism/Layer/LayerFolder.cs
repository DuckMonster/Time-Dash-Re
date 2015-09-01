using System;

public class LayerFolder : LayerNode
{
	public LayerFolder(string name, Editor e)
		:base(name, e)
	{
	}

	public LayerFolder(LayerFolder copy)
		: base(copy)
	{ }

	public override void Logic()
	{
	}

	public override void Draw()
	{
	}

	public override LayerNode GetCopy()
	{
		return new LayerFolder(this);
	}

	public override string ToString()
	{
		string str = Name + " [";
		foreach (LayerNode n in Nodes)
			str += n.ToString() + ", ";

		if (Nodes.Count > 0)
			str = str.Substring(0, str.Length - 2);

		str += "]";
		return str;
	}
}