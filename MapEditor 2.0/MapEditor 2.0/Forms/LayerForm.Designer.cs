partial class LayerForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayerForm));
			this.layerList = new System.Windows.Forms.ListView();
			this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.addButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.editButton = new System.Windows.Forms.Button();
			this.visibleCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// layerList
			// 
			this.layerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.layerList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name});
			this.layerList.FullRowSelect = true;
			this.layerList.GridLines = true;
			this.layerList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.layerList.HideSelection = false;
			this.layerList.Location = new System.Drawing.Point(12, 12);
			this.layerList.Name = "layerList";
			this.layerList.Size = new System.Drawing.Size(187, 210);
			this.layerList.TabIndex = 0;
			this.layerList.UseCompatibleStateImageBehavior = false;
			this.layerList.View = System.Windows.Forms.View.Details;
			this.layerList.SelectedIndexChanged += new System.EventHandler(this.SelectedLayerChanged);
			this.layerList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnMouseDoubleClick);
			// 
			// name
			// 
			this.name.Text = "Name";
			this.name.Width = 160;
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.addButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("addButton.BackgroundImage")));
			this.addButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.addButton.Location = new System.Drawing.Point(13, 251);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(30, 30);
			this.addButton.TabIndex = 1;
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.AddButtonClicked);
			// 
			// deleteButton
			// 
			this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.deleteButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("deleteButton.BackgroundImage")));
			this.deleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.deleteButton.Enabled = false;
			this.deleteButton.Location = new System.Drawing.Point(49, 251);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(30, 30);
			this.deleteButton.TabIndex = 2;
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.DeleteButtonClicked);
			// 
			// editButton
			// 
			this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.editButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("editButton.BackgroundImage")));
			this.editButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.editButton.Enabled = false;
			this.editButton.Location = new System.Drawing.Point(169, 251);
			this.editButton.Name = "editButton";
			this.editButton.Size = new System.Drawing.Size(30, 30);
			this.editButton.TabIndex = 3;
			this.editButton.UseVisualStyleBackColor = true;
			this.editButton.Click += new System.EventHandler(this.EditButtonClicked);
			// 
			// visibleCheckBox
			// 
			this.visibleCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.visibleCheckBox.AutoSize = true;
			this.visibleCheckBox.Enabled = false;
			this.visibleCheckBox.Location = new System.Drawing.Point(13, 228);
			this.visibleCheckBox.Name = "visibleCheckBox";
			this.visibleCheckBox.Size = new System.Drawing.Size(56, 17);
			this.visibleCheckBox.TabIndex = 4;
			this.visibleCheckBox.Text = "Visible";
			this.visibleCheckBox.UseVisualStyleBackColor = true;
			this.visibleCheckBox.CheckedChanged += new System.EventHandler(this.ToggleVisible);
			// 
			// LayerForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(211, 290);
			this.Controls.Add(this.visibleCheckBox);
			this.Controls.Add(this.editButton);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.layerList);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LayerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Layers";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClose);
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.ListView layerList;
	private System.Windows.Forms.ColumnHeader name;
	private System.Windows.Forms.Button addButton;
	private System.Windows.Forms.Button deleteButton;
	private System.Windows.Forms.Button editButton;
	private System.Windows.Forms.CheckBox visibleCheckBox;
}