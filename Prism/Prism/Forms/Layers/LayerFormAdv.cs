using Aga.Controls.Tree;
using System.Drawing;
using System.Windows.Forms;

public partial class LayerFormAdv : EditorUIForm
{
	public class BaseNode : Node
	{
		global::LayerNode node;
		public global::LayerNode LayerNode
		{
			get { return node; }
		}

		public override string Text
		{
			get { return base.Text; }
			set
			{
				base.Text = value;
				node.Name = value;
			}
		}

		public BaseNode(string name, global::LayerNode node)
			: base(name)
		{
			this.node = node;
		}
	}

	public class FolderNode : BaseNode
	{
		public override bool IsLeaf
		{
			get { return false; }
		}

		public FolderNode(string name, LayerFolder folder) : base(name, folder)
		{
		}
	}
	public class LayerNode : BaseNode
	{
		public override bool IsLeaf
		{
			get { return true; }
		}

		public LayerNode(string name, Layer layer) : base(name, layer)
		{
		}
	}

	TreeModel model = new TreeModel();

	public LayerFormAdv()
	{
		InitializeComponent();
		layerTree.Model = model;

		newFolderItem.Click += (o, e) => CreateFolder();
		newLayerItem.Click += (o, e) => CreateLayer();
	}

	public void UpdateLayers()
	{

	}

	private void CreateFolder()
	{
		CreateNode(new FolderNode("Untitled Folder", new LayerFolder("Untitled Folder", Editor)));
	}

	private void CreateLayer()
	{
		CreateNode(new LayerNode("Untitled Layer", new Layer("Untitled Layer", Editor)));
	}

	private void CreateNode(BaseNode node)
	{
		layerTree.BeginUpdate();

		if (layerTree.SelectedNode != null)
		{
			Node n = layerTree.SelectedNode.Tag as Node;

			if (n is LayerNode)
				n.Parent.Nodes.Insert(n.Parent.Nodes.IndexOf(n) + 1, node);
			else
				n.Nodes.Insert(0, node);
		}
		else
			model.Nodes.Add(node);

		layerTree.EndUpdate();

		Node parent = node.Parent;
		int index = parent.Nodes.IndexOf(node);

		if (!(parent is BaseNode))
			node.LayerNode.Parent = Editor.rootLayer;
		else
			node.LayerNode.Parent = (parent as BaseNode).LayerNode;

		node.LayerNode.Index = index;
	}

	public void NodeClicked(object sender, TreeNodeAdvMouseEventArgs e)
	{
	}

	private void ItemDrag(object sender, ItemDragEventArgs e)
	{
		layerTree.DoDragDropSelectedNodes(DragDropEffects.Move);
	}

	private void ItemDragOver(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(typeof(TreeNodeAdv[])) && layerTree.DropPosition.Node != null)
		{
			TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
			TreeNodeAdv parent = layerTree.DropPosition.Node;
			if (layerTree.DropPosition.Position != NodePosition.Inside)
				parent = parent.Parent;

			foreach (TreeNodeAdv node in nodes)
				if (CheckParent(parent, node) ||
					(parent.IsLeaf && layerTree.DropPosition.Position == NodePosition.Inside))
				{
					e.Effect = DragDropEffects.None;
					return;
				}

			e.Effect = e.AllowedEffect;
		}
	}

	bool CheckParent(TreeNodeAdv parent, TreeNodeAdv node)
	{
		while (parent != null)
		{
			if (node == parent) return true;
			else parent = parent.Parent;
		}

		return false;
	}

	private void ItemDrop(object sender, DragEventArgs e)
	{
		layerTree.BeginUpdate();

		TreeNodeAdv[] nodes = (TreeNodeAdv[])e.Data.GetData(typeof(TreeNodeAdv[]));
		if (layerTree.DropPosition.Node != null)
		{
			Node dropNode = layerTree.DropPosition.Node.Tag as Node;

			if (layerTree.DropPosition.Position == NodePosition.Inside)
			{
				foreach (TreeNodeAdv node in nodes)
				{
					BaseNode item = node.Tag as BaseNode;

					item.Parent = dropNode;
					item.LayerNode.Parent = (dropNode as BaseNode).LayerNode;
				}

				layerTree.DropPosition.Node.Expand();
			}
			else
			{
				BaseNode parent = dropNode.Parent as BaseNode;
				if (layerTree.DropPosition.Position == NodePosition.After)
					dropNode = dropNode.NextNode;

				foreach (TreeNodeAdv node in nodes)
					(node.Tag as Node).Parent = null;

				int index = -1;
				index = parent.Nodes.IndexOf(dropNode);

				foreach (TreeNodeAdv node in nodes)
				{
					BaseNode item = node.Tag as BaseNode;

					if (index == -1)
					{
						parent.Nodes.Add(item);
						item.LayerNode.Parent = parent.LayerNode;
					}
					else
					{
						parent.Nodes.Insert(index, item);
						item.LayerNode.Parent = parent.LayerNode;
						item.LayerNode.Index = index;

						index++;
					}
				}
			}
		}
		else
		{
			foreach (TreeNodeAdv node in nodes)
				(node.Tag as Node).Parent = model.Root;
		}

		layerTree.EndUpdate();
	}

	private void MouseClicked(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			Point p = e.Location;
			p.X += 20;
			p.Y += 20;

			deleteItem.Enabled = layerTree.SelectedNode != null;

			rightClickMenu.Show(this, p);
		}
	}
}
