namespace MapEditor
{
	partial class LevelDataForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.mapNameBox = new System.Windows.Forms.TextBox();
			this.gameModeBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(9, 7);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "Map Name";
			this.label1.Click += new System.EventHandler(this.label1_Click);
			// 
			// mapNameBox
			// 
			this.mapNameBox.Location = new System.Drawing.Point(12, 28);
			this.mapNameBox.Name = "mapNameBox";
			this.mapNameBox.Size = new System.Drawing.Size(228, 20);
			this.mapNameBox.TabIndex = 1;
			this.mapNameBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// gameModeBox
			// 
			this.gameModeBox.Location = new System.Drawing.Point(12, 86);
			this.gameModeBox.Name = "gameModeBox";
			this.gameModeBox.Size = new System.Drawing.Size(27, 20);
			this.gameModeBox.TabIndex = 3;
			this.gameModeBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(9, 65);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(121, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "Game Mode ID";
			this.label2.Click += new System.EventHandler(this.label2_Click);
			// 
			// LevelDataForm
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(253, 120);
			this.Controls.Add(this.gameModeBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.mapNameBox);
			this.Controls.Add(this.label1);
			this.Name = "LevelDataForm";
			this.Text = "Level Data";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox mapNameBox;
		private System.Windows.Forms.TextBox gameModeBox;
		private System.Windows.Forms.Label label2;
	}
}