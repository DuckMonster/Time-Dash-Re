partial class OutlinerForm
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
			this.meshTree = new Aga.Controls.Tree.TreeViewAdv();
			this.nodeControlName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
			this.SuspendLayout();
			// 
			// meshTree
			// 
			this.meshTree.AllowDrop = true;
			this.meshTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.meshTree.BackColor = System.Drawing.SystemColors.Window;
			this.meshTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.meshTree.DefaultToolTipProvider = null;
			this.meshTree.DisplayDraggingNodes = true;
			this.meshTree.DragDropMarkColor = System.Drawing.Color.Black;
			this.meshTree.LineColor = System.Drawing.SystemColors.Control;
			this.meshTree.LoadOnDemand = true;
			this.meshTree.Location = new System.Drawing.Point(12, 12);
			this.meshTree.Model = null;
			this.meshTree.Name = "meshTree";
			this.meshTree.NodeControls.Add(this.nodeControlName);
			this.meshTree.SelectedNode = null;
			this.meshTree.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
			this.meshTree.Size = new System.Drawing.Size(260, 478);
			this.meshTree.TabIndex = 1;
			this.meshTree.Text = "treeViewAdv1";
			this.meshTree.SelectionChanged += new System.EventHandler(this.NodeSelected);
			this.meshTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.ItemDrop);
			this.meshTree.DragOver += new System.Windows.Forms.DragEventHandler(this.ItemDragOver);
			this.meshTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoved);
			// 
			// nodeControlName
			// 
			this.nodeControlName.DataPropertyName = "Name";
			this.nodeControlName.IncrementalSearchEnabled = true;
			this.nodeControlName.LeftMargin = 3;
			this.nodeControlName.ParentColumn = null;
			// 
			// OutlinerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 502);
			this.Controls.Add(this.meshTree);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "OutlinerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Outliner";
			this.TopMost = true;
			this.ResumeLayout(false);

	}

	#endregion

	private Aga.Controls.Tree.TreeViewAdv meshTree;
	private Aga.Controls.Tree.NodeControls.NodeTextBox nodeControlName;
}
