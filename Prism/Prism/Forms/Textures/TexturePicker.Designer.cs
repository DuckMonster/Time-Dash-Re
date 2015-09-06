partial class TextureForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureForm));
			this.textureList = new System.Windows.Forms.ListView();
			this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.textureLabel = new System.Windows.Forms.Label();
			this.editButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.previewBox = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
			this.SuspendLayout();
			// 
			// textureList
			// 
			this.textureList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name});
			this.textureList.FullRowSelect = true;
			this.textureList.GridLines = true;
			this.textureList.LabelEdit = true;
			this.textureList.Location = new System.Drawing.Point(12, 25);
			this.textureList.Name = "textureList";
			this.textureList.Size = new System.Drawing.Size(178, 260);
			this.textureList.TabIndex = 0;
			this.textureList.UseCompatibleStateImageBehavior = false;
			this.textureList.View = System.Windows.Forms.View.Details;
			this.textureList.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.TextureNameChanged);
			this.textureList.SelectedIndexChanged += new System.EventHandler(this.ListIndexChanged);
			// 
			// name
			// 
			this.name.Text = "Name";
			this.name.Width = 148;
			// 
			// textureLabel
			// 
			this.textureLabel.AutoSize = true;
			this.textureLabel.Location = new System.Drawing.Point(13, 6);
			this.textureLabel.Name = "textureLabel";
			this.textureLabel.Size = new System.Drawing.Size(48, 13);
			this.textureLabel.TabIndex = 1;
			this.textureLabel.Text = "Textures";
			// 
			// editButton
			// 
			this.editButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("editButton.BackgroundImage")));
			this.editButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.editButton.Enabled = false;
			this.editButton.Location = new System.Drawing.Point(160, 293);
			this.editButton.Name = "editButton";
			this.editButton.Size = new System.Drawing.Size(30, 30);
			this.editButton.TabIndex = 6;
			this.editButton.UseVisualStyleBackColor = true;
			this.editButton.Click += new System.EventHandler(this.EditButtonPressed);
			// 
			// deleteButton
			// 
			this.deleteButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("deleteButton.BackgroundImage")));
			this.deleteButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.deleteButton.Enabled = false;
			this.deleteButton.Location = new System.Drawing.Point(48, 291);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(30, 30);
			this.deleteButton.TabIndex = 5;
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.DeleteButtonPressed);
			// 
			// addButton
			// 
			this.addButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("addButton.BackgroundImage")));
			this.addButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.addButton.Location = new System.Drawing.Point(12, 291);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(30, 30);
			this.addButton.TabIndex = 4;
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.AddButtonPressed);
			// 
			// previewBox
			// 
			this.previewBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.previewBox.BackColor = System.Drawing.SystemColors.ControlDark;
			this.previewBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.previewBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.previewBox.InitialImage = null;
			this.previewBox.Location = new System.Drawing.Point(246, 25);
			this.previewBox.Name = "previewBox";
			this.previewBox.Size = new System.Drawing.Size(260, 260);
			this.previewBox.TabIndex = 7;
			this.previewBox.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(254, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Preview";
			// 
			// TextureForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(511, 321);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.previewBox);
			this.Controls.Add(this.editButton);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.textureLabel);
			this.Controls.Add(this.textureList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextureForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Textures";
			((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.ListView textureList;
	private System.Windows.Forms.Label textureLabel;
	private System.Windows.Forms.Button editButton;
	private System.Windows.Forms.Button deleteButton;
	private System.Windows.Forms.Button addButton;
	private System.Windows.Forms.PictureBox previewBox;
	private System.Windows.Forms.ColumnHeader name;
	private System.Windows.Forms.Label label1;
}
