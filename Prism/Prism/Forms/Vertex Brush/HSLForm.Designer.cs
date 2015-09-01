partial class HSLForm
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
			this.hueBar = new System.Windows.Forms.TrackBar();
			this.saturationBar = new System.Windows.Forms.TrackBar();
			this.lightnessBar = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this.hueBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.saturationBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lightnessBar)).BeginInit();
			this.SuspendLayout();
			// 
			// hueBar
			// 
			this.hueBar.Location = new System.Drawing.Point(13, 13);
			this.hueBar.Maximum = 100;
			this.hueBar.Name = "hueBar";
			this.hueBar.Size = new System.Drawing.Size(436, 45);
			this.hueBar.TabIndex = 0;
			this.hueBar.Value = 50;
			this.hueBar.Scroll += new System.EventHandler(this.HueChanged);
			// 
			// saturationBar
			// 
			this.saturationBar.Location = new System.Drawing.Point(12, 64);
			this.saturationBar.Maximum = 100;
			this.saturationBar.Name = "saturationBar";
			this.saturationBar.Size = new System.Drawing.Size(436, 45);
			this.saturationBar.TabIndex = 1;
			this.saturationBar.Value = 50;
			this.saturationBar.Scroll += new System.EventHandler(this.SaturationChanged);
			// 
			// lightnessBar
			// 
			this.lightnessBar.Location = new System.Drawing.Point(12, 115);
			this.lightnessBar.Maximum = 100;
			this.lightnessBar.Name = "lightnessBar";
			this.lightnessBar.Size = new System.Drawing.Size(436, 45);
			this.lightnessBar.TabIndex = 2;
			this.lightnessBar.Value = 50;
			this.lightnessBar.Scroll += new System.EventHandler(this.LightnessChanged);
			// 
			// HSLForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(461, 170);
			this.Controls.Add(this.lightnessBar);
			this.Controls.Add(this.saturationBar);
			this.Controls.Add(this.hueBar);
			this.Name = "HSLForm";
			this.Text = "HSLForm";
			((System.ComponentModel.ISupportInitialize)(this.hueBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.saturationBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lightnessBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.TrackBar hueBar;
	private System.Windows.Forms.TrackBar saturationBar;
	private System.Windows.Forms.TrackBar lightnessBar;
}
