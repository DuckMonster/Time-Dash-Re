partial class LayerFormAdv
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
			this.components = new System.ComponentModel.Container();
			this.layerTree = new Aga.Controls.Tree.TreeViewAdv();
			this.nodeStateIcon = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
			this.nodeText = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.newFolderItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newLayerItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rightClickMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// layerTree
			// 
			this.layerTree.AllowDrop = true;
			this.layerTree.BackColor = System.Drawing.SystemColors.Window;
			this.layerTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.layerTree.DefaultToolTipProvider = null;
			this.layerTree.DisplayDraggingNodes = true;
			this.layerTree.DragDropMarkColor = System.Drawing.Color.Black;
			this.layerTree.LineColor = System.Drawing.SystemColors.Control;
			this.layerTree.LoadOnDemand = true;
			this.layerTree.Location = new System.Drawing.Point(12, 12);
			this.layerTree.Model = null;
			this.layerTree.Name = "layerTree";
			this.layerTree.NodeControls.Add(this.nodeStateIcon);
			this.layerTree.NodeControls.Add(this.nodeText);
			this.layerTree.SelectedNode = null;
			this.layerTree.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
			this.layerTree.Size = new System.Drawing.Size(336, 387);
			this.layerTree.TabIndex = 0;
			this.layerTree.Text = "treeViewAdv1";
			this.layerTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.ItemDrag);
			this.layerTree.NodeMouseClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.NodeClicked);
			this.layerTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.ItemDrop);
			this.layerTree.DragOver += new System.Windows.Forms.DragEventHandler(this.ItemDragOver);
			this.layerTree.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MouseClicked);
			// 
			// nodeStateIcon
			// 
			this.nodeStateIcon.LeftMargin = 1;
			this.nodeStateIcon.ParentColumn = null;
			this.nodeStateIcon.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
			// 
			// nodeText
			// 
			this.nodeText.DataPropertyName = "Text";
			this.nodeText.EditEnabled = true;
			this.nodeText.IncrementalSearchEnabled = true;
			this.nodeText.LeftMargin = 3;
			this.nodeText.ParentColumn = null;
			// 
			// rightClickMenu
			// 
			this.rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newFolderItem,
            this.newLayerItem,
            this.toolStripSeparator1,
            this.deleteItem});
			this.rightClickMenu.Name = "rightClickMenu";
			this.rightClickMenu.Size = new System.Drawing.Size(153, 98);
			// 
			// newFolderItem
			// 
			this.newFolderItem.Name = "newFolderItem";
			this.newFolderItem.Size = new System.Drawing.Size(152, 22);
			this.newFolderItem.Text = "New Folder";
			// 
			// newLayerItem
			// 
			this.newLayerItem.Name = "newLayerItem";
			this.newLayerItem.Size = new System.Drawing.Size(152, 22);
			this.newLayerItem.Text = "New Layer";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
			// 
			// deleteItem
			// 
			this.deleteItem.Name = "deleteItem";
			this.deleteItem.Size = new System.Drawing.Size(152, 22);
			this.deleteItem.Text = "Delete";
			// 
			// LayerFormAdv
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(360, 411);
			this.Controls.Add(this.layerTree);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LayerFormAdv";
			this.ShowIcon = false;
			this.Text = "Layers";
			this.rightClickMenu.ResumeLayout(false);
			this.ResumeLayout(false);

	}

	#endregion

	private Aga.Controls.Tree.TreeViewAdv layerTree;
	private Aga.Controls.Tree.NodeControls.NodeTextBox nodeText;
	private Aga.Controls.Tree.NodeControls.NodeStateIcon nodeStateIcon;
	private System.Windows.Forms.ContextMenuStrip rightClickMenu;
	private System.Windows.Forms.ToolStripMenuItem newFolderItem;
	private System.Windows.Forms.ToolStripMenuItem newLayerItem;
	private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	private System.Windows.Forms.ToolStripMenuItem deleteItem;
}