partial class TileEditor
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileEditor));
			this.uvBox = new System.Windows.Forms.PictureBox();
			this.tileList = new System.Windows.Forms.ListView();
			this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.debugLabel = new System.Windows.Forms.Label();
			this.editButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.uvBox)).BeginInit();
			this.SuspendLayout();
			// 
			// uvBox
			// 
			this.uvBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.uvBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.uvBox.Dock = System.Windows.Forms.DockStyle.Right;
			this.uvBox.Location = new System.Drawing.Point(367, 0);
			this.uvBox.Name = "uvBox";
			this.uvBox.Size = new System.Drawing.Size(546, 530);
			this.uvBox.TabIndex = 0;
			this.uvBox.TabStop = false;
			this.uvBox.Paint += new System.Windows.Forms.PaintEventHandler(this.UVBoxPaint);
			this.uvBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UVBoxMouseClick);
			this.uvBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UVBoxMouseMove);
			// 
			// tileList
			// 
			this.tileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name});
			this.tileList.FullRowSelect = true;
			this.tileList.GridLines = true;
			this.tileList.LabelEdit = true;
			this.tileList.Location = new System.Drawing.Point(12, 13);
			this.tileList.Name = "tileList";
			this.tileList.Size = new System.Drawing.Size(351, 380);
			this.tileList.TabIndex = 1;
			this.tileList.UseCompatibleStateImageBehavior = false;
			this.tileList.View = System.Windows.Forms.View.Details;
			this.tileList.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.TileNameChanged);
			// 
			// name
			// 
			this.name.Text = "Name";
			this.name.Width = 332;
			// 
			// debugLabel
			// 
			this.debugLabel.AutoSize = true;
			this.debugLabel.Location = new System.Drawing.Point(298, 363);
			this.debugLabel.Name = "debugLabel";
			this.debugLabel.Size = new System.Drawing.Size(0, 13);
			this.debugLabel.TabIndex = 2;
			// 
			// editButton
			// 
			this.editButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("editButton.BackgroundImage")));
			this.editButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.editButton.Enabled = false;
			this.editButton.Location = new System.Drawing.Point(333, 442);
			this.editButton.Name = "editButton";
			this.editButton.Size = new System.Drawing.Size(30, 30);
			this.editButton.TabIndex = 6;
			this.editButton.UseVisualStyleBackColor = true;
			// 
			// deleteButton
			// 
			this.deleteButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("deleteButton.BackgroundImage")));
			this.deleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.deleteButton.Enabled = false;
			this.deleteButton.Location = new System.Drawing.Point(48, 442);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(30, 30);
			this.deleteButton.TabIndex = 5;
			this.deleteButton.UseVisualStyleBackColor = true;
			// 
			// addButton
			// 
			this.addButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("addButton.BackgroundImage")));
			this.addButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.addButton.Enabled = false;
			this.addButton.Location = new System.Drawing.Point(12, 442);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(30, 30);
			this.addButton.TabIndex = 4;
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.AddButtonClicked);
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(12, 478);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(122, 48);
			this.closeButton.TabIndex = 7;
			this.closeButton.Text = "Close";
			this.closeButton.UseVisualStyleBackColor = true;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// TileEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(913, 530);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.editButton);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.debugLabel);
			this.Controls.Add(this.tileList);
			this.Controls.Add(this.uvBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TileEditor";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Tile Editor";
			((System.ComponentModel.ISupportInitialize)(this.uvBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.PictureBox uvBox;
	private System.Windows.Forms.ListView tileList;
	private System.Windows.Forms.Label debugLabel;
	private System.Windows.Forms.Button editButton;
	private System.Windows.Forms.Button deleteButton;
	private System.Windows.Forms.Button addButton;
	private System.Windows.Forms.ColumnHeader name;
	private System.Windows.Forms.Button closeButton;
}
