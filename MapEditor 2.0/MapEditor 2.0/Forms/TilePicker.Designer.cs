partial class TilePicker
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
			this.tileList = new System.Windows.Forms.ListView();
			this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.textureList = new System.Windows.Forms.ListView();
			this.textureName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.textureLabel = new System.Windows.Forms.Label();
			this.filterBox = new System.Windows.Forms.TextBox();
			this.filterLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// tileList
			// 
			this.tileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name});
			this.tileList.HideSelection = false;
			this.tileList.Location = new System.Drawing.Point(178, 13);
			this.tileList.MultiSelect = false;
			this.tileList.Name = "tileList";
			this.tileList.Size = new System.Drawing.Size(367, 309);
			this.tileList.TabIndex = 1;
			this.tileList.UseCompatibleStateImageBehavior = false;
			this.tileList.SelectedIndexChanged += new System.EventHandler(this.TileSelected);
			// 
			// name
			// 
			this.name.Text = "Name";
			// 
			// textureList
			// 
			this.textureList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textureList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.textureName});
			this.textureList.FullRowSelect = true;
			this.textureList.GridLines = true;
			this.textureList.Location = new System.Drawing.Point(12, 36);
			this.textureList.Name = "textureList";
			this.textureList.Size = new System.Drawing.Size(159, 315);
			this.textureList.TabIndex = 2;
			this.textureList.UseCompatibleStateImageBehavior = false;
			this.textureList.View = System.Windows.Forms.View.List;
			this.textureList.SelectedIndexChanged += new System.EventHandler(this.TextureSelected);
			// 
			// textureName
			// 
			this.textureName.Text = "Name";
			this.textureName.Width = 149;
			// 
			// textureLabel
			// 
			this.textureLabel.AutoSize = true;
			this.textureLabel.Location = new System.Drawing.Point(10, 13);
			this.textureLabel.Name = "textureLabel";
			this.textureLabel.Size = new System.Drawing.Size(43, 13);
			this.textureLabel.TabIndex = 3;
			this.textureLabel.Text = "Texture";
			// 
			// filterBox
			// 
			this.filterBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.filterBox.Location = new System.Drawing.Point(212, 329);
			this.filterBox.Name = "filterBox";
			this.filterBox.Size = new System.Drawing.Size(333, 20);
			this.filterBox.TabIndex = 4;
			this.filterBox.TextChanged += new System.EventHandler(this.FilterChanged);
			// 
			// filterLabel
			// 
			this.filterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.filterLabel.AutoSize = true;
			this.filterLabel.Location = new System.Drawing.Point(177, 332);
			this.filterLabel.Name = "filterLabel";
			this.filterLabel.Size = new System.Drawing.Size(29, 13);
			this.filterLabel.TabIndex = 5;
			this.filterLabel.Text = "Filter";
			this.filterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// TilePicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(557, 363);
			this.Controls.Add(this.filterLabel);
			this.Controls.Add(this.filterBox);
			this.Controls.Add(this.textureLabel);
			this.Controls.Add(this.textureList);
			this.Controls.Add(this.tileList);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TilePicker";
			this.Text = "Tile Picker";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClose);
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion
	private System.Windows.Forms.ListView tileList;
	private System.Windows.Forms.ColumnHeader name;
	private System.Windows.Forms.ListView textureList;
	private System.Windows.Forms.ColumnHeader textureName;
	private System.Windows.Forms.Label textureLabel;
	private System.Windows.Forms.TextBox filterBox;
	private System.Windows.Forms.Label filterLabel;
}
