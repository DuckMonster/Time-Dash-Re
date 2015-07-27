using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class CreateLayerForm : Form
{
	string layerName;
	public string LayerName
	{
		get { return layerName; }
	}

	public CreateLayerForm()
	{
		InitializeComponent();
		createButton.Enabled = false;
	}

	public void NameChanged(object sender, EventArgs a)
	{
		createButton.Enabled = nameBox.Text.Length > 0;
	}

	public void OnCreatePressed(object sender, EventArgs a)
	{
		layerName = nameBox.Text;
		DialogResult = DialogResult.OK;
		Close();
	}

	public void OnClosePressed(object sender, EventArgs a)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}
}