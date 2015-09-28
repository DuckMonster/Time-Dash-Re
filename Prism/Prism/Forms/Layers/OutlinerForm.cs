using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class OutlinerForm : EditorUIForm
{
	class MeshNode : Node
	{
		Editor editor;
		EMesh mesh;

		public EMesh Mesh { get { return mesh; } }

		public override bool IsLeaf { get { return true; } }
		public string Name
		{
			get
			{
				if (mesh.Tile == null) return "[NO TILE]";
				else return mesh.Tile.Name;
			}
		}

		public bool Selected { get { return mesh.Selected; } }
		public bool Hovered { get { return mesh.Hovered; } }

		public MeshNode(EMesh m, Editor e)
			: base()
		{
			editor = e;
			mesh = m;
		}

		public void Select()
		{
			editor.SetSelected(new EMesh[] { mesh });
		}

		public void SetIndex(int index)
		{
			Node p = Parent;
			p.Nodes.Remove(this);

			if (index == -1 || index >= p.Nodes.Count)
				p.Nodes.Add(this);
			else
				p.Nodes.Insert(index, this);

			mesh.Index = index == -1 ? 0 : p.Nodes.Count - index - 1;
		}
	}

	TreeModel model;
	Font selectedFont;

	MeshNode hoveredNode = null;
	public EMesh HoveredNode
	{
		get
		{
			if (hoveredNode == null) return null;
			else return hoveredNode.Mesh;
		}
	}

	public OutlinerForm()
	{
		InitializeComponent();

		meshTree.ItemDrag += (o, e) => meshTree.DoDragDropSelectedNodes(DragDropEffects.Move);
		nodeControlName.DrawText += DrawText;

		model = new TreeModel();
		meshTree.Model = model;

		selectedFont = new Font(meshTree.Font.FontFamily, meshTree.Font.Size, FontStyle.Bold);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();

		meshTree.BeginUpdate();

		model.Nodes.Clear();

		if (Editor.ActiveLayers.Count > 0)
			for (int i = Editor.ActiveLayers[0].Meshes.Count - 1; i >= 0; i--)
				model.Nodes.Add(new MeshNode(Editor.ActiveLayers[0].Meshes[i], Editor));

		meshTree.EndUpdate();
	}

	public void UpdateSelected()
	{
		enableNodeSelect = false;

		meshTree.BeginUpdate();

		foreach (TreeNodeAdv n in meshTree.AllNodes)
			n.IsSelected = (n.Tag as MeshNode).Selected;

		meshTree.EndUpdate();

		enableNodeSelect = true;
	}

	protected void DrawText(object sender, DrawEventArgs e)
	{
		if ((e.Node.Tag as MeshNode).Selected)
			e.Font = selectedFont;
	}

	bool enableNodeSelect = true;
	protected void NodeSelected(object sender, EventArgs e)
	{
		if (!enableNodeSelect) return;

		List<EMesh> selected = new List<EMesh>(meshTree.SelectedNodes.Count);

		foreach (TreeNodeAdv tn in meshTree.SelectedNodes)
			selected.Add((tn.Tag as MeshNode).Mesh);

		Editor.SetSelected(selected);
	}

	protected void ItemDragOver(object sender, DragEventArgs e)
	{
		if (meshTree.DropPosition.Node == null)
		{
			e.Effect = e.AllowedEffect;
			return;
		}

		if (e.Data.GetDataPresent(typeof(TreeNodeAdv[])))
			e.Effect = e.AllowedEffect;
	}

	protected void ItemDrop(object sender, DragEventArgs e)
	{
		meshTree.BeginUpdate();

		TreeNodeAdv[] nodes = e.Data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];

		Node dropNode = meshTree.DropPosition.Node.Tag as Node;
		Node parent = dropNode.Parent;

		int index = dropNode.Index;
		if (meshTree.DropPosition.Position == NodePosition.After) index = parent.Nodes.IndexOf(dropNode.NextNode);

		foreach (TreeNodeAdv node in nodes)
		{
			int i = index;

			if (node.Index < index)
				i--;
			else if (index != -1)
				index++;

			(node.Tag as MeshNode).SetIndex(i);
		}

		meshTree.EndUpdate();

		Editor.DeselectAll();

		foreach (TreeNodeAdv n in nodes)
			(n.Tag as MeshNode).Mesh.Selected = true;
	}

	protected void MouseMoved(object sender, MouseEventArgs e)
	{
		TreeNodeAdv node = meshTree.GetNodeAt(e.Location);

		if (node != null)
			hoveredNode = node.Tag as MeshNode;
		else
			hoveredNode = null;
	}
}
