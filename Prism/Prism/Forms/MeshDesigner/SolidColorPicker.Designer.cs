partial class SolidColorPicker
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

	#region Component Designer generated code

	/// <summary> 
	/// Required method for Designer support - do not modify 
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
			this.colorButton = new System.Windows.Forms.Button();
			this.colorBox = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.colorBox)).BeginInit();
			this.SuspendLayout();
			// 
			// colorButton
			// 
			this.colorButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.colorButton.Location = new System.Drawing.Point(4, 274);
			this.colorButton.Name = "colorButton";
			this.colorButton.Size = new System.Drawing.Size(448, 64);
			this.colorButton.TabIndex = 1;
			this.colorButton.Text = "Pick Color";
			this.colorButton.UseVisualStyleBackColor = true;
			this.colorButton.Click += new System.EventHandler(this.ColorButtonClicked);
			// 
			// colorBox
			// 
			this.colorBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.colorBox.Location = new System.Drawing.Point(3, 3);
			this.colorBox.Name = "colorBox";
			this.colorBox.Size = new System.Drawing.Size(449, 264);
			this.colorBox.TabIndex = 0;
			this.colorBox.TabStop = false;
			this.colorBox.Click += new System.EventHandler(this.ColorButtonClicked);
			// 
			// SolidColorPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.colorButton);
			this.Controls.Add(this.colorBox);
			this.Name = "SolidColorPicker";
			this.Size = new System.Drawing.Size(455, 341);
			((System.ComponentModel.ISupportInitialize)(this.colorBox)).EndInit();
			this.ResumeLayout(false);

	}

	#endregion

	private System.Windows.Forms.PictureBox colorBox;
	private System.Windows.Forms.Button colorButton;
}

