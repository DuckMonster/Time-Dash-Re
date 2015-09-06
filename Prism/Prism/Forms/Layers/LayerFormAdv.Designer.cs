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
			this.nodeCheckBox = new Aga.Controls.Tree.NodeControls.NodeCheckBox();
			this.nodeStateIcon = new Aga.Controls.Tree.NodeControls.NodeStateIcon();
			this.nodeText = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemNew = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemNewLayer = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemNewFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemMerge = new System.Windows.Forms.ToolStripMenuItem();
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
			this.layerTree.NodeControls.Add(this.nodeCheckBox);
			this.layerTree.NodeControls.Add(this.nodeStateIcon);
			this.layerTree.NodeControls.Add(this.nodeText);
			this.layerTree.SelectedNode = null;
			this.layerTree.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
			this.layerTree.Size = new System.Drawing.Size(336, 387);
			this.layerTree.TabIndex = 0;
			this.layerTree.Text = "treeViewAdv1";
			this.layerTree.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.ItemDoubleClicked);
			this.layerTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.ItemDrop);
			this.layerTree.DragOver += new System.Windows.Forms.DragEventHandler(this.ItemDragOver);
			this.layerTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyboardPress);
			this.layerTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MousePressed);
			// 
			// nodeCheckBox
			// 
			this.nodeCheckBox.DataPropertyName = "Visible";
			this.nodeCheckBox.EditEnabled = true;
			this.nodeCheckBox.LeftMargin = 0;
			this.nodeCheckBox.ParentColumn = null;
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
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
			// 
			// menuItemDelete
			// 
			this.menuItemDelete.Name = "menuItemDelete";
			this.menuItemDelete.Size = new System.Drawing.Size(142, 22);
			this.menuItemDelete.Text = "Delete";
			// 
			// menuItemCopy
			// 
			this.menuItemCopy.Name = "menuItemCopy";
			this.menuItemCopy.Size = new System.Drawing.Size(142, 22);
			this.menuItemCopy.Text = "Copy";
			// 
			// menuItemPaste
			// 
			this.menuItemPaste.Name = "menuItemPaste";
			this.menuItemPaste.Size = new System.Drawing.Size(142, 22);
			this.menuItemPaste.Text = "Paste";
			// 
			// rightClickMenu
			// 
			this.rightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNew,
            this.toolStripSeparator1,
            this.menuItemDelete,
            this.menuItemCopy,
            this.menuItemPaste,
            this.toolStripSeparator2,
            this.menuItemMerge});
			this.rightClickMenu.Name = "rightClickMenu";
			this.rightClickMenu.Size = new System.Drawing.Size(143, 126);
			// 
			// menuItemNew
			// 
			this.menuItemNew.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemNewLayer,
            this.menuItemNewFolder});
			this.menuItemNew.Name = "menuItemNew";
			this.menuItemNew.Size = new System.Drawing.Size(142, 22);
			this.menuItemNew.Text = "New...";
			// 
			// menuItemNewLayer
			// 
			this.menuItemNewLayer.Name = "menuItemNewLayer";
			this.menuItemNewLayer.Size = new System.Drawing.Size(107, 22);
			this.menuItemNewLayer.Text = "Layer";
			// 
			// menuItemNewFolder
			// 
			this.menuItemNewFolder.Name = "menuItemNewFolder";
			this.menuItemNewFolder.Size = new System.Drawing.Size(107, 22);
			this.menuItemNewFolder.Text = "Folder";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(139, 6);
			// 
			// menuItemMerge
			// 
			this.menuItemMerge.Name = "menuItemMerge";
			this.menuItemMerge.Size = new System.Drawing.Size(142, 22);
			this.menuItemMerge.Text = "Merge Down";
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
	private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	private System.Windows.Forms.ToolStripMenuItem menuItemDelete;
	private System.Windows.Forms.ToolStripMenuItem menuItemCopy;
	private System.Windows.Forms.ToolStripMenuItem menuItemPaste;
	private System.Windows.Forms.ContextMenuStrip rightClickMenu;
	private System.Windows.Forms.ToolStripMenuItem menuItemNew;
	private System.Windows.Forms.ToolStripMenuItem menuItemNewLayer;
	private System.Windows.Forms.ToolStripMenuItem menuItemNewFolder;
	private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
	private System.Windows.Forms.ToolStripMenuItem menuItemMerge;
	private Aga.Controls.Tree.NodeControls.NodeCheckBox nodeCheckBox;
}