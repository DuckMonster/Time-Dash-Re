partial class OptionsForm
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
			this.showGridBox = new System.Windows.Forms.CheckBox();
			this.gridOpacityBar = new System.Windows.Forms.TrackBar();
			this.gridOpacityLabel = new System.Windows.Forms.Label();
			this.focusLayerBox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.layerOpacityBar = new System.Windows.Forms.TrackBar();
			this.label2 = new System.Windows.Forms.Label();
			this.meshBorderOpacity = new System.Windows.Forms.TrackBar();
			this.meshBorderBox = new System.Windows.Forms.CheckBox();
			this.gridSizeBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.colorLabel = new System.Windows.Forms.Label();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.colorBoxL = new System.Windows.Forms.PictureBox();
			this.colorBoxR = new System.Windows.Forms.PictureBox();
			this.backColorPreview = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.gridOpacityBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layerOpacityBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.meshBorderOpacity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.colorBoxL)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.colorBoxR)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.backColorPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// showGridBox
			// 
			this.showGridBox.AutoSize = true;
			this.showGridBox.Location = new System.Drawing.Point(12, 12);
			this.showGridBox.Name = "showGridBox";
			this.showGridBox.Size = new System.Drawing.Size(75, 17);
			this.showGridBox.TabIndex = 0;
			this.showGridBox.Text = "Show Grid";
			this.showGridBox.UseVisualStyleBackColor = true;
			this.showGridBox.CheckedChanged += new System.EventHandler(this.ShowGridChanged);
			// 
			// gridOpacityBar
			// 
			this.gridOpacityBar.Location = new System.Drawing.Point(12, 50);
			this.gridOpacityBar.Name = "gridOpacityBar";
			this.gridOpacityBar.Size = new System.Drawing.Size(372, 45);
			this.gridOpacityBar.TabIndex = 1;
			this.gridOpacityBar.Scroll += new System.EventHandler(this.GridOpacityChanged);
			// 
			// gridOpacityLabel
			// 
			this.gridOpacityLabel.AutoSize = true;
			this.gridOpacityLabel.Location = new System.Drawing.Point(13, 36);
			this.gridOpacityLabel.Name = "gridOpacityLabel";
			this.gridOpacityLabel.Size = new System.Drawing.Size(65, 13);
			this.gridOpacityLabel.TabIndex = 2;
			this.gridOpacityLabel.Text = "Grid Opacity";
			// 
			// focusLayerBox
			// 
			this.focusLayerBox.AutoSize = true;
			this.focusLayerBox.Location = new System.Drawing.Point(12, 101);
			this.focusLayerBox.Name = "focusLayerBox";
			this.focusLayerBox.Size = new System.Drawing.Size(117, 17);
			this.focusLayerBox.TabIndex = 3;
			this.focusLayerBox.Text = "Focus Active Layer";
			this.focusLayerBox.UseVisualStyleBackColor = true;
			this.focusLayerBox.CheckedChanged += new System.EventHandler(this.FocusLayerChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 124);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(127, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Unfocused Layer Opacity";
			// 
			// layerOpacityBar
			// 
			this.layerOpacityBar.Location = new System.Drawing.Point(12, 137);
			this.layerOpacityBar.Name = "layerOpacityBar";
			this.layerOpacityBar.Size = new System.Drawing.Size(372, 45);
			this.layerOpacityBar.TabIndex = 4;
			this.layerOpacityBar.Scroll += new System.EventHandler(this.LayerOpacityChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 211);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Mesh Border Opacity";
			// 
			// meshBorderOpacity
			// 
			this.meshBorderOpacity.Location = new System.Drawing.Point(12, 225);
			this.meshBorderOpacity.Name = "meshBorderOpacity";
			this.meshBorderOpacity.Size = new System.Drawing.Size(372, 45);
			this.meshBorderOpacity.TabIndex = 7;
			this.meshBorderOpacity.Scroll += new System.EventHandler(this.MeshBorderOpacityChanged);
			// 
			// meshBorderBox
			// 
			this.meshBorderBox.AutoSize = true;
			this.meshBorderBox.Location = new System.Drawing.Point(12, 188);
			this.meshBorderBox.Name = "meshBorderBox";
			this.meshBorderBox.Size = new System.Drawing.Size(121, 17);
			this.meshBorderBox.TabIndex = 6;
			this.meshBorderBox.Text = "Show Mesh Borders";
			this.meshBorderBox.UseVisualStyleBackColor = true;
			this.meshBorderBox.CheckedChanged += new System.EventHandler(this.MeshBordersChanged);
			// 
			// gridSizeBox
			// 
			this.gridSizeBox.Location = new System.Drawing.Point(338, 9);
			this.gridSizeBox.Name = "gridSizeBox";
			this.gridSizeBox.Size = new System.Drawing.Size(46, 20);
			this.gridSizeBox.TabIndex = 9;
			this.gridSizeBox.TextChanged += new System.EventHandler(this.GridSizeChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(283, 13);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Grid Size";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label4.Location = new System.Drawing.Point(93, 13);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 13);
			this.label4.TabIndex = 11;
			this.label4.Text = "(CTRL + SHIFT + G)";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label5.Location = new System.Drawing.Point(135, 102);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(102, 13);
			this.label5.TabIndex = 12;
			this.label5.Text = "(CTRL + SHIFT + L)";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label6.Location = new System.Drawing.Point(139, 189);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(103, 13);
			this.label6.TabIndex = 13;
			this.label6.Text = "(CTRL + SHIFT + B)";
			// 
			// colorLabel
			// 
			this.colorLabel.AutoSize = true;
			this.colorLabel.Location = new System.Drawing.Point(9, 287);
			this.colorLabel.Name = "colorLabel";
			this.colorLabel.Size = new System.Drawing.Size(175, 13);
			this.colorLabel.TabIndex = 14;
			this.colorLabel.Text = "Background Color (Click to change)";
			// 
			// colorBoxL
			// 
			this.colorBoxL.Location = new System.Drawing.Point(12, 303);
			this.colorBoxL.Name = "colorBoxL";
			this.colorBoxL.Size = new System.Drawing.Size(35, 34);
			this.colorBoxL.TabIndex = 15;
			this.colorBoxL.TabStop = false;
			this.colorBoxL.Click += new System.EventHandler(this.BackColorLeftClick);
			// 
			// colorBoxR
			// 
			this.colorBoxR.Location = new System.Drawing.Point(349, 303);
			this.colorBoxR.Name = "colorBoxR";
			this.colorBoxR.Size = new System.Drawing.Size(35, 34);
			this.colorBoxR.TabIndex = 16;
			this.colorBoxR.TabStop = false;
			this.colorBoxR.Click += new System.EventHandler(this.BackColorRightClick);
			// 
			// backColorPreview
			// 
			this.backColorPreview.Location = new System.Drawing.Point(53, 314);
			this.backColorPreview.Name = "backColorPreview";
			this.backColorPreview.Size = new System.Drawing.Size(290, 13);
			this.backColorPreview.TabIndex = 17;
			this.backColorPreview.TabStop = false;
			this.backColorPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewPaint);
			// 
			// OptionsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(396, 361);
			this.Controls.Add(this.backColorPreview);
			this.Controls.Add(this.colorBoxR);
			this.Controls.Add(this.colorBoxL);
			this.Controls.Add(this.colorLabel);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.gridSizeBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.meshBorderOpacity);
			this.Controls.Add(this.meshBorderBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.layerOpacityBar);
			this.Controls.Add(this.focusLayerBox);
			this.Controls.Add(this.gridOpacityLabel);
			this.Controls.Add(this.gridOpacityBar);
			this.Controls.Add(this.showGridBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Options";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.gridOpacityBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layerOpacityBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.meshBorderOpacity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.colorBoxL)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.colorBoxR)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.backColorPreview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.CheckBox showGridBox;
	private System.Windows.Forms.TrackBar gridOpacityBar;
	private System.Windows.Forms.Label gridOpacityLabel;
	private System.Windows.Forms.CheckBox focusLayerBox;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.TrackBar layerOpacityBar;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.TrackBar meshBorderOpacity;
	private System.Windows.Forms.CheckBox meshBorderBox;
	private System.Windows.Forms.TextBox gridSizeBox;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.Label label5;
	private System.Windows.Forms.Label label6;
	private System.Windows.Forms.Label colorLabel;
	private System.Windows.Forms.ColorDialog colorDialog;
	private System.Windows.Forms.PictureBox colorBoxL;
	private System.Windows.Forms.PictureBox colorBoxR;
	private System.Windows.Forms.PictureBox backColorPreview;
}
