partial class DebugForm
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
			this.textPositionX = new System.Windows.Forms.TextBox();
			this.textPositionY = new System.Windows.Forms.TextBox();
			this.variousBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textPositionX
			// 
			this.textPositionX.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.textPositionX.Location = new System.Drawing.Point(12, 13);
			this.textPositionX.Name = "textPositionX";
			this.textPositionX.ReadOnly = true;
			this.textPositionX.Size = new System.Drawing.Size(67, 20);
			this.textPositionX.TabIndex = 0;
			// 
			// textPositionY
			// 
			this.textPositionY.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.textPositionY.Location = new System.Drawing.Point(85, 13);
			this.textPositionY.Name = "textPositionY";
			this.textPositionY.ReadOnly = true;
			this.textPositionY.Size = new System.Drawing.Size(67, 20);
			this.textPositionY.TabIndex = 1;
			// 
			// variousBox
			// 
			this.variousBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.variousBox.Location = new System.Drawing.Point(158, 12);
			this.variousBox.Name = "variousBox";
			this.variousBox.ReadOnly = true;
			this.variousBox.Size = new System.Drawing.Size(258, 20);
			this.variousBox.TabIndex = 2;
			// 
			// DebugForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.ClientSize = new System.Drawing.Size(624, 44);
			this.ControlBox = false;
			this.Controls.Add(this.variousBox);
			this.Controls.Add(this.textPositionY);
			this.Controls.Add(this.textPositionX);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "DebugForm";
			this.ShowInTaskbar = false;
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.TextBox textPositionX;
	private System.Windows.Forms.TextBox textPositionY;
	private System.Windows.Forms.TextBox variousBox;
}