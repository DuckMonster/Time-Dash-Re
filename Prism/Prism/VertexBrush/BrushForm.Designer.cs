partial class BrushForm
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
			this.lightnessBar = new System.Windows.Forms.TrackBar();
			this.hueBar = new System.Windows.Forms.TrackBar();
			this.saturationBar = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.hueBox = new System.Windows.Forms.TextBox();
			this.saturationBox = new System.Windows.Forms.TextBox();
			this.lightnessBox = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.opacityBar = new System.Windows.Forms.TrackBar();
			this.hardnessBar = new System.Windows.Forms.TrackBar();
			this.brushPreview = new System.Windows.Forms.PictureBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.sizeBar = new System.Windows.Forms.TrackBar();
			this.label15 = new System.Windows.Forms.Label();
			this.targetBox = new System.Windows.Forms.ComboBox();
			this.label16 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.lightnessBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.hueBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.saturationBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.opacityBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.hardnessBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.brushPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sizeBar)).BeginInit();
			this.SuspendLayout();
			// 
			// lightnessBar
			// 
			this.lightnessBar.Location = new System.Drawing.Point(35, 131);
			this.lightnessBar.Maximum = 200;
			this.lightnessBar.Name = "lightnessBar";
			this.lightnessBar.Size = new System.Drawing.Size(384, 45);
			this.lightnessBar.TabIndex = 0;
			this.lightnessBar.TickFrequency = 0;
			this.lightnessBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.lightnessBar.Value = 100;
			this.lightnessBar.Scroll += new System.EventHandler(this.HSLSliderChanged);
			this.lightnessBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SliderClicked);
			// 
			// hueBar
			// 
			this.hueBar.Location = new System.Drawing.Point(35, 32);
			this.hueBar.Maximum = 200;
			this.hueBar.Name = "hueBar";
			this.hueBar.Size = new System.Drawing.Size(384, 45);
			this.hueBar.TabIndex = 0;
			this.hueBar.TickFrequency = 0;
			this.hueBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.hueBar.Value = 100;
			this.hueBar.Scroll += new System.EventHandler(this.HSLSliderChanged);
			this.hueBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SliderClicked);
			// 
			// saturationBar
			// 
			this.saturationBar.Location = new System.Drawing.Point(35, 82);
			this.saturationBar.Maximum = 200;
			this.saturationBar.Name = "saturationBar";
			this.saturationBar.Size = new System.Drawing.Size(384, 45);
			this.saturationBar.TabIndex = 0;
			this.saturationBar.TickFrequency = 0;
			this.saturationBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.saturationBar.Value = 100;
			this.saturationBar.Scroll += new System.EventHandler(this.HSLSliderChanged);
			this.saturationBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SliderClicked);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(54, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(27, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Hue";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(54, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Saturation";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(54, 121);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(52, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Lightness";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(416, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(31, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "+180";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 46);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(28, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "-180";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(25, 96);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(13, 13);
			this.label6.TabIndex = 7;
			this.label6.Text = "0";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(416, 96);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(19, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "+6";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(25, 145);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(13, 13);
			this.label8.TabIndex = 9;
			this.label8.Text = "0";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(416, 145);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(19, 13);
			this.label9.TabIndex = 8;
			this.label9.Text = "+6";
			// 
			// hueBox
			// 
			this.hueBox.Location = new System.Drawing.Point(453, 43);
			this.hueBox.Name = "hueBox";
			this.hueBox.Size = new System.Drawing.Size(40, 20);
			this.hueBox.TabIndex = 10;
			this.hueBox.Text = "0";
			this.hueBox.TextChanged += new System.EventHandler(this.HSLBoxChanged);
			// 
			// saturationBox
			// 
			this.saturationBox.Location = new System.Drawing.Point(453, 93);
			this.saturationBox.Name = "saturationBox";
			this.saturationBox.Size = new System.Drawing.Size(40, 20);
			this.saturationBox.TabIndex = 11;
			this.saturationBox.Text = "1";
			this.saturationBox.TextChanged += new System.EventHandler(this.HSLBoxChanged);
			// 
			// lightnessBox
			// 
			this.lightnessBox.Location = new System.Drawing.Point(453, 142);
			this.lightnessBox.Name = "lightnessBox";
			this.lightnessBox.Size = new System.Drawing.Size(40, 20);
			this.lightnessBox.TabIndex = 12;
			this.lightnessBox.Text = "1";
			this.lightnessBox.TextChanged += new System.EventHandler(this.HSLBoxChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(221, 163);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(13, 13);
			this.label10.TabIndex = 14;
			this.label10.Text = "1";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(221, 114);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(13, 13);
			this.label11.TabIndex = 13;
			this.label11.Text = "1";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(221, 67);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(13, 13);
			this.label12.TabIndex = 15;
			this.label12.Text = "0";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// opacityBar
			// 
			this.opacityBar.Location = new System.Drawing.Point(524, 32);
			this.opacityBar.Maximum = 200;
			this.opacityBar.Name = "opacityBar";
			this.opacityBar.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.opacityBar.Size = new System.Drawing.Size(45, 189);
			this.opacityBar.TabIndex = 16;
			this.opacityBar.TickFrequency = 0;
			this.opacityBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.opacityBar.Value = 200;
			this.opacityBar.Scroll += new System.EventHandler(this.OHSSliderChanged);
			this.opacityBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SliderClicked);
			this.opacityBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OHMouseReleased);
			// 
			// hardnessBar
			// 
			this.hardnessBar.Location = new System.Drawing.Point(567, 32);
			this.hardnessBar.Maximum = 200;
			this.hardnessBar.Name = "hardnessBar";
			this.hardnessBar.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.hardnessBar.Size = new System.Drawing.Size(45, 189);
			this.hardnessBar.TabIndex = 17;
			this.hardnessBar.TickFrequency = 0;
			this.hardnessBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.hardnessBar.Value = 200;
			this.hardnessBar.Scroll += new System.EventHandler(this.OHSSliderChanged);
			this.hardnessBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SliderClicked);
			this.hardnessBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OHMouseReleased);
			// 
			// brushPreview
			// 
			this.brushPreview.BackColor = System.Drawing.Color.Black;
			this.brushPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.brushPreview.Location = new System.Drawing.Point(533, 228);
			this.brushPreview.Name = "brushPreview";
			this.brushPreview.Size = new System.Drawing.Size(66, 66);
			this.brushPreview.TabIndex = 18;
			this.brushPreview.TabStop = false;
			this.brushPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.BrushPreviewPaint);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(521, 16);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(43, 13);
			this.label13.TabIndex = 19;
			this.label13.Text = "Opacity";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(564, 16);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(52, 13);
			this.label14.TabIndex = 20;
			this.label14.Text = "Hardness";
			// 
			// sizeBar
			// 
			this.sizeBar.Location = new System.Drawing.Point(338, 239);
			this.sizeBar.Maximum = 200;
			this.sizeBar.Name = "sizeBar";
			this.sizeBar.Size = new System.Drawing.Size(189, 45);
			this.sizeBar.TabIndex = 21;
			this.sizeBar.TickFrequency = 0;
			this.sizeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.sizeBar.Value = 10;
			this.sizeBar.Scroll += new System.EventHandler(this.OHSSliderChanged);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(358, 223);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(27, 13);
			this.label15.TabIndex = 22;
			this.label15.Text = "Size";
			// 
			// targetBox
			// 
			this.targetBox.FormattingEnabled = true;
			this.targetBox.Location = new System.Drawing.Point(15, 191);
			this.targetBox.Name = "targetBox";
			this.targetBox.Size = new System.Drawing.Size(157, 21);
			this.targetBox.TabIndex = 23;
			this.targetBox.SelectedIndexChanged += new System.EventHandler(this.TargetChanged);
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(18, 174);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(56, 13);
			this.label16.TabIndex = 24;
			this.label16.Text = "Pen target";
			// 
			// BrushForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(629, 306);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.targetBox);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.sizeBar);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.brushPreview);
			this.Controls.Add(this.hardnessBar);
			this.Controls.Add(this.opacityBar);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.lightnessBox);
			this.Controls.Add(this.saturationBox);
			this.Controls.Add(this.hueBox);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.saturationBar);
			this.Controls.Add(this.hueBar);
			this.Controls.Add(this.lightnessBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BrushForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Select Brush";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.lightnessBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.hueBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.saturationBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.opacityBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.hardnessBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.brushPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sizeBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.TrackBar lightnessBar;
	private System.Windows.Forms.TrackBar hueBar;
	private System.Windows.Forms.TrackBar saturationBar;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.Label label2;
	private System.Windows.Forms.Label label3;
	private System.Windows.Forms.Label label4;
	private System.Windows.Forms.Label label5;
	private System.Windows.Forms.Label label6;
	private System.Windows.Forms.Label label7;
	private System.Windows.Forms.Label label8;
	private System.Windows.Forms.Label label9;
	private System.Windows.Forms.TextBox hueBox;
	private System.Windows.Forms.TextBox saturationBox;
	private System.Windows.Forms.TextBox lightnessBox;
	private System.Windows.Forms.Label label10;
	private System.Windows.Forms.Label label11;
	private System.Windows.Forms.Label label12;
	private System.Windows.Forms.TrackBar opacityBar;
	private System.Windows.Forms.TrackBar hardnessBar;
	private System.Windows.Forms.PictureBox brushPreview;
	private System.Windows.Forms.Label label13;
	private System.Windows.Forms.Label label14;
	private System.Windows.Forms.TrackBar sizeBar;
	private System.Windows.Forms.Label label15;
	private System.Windows.Forms.ComboBox targetBox;
	private System.Windows.Forms.Label label16;
}
