using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class EditLayerForm : Form
{
	Layer layer;

	public EditLayerForm(Layer layer)
	{
		InitializeComponent();

		this.layer = layer;
		nameBox.Text = layer.Name;
	}

	private void NameBoxChanged(object sender, EventArgs a)
	{
		applyButton.Enabled = nameBox.Text.Length > 0;
	}

	private void ApplyClicked(object sender, EventArgs a)
	{
		layer.Name = nameBox.Text;

		DialogResult = DialogResult.OK;
		Close();
	}

	private void CancelClicked(object sender, EventArgs a)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}
}