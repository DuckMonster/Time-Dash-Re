namespace MapEditor
{
	partial class EventForm
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
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.eventList = new System.Windows.Forms.ListView();
			this.eventID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.eventName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonEdit = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.textBoxParams = new System.Windows.Forms.TextBox();
			this.lblParams = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// eventList
			// 
			this.eventList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.eventID,
            this.eventName});
			this.eventList.FullRowSelect = true;
			this.eventList.Location = new System.Drawing.Point(12, 12);
			this.eventList.Name = "eventList";
			this.eventList.Size = new System.Drawing.Size(478, 210);
			this.eventList.TabIndex = 0;
			this.eventList.UseCompatibleStateImageBehavior = false;
			this.eventList.View = System.Windows.Forms.View.Details;
			this.eventList.SelectedIndexChanged += new System.EventHandler(this.eventList_SelectedIndexChanged);
			this.eventList.DoubleClick += new System.EventHandler(this.eventList_DoubleClick);
			// 
			// eventID
			// 
			this.eventID.Text = "ID";
			this.eventID.Width = 31;
			// 
			// eventName
			// 
			this.eventName.Text = "EventName";
			this.eventName.Width = 185;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(13, 292);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(142, 35);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonEdit
			// 
			this.buttonEdit.Location = new System.Drawing.Point(160, 292);
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.Size = new System.Drawing.Size(49, 22);
			this.buttonEdit.TabIndex = 2;
			this.buttonEdit.Text = "Edit";
			this.buttonEdit.UseVisualStyleBackColor = true;
			this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(406, 292);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(84, 35);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(309, 292);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(49, 22);
			this.buttonAdd.TabIndex = 4;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Location = new System.Drawing.Point(215, 292);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(58, 22);
			this.buttonRemove.TabIndex = 5;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// textBoxParams
			// 
			this.textBoxParams.Location = new System.Drawing.Point(13, 260);
			this.textBoxParams.Name = "textBoxParams";
			this.textBoxParams.Size = new System.Drawing.Size(477, 20);
			this.textBoxParams.TabIndex = 6;
			this.textBoxParams.TextChanged += new System.EventHandler(this.textBoxParams_TextChanged);
			// 
			// lblParams
			// 
			this.lblParams.AutoSize = true;
			this.lblParams.Location = new System.Drawing.Point(12, 244);
			this.lblParams.Name = "lblParams";
			this.lblParams.Size = new System.Drawing.Size(60, 13);
			this.lblParams.TabIndex = 7;
			this.lblParams.Text = "Parameters";
			// 
			// EventForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 339);
			this.Controls.Add(this.lblParams);
			this.Controls.Add(this.textBoxParams);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonEdit);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.eventList);
			this.Name = "EventForm";
			this.Text = "EventForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.ListView eventList;
		private System.Windows.Forms.ColumnHeader eventID;
		private System.Windows.Forms.ColumnHeader eventName;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonEdit;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.TextBox textBoxParams;
		private System.Windows.Forms.Label lblParams;
	}
}