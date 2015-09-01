using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TreeNodes;

public partial class LayerFormAdv : EditorUIForm
{
	private static Font activeFont;

	internal TreeModel model;
	TreeNodeAdv activeNode;

	TreeLayerNode[] clipboard;

	public LayerFormAdv()
		: base()
	{
		InitializeComponent();

		nodeText.DrawText += DrawText;

		layerTree.ItemDrag += (o, e) => layerTree.DoDragDropSelectedNodes(DragDropEffects.Move);
		menuItemNewFolder.Click += (o, e) => AddItem(new TreeLayerNode(this, new LayerFolder("Untitled Folder", Editor)));
		menuItemNewLayer.Click += (o, e) => AddItem(new TreeLayerNode(this, new Layer("Untitled Layer", Editor)));
		menuItemDelete.Click += (o, e) => RemoveSelected();

		menuItemCopy.Click += (o, e) =>
		{
			clipboard = new TreeLayerNode[layerTree.SelectedNodes.Count];
			for (int i = 0; i < clipboard.Length; i++)
				clipboard[i] = (layerTree.SelectedNodes[i].Tag as TreeLayerNode);
		};

		menuItemPaste.Click += (o, e) =>
		{
			foreach (TreeLayerNode n in clipboard)
				AddItem(n.GetCopy());
		};

		model = new TreeModel();
		layerTree.Model = model;

		activeFont = new Font(layerTree.Font.FontFamily, layerTree.Font.Size, FontStyle.Bold);
	}

	protected void DrawText(object o, DrawEventArgs e)
	{
		if (activeNode == e.Node)
		{
			e.Font = activeFont;
		}
	}

	void GenerateSample()
	{
		layerTree.BeginUpdate();

		for (int i = 0; i < 2; i++)
		{
			TreeLayerNode node = new TreeLayerNode(this, new LayerFolder("Folder " + i, Editor));
			node.SetParent(model.Root);

			for (int j = 0; j < 3; j++)
			{
				TreeLayerNode layer = new TreeLayerNode(this, new Layer("Layer " + i + "." + j, Editor));
				layer.SetParent(node);
			}
		}

		layerTree.EndUpdate();
	}

	
	void AddItem(TreeLayerNode node)
	{
		layerTree.BeginUpdate();

		if (layerTree.SelectedNode != null)
		{
			Node n = layerTree.SelectedNode.Tag as Node;

			if (n.IsLeaf)
				node.SetParent(n.Parent, n.Index);
			else
				node.SetParent(n);

			layerTree.SelectedNode.Expand();
		}
		else
			node.SetParent(model.Root);

		layerTree.EndUpdate();
	}

	void RemoveSelected()
	{
		layerTree.BeginUpdate();

		TreeNodeAdv[] removedLayers = new TreeNodeAdv[layerTree.SelectedNodes.Count];
		layerTree.SelectedNodes.CopyTo(removedLayers, 0);

		if (removedLayers.Length > 0)
			if (MessageBox.Show("Are you sure you want to delete " + removedLayers.Length + " layers?", "Are you sure?", MessageBoxButtons.YesNo) == DialogResult.No)
				return;

		foreach(TreeNodeAdv layer in removedLayers)
		{
			TreeLayerNode n = layer.Tag as TreeLayerNode;
			n.LayerNode.Remove();
			n.Parent = null;
		}

		layerTree.EndUpdate();
	}

	protected void ItemDoubleClicked(object sender, TreeNodeAdvMouseEventArgs e)
	{
		if (!e.Node.IsLeaf)
			return;

		activeNode = e.Node;

		Editor.SetActiveLayers(new Layer[] { (e.Node.Tag as TreeLayerNode).LayerNode as Layer });
	}

	protected void MousePressed(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
			OpenRightClickMenu(e.Location);
	}

	protected void KeyboardPress(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete)
			RemoveSelected();
	}

	void OpenRightClickMenu(Point point)
	{
		menuItemDelete.Enabled = menuItemCopy.Enabled = layerTree.SelectedNode != null;
		menuItemPaste.Enabled = clipboard != null;

		rightClickMenu.Show(this, point);
	}

	protected void ItemDragOver(object sender, DragEventArgs e)
	{
		debugLabel.Text = layerTree.DropPosition.Node == null ? "NULL" : (layerTree.DropPosition.Node.Tag as TreeLayerNode).ToString();

		if (layerTree.DropPosition.Node == null)
		{
			e.Effect = e.AllowedEffect;
			return;
		}

		if (e.Data.GetDataPresent(typeof(TreeNodeAdv[])))
		{
			TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
			TreeNodeAdv dropNode = layerTree.DropPosition.Node;

			foreach (TreeNodeAdv treeNode in nodes)
			{
				TreeLayerNode node = treeNode.Tag as TreeLayerNode;
				if (node.IsParentTo(dropNode.Tag as Node)
					|| (dropNode.IsLeaf && layerTree.DropPosition.Position == NodePosition.Inside))
				{
					e.Effect = DragDropEffects.None;
					return;
				}
			}

			e.Effect = e.AllowedEffect;
		}
	}

	protected void ItemDrop(object sender, DragEventArgs e)
	{
		layerTree.BeginUpdate();

		TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];

		if (layerTree.DropPosition.Node == null)
		{
			foreach (TreeNodeAdv node in nodes)
				(node.Tag as TreeLayerNode).SetParent(model.Root);

			return;
		}

		Node dropNode = layerTree.DropPosition.Node.Tag as Node;

		if (layerTree.DropPosition.Position == NodePosition.Inside)
		{
			foreach (TreeNodeAdv node in nodes)
				(node.Tag as TreeLayerNode).SetParent(dropNode);

			layerTree.DropPosition.Node.Expand();
		}
		else
		{
			Node parent = dropNode.Parent;

			int index = dropNode.Index;
			if (layerTree.DropPosition.Position == NodePosition.After) index = parent.Nodes.IndexOf(dropNode.NextNode);

			foreach (TreeNodeAdv node in nodes)
			{
				if (index == -1)
					(node.Tag as TreeLayerNode).SetParent(parent);
				else
				{
					(node.Tag as TreeLayerNode).SetParent(parent, index);
					index++;
				}
			}
		}

		layerTree.EndUpdate();
		layerTree.Update();
	}
}

namespace TreeNodes
{
	public class TreeLayerNode : Node
	{
		LayerNode layerNode;
		public LayerNode LayerNode
		{
			get { return layerNode; }
		}

		public IEnumerable<Layer> Layers
		{
			get
			{
				foreach (Layer l in layerNode.Layers)
					yield return l;
			}
		}

		public override string Text
		{
			get { return layerNode.Name; }
			set { layerNode.Name = value; }
		}

		public override bool IsLeaf
		{
			get { return (layerNode is Layer); }
		}

		LayerFormAdv form;

		public TreeLayerNode(LayerFormAdv form, LayerNode node)
			: base(node.Name)
		{
			this.form = form;
			this.layerNode = node;
		}

		public bool IsParentTo(Node n)
		{
			Node parent = n;

			while (parent != null)
			{
				if (parent == this)
					return true;
				else
					parent = parent.Parent;
			}

			return false;
		}

		public void SetParent(Node node) { SetParent(node, -1); }
		public void SetParent(Node node, int index)
		{
			Parent = null;

			if (index == -1)
				node.Nodes.Add(this);
			else
				node.Nodes.Insert(index, this);

			if (node is TreeLayerNode)
				layerNode.SetParent((node as TreeLayerNode).LayerNode, index);
			else
				layerNode.SetParent(form.Editor.rootLayer, index);
		}

		public TreeLayerNode GetCopy()
		{
			TreeLayerNode copy = new TreeLayerNode(form, layerNode.GetCopy());

			foreach(TreeLayerNode node in Nodes)
			{
				TreeLayerNode newNode = node.GetCopy();
				copy.Nodes.Add(newNode);
			}

			return copy;
		}
	}
}