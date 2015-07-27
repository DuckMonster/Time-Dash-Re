partial class EditLayerForm
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
			this.nameBox = new System.Windows.Forms.TextBox();
			this.nameLabel = new System.Windows.Forms.Label();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.effectLabel = new System.Windows.Forms.Label();
			this.applyButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.effectAddButton = new System.Windows.Forms.Button();
			this.effectRemoveButton = new System.Windows.Forms.Button();
			this.effectEditButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// nameBox
			// 
			this.nameBox.Location = new System.Drawing.Point(12, 29);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(128, 20);
			this.nameBox.TabIndex = 0;
			this.nameBox.TextChanged += new System.EventHandler(this.NameBoxChanged);
			// 
			// nameLabel
			// 
			this.nameLabel.AutoSize = true;
			this.nameLabel.Location = new System.Drawing.Point(13, 10);
			this.nameLabel.Name = "nameLabel";
			this.nameLabel.Size = new System.Drawing.Size(64, 13);
			this.nameLabel.TabIndex = 1;
			this.nameLabel.Text = "Layer Name";
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.FormattingEnabled = true;
			this.checkedListBox1.Location = new System.Drawing.Point(176, 29);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(166, 154);
			this.checkedListBox1.TabIndex = 2;
			// 
			// effectLabel
			// 
			this.effectLabel.AutoSize = true;
			this.effectLabel.Location = new System.Drawing.Point(173, 10);
			this.effectLabel.Name = "effectLabel";
			this.effectLabel.Size = new System.Drawing.Size(40, 13);
			this.effectLabel.TabIndex = 3;
			this.effectLabel.Text = "Effects";
			// 
			// applyButton
			// 
			this.applyButton.Location = new System.Drawing.Point(16, 167);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(124, 45);
			this.applyButton.TabIndex = 4;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.ApplyClicked);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(16, 218);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(124, 23);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelClicked);
			// 
			// effectAddButton
			// 
			this.effectAddButton.Location = new System.Drawing.Point(176, 189);
			this.effectAddButton.Name = "effectAddButton";
			this.effectAddButton.Size = new System.Drawing.Size(65, 23);
			this.effectAddButton.TabIndex = 6;
			this.effectAddButton.Text = "Add";
			this.effectAddButton.UseVisualStyleBackColor = true;
			// 
			// effectRemoveButton
			// 
			this.effectRemoveButton.Location = new System.Drawing.Point(176, 218);
			this.effectRemoveButton.Name = "effectRemoveButton";
			this.effectRemoveButton.Size = new System.Drawing.Size(65, 23);
			this.effectRemoveButton.TabIndex = 7;
			this.effectRemoveButton.Text = "Remove";
			this.effectRemoveButton.UseVisualStyleBackColor = true;
			// 
			// effectEditButton
			// 
			this.effectEditButton.Location = new System.Drawing.Point(248, 189);
			this.effectEditButton.Name = "effectEditButton";
			this.effectEditButton.Size = new System.Drawing.Size(94, 52);
			this.effectEditButton.TabIndex = 8;
			this.effectEditButton.Text = "Edit";
			this.effectEditButton.UseVisualStyleBackColor = true;
			// 
			// EditLayerForm
			// 
			this.AcceptButton = this.applyButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(354, 256);
			this.ControlBox = false;
			this.Controls.Add(this.effectEditButton);
			this.Controls.Add(this.effectRemoveButton);
			this.Controls.Add(this.effectAddButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.applyButton);
			this.Controls.Add(this.effectLabel);
			this.Controls.Add(this.checkedListBox1);
			this.Controls.Add(this.nameLabel);
			this.Controls.Add(this.nameBox);
			this.Name = "EditLayerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Edit Layer";
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.TextBox nameBox;
	private System.Windows.Forms.Label nameLabel;
	private System.Windows.Forms.CheckedListBox checkedListBox1;
	private System.Windows.Forms.Label effectLabel;
	private System.Windows.Forms.Button applyButton;
	private System.Windows.Forms.Button cancelButton;
	private System.Windows.Forms.Button effectAddButton;
	private System.Windows.Forms.Button effectRemoveButton;
	private System.Windows.Forms.Button effectEditButton;
}