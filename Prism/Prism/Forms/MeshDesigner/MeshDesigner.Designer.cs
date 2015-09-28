partial class MeshDesigner
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
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabTile = new System.Windows.Forms.TabPage();
			this.tilePicker = new TilePicker();
			this.tabSolid = new System.Windows.Forms.TabPage();
			this.colorPicker = new SolidColorPicker();
			this.tabControl.SuspendLayout();
			this.tabTile.SuspendLayout();
			this.tabSolid.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabTile);
			this.tabControl.Controls.Add(this.tabSolid);
			this.tabControl.Location = new System.Drawing.Point(12, 12);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(677, 416);
			this.tabControl.TabIndex = 1;
			// 
			// tabTile
			// 
			this.tabTile.Controls.Add(this.tilePicker);
			this.tabTile.Location = new System.Drawing.Point(4, 22);
			this.tabTile.Name = "tabTile";
			this.tabTile.Padding = new System.Windows.Forms.Padding(3);
			this.tabTile.Size = new System.Drawing.Size(669, 390);
			this.tabTile.TabIndex = 0;
			this.tabTile.Text = "Tile";
			this.tabTile.UseVisualStyleBackColor = true;
			// 
			// tilePicker
			// 
			this.tilePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tilePicker.Editor = null;
			this.tilePicker.Location = new System.Drawing.Point(0, 0);
			this.tilePicker.Name = "tilePicker";
			this.tilePicker.Size = new System.Drawing.Size(669, 390);
			this.tilePicker.TabIndex = 1;
			// 
			// tabSolid
			// 
			this.tabSolid.Controls.Add(this.colorPicker);
			this.tabSolid.Location = new System.Drawing.Point(4, 22);
			this.tabSolid.Name = "tabSolid";
			this.tabSolid.Padding = new System.Windows.Forms.Padding(3);
			this.tabSolid.Size = new System.Drawing.Size(669, 390);
			this.tabSolid.TabIndex = 1;
			this.tabSolid.Text = "Solid Color";
			this.tabSolid.UseVisualStyleBackColor = true;
			// 
			// colorPicker
			// 
			this.colorPicker.Editor = null;
			this.colorPicker.Location = new System.Drawing.Point(0, 0);
			this.colorPicker.Name = "colorPicker";
			this.colorPicker.Size = new System.Drawing.Size(669, 390);
			this.colorPicker.TabIndex = 0;
			// 
			// MeshDesigner
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(701, 440);
			this.Controls.Add(this.tabControl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MeshDesigner";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Mesh Designer";
			this.tabControl.ResumeLayout(false);
			this.tabTile.ResumeLayout(false);
			this.tabSolid.ResumeLayout(false);
			this.ResumeLayout(false);

	}

	#endregion

	private System.Windows.Forms.TabControl tabControl;
	private System.Windows.Forms.TabPage tabTile;
	private TilePicker tilePicker;
	private System.Windows.Forms.TabPage tabSolid;
	private SolidColorPicker colorPicker;
}
