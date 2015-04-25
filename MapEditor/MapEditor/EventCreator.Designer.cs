namespace MapEditor
{
	partial class EventCreator
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
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.lblName = new System.Windows.Forms.Label();
			this.lvlEventID = new System.Windows.Forms.Label();
			this.textBoxID = new System.Windows.Forms.TextBox();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.colorButton = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(12, 25);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(228, 20);
			this.textBoxName.TabIndex = 0;
			this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.Location = new System.Drawing.Point(12, 9);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(66, 13);
			this.lblName.TabIndex = 6;
			this.lblName.Text = "Event Name";
			// 
			// lvlEventID
			// 
			this.lvlEventID.AutoSize = true;
			this.lvlEventID.Location = new System.Drawing.Point(15, 56);
			this.lvlEventID.Name = "lvlEventID";
			this.lvlEventID.Size = new System.Drawing.Size(49, 13);
			this.lvlEventID.TabIndex = 5;
			this.lvlEventID.Text = "Event ID";
			// 
			// textBoxID
			// 
			this.textBoxID.Location = new System.Drawing.Point(15, 72);
			this.textBoxID.Name = "textBoxID";
			this.textBoxID.Size = new System.Drawing.Size(22, 20);
			this.textBoxID.TabIndex = 1;
			this.textBoxID.TextChanged += new System.EventHandler(this.textBoxID_TextChanged);
			// 
			// colorButton
			// 
			this.colorButton.Location = new System.Drawing.Point(108, 56);
			this.colorButton.Name = "colorButton";
			this.colorButton.Size = new System.Drawing.Size(132, 35);
			this.colorButton.TabIndex = 2;
			this.colorButton.Text = "Color";
			this.colorButton.UseVisualStyleBackColor = true;
			this.colorButton.Click += new System.EventHandler(this.colorButton_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(75, 111);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(111, 42);
			this.buttonOK.TabIndex = 3;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(91, 159);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(79, 28);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// EventCreator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(264, 200);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.colorButton);
			this.Controls.Add(this.lvlEventID);
			this.Controls.Add(this.textBoxID);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.textBoxName);
			this.Name = "EventCreator";
			this.Text = "Event Creator";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.Label lblName;
		private System.Windows.Forms.Label lvlEventID;
		private System.Windows.Forms.TextBox textBoxID;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Button colorButton;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
	}
}