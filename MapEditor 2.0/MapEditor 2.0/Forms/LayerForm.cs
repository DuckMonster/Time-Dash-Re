using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class LayerForm : Form
{
	class LayerItem : ListViewItem
	{
		public Layer layer;

		public LayerItem(Layer l)
			:base(l.Name)
		{
			layer = l;
		}
	}

	public LayerForm()
	{
		InitializeComponent();
		UpdateLayers();
	}

	public void UpdateLayers()
	{
		layerList.Items.Clear();

		List<Layer> layers = Editor.CurrentEditor.layerList;
		foreach (Layer l in layers)
		{
			ListViewItem item = layerList.Items.Add(new LayerItem(l));
			item.ForeColor = l.Visible ? Color.Black : Color.Gray;

			if (Editor.CurrentEditor.activeLayers.Contains(l))
				item.Selected = true;
		}
	}

	private void SelectedLayerChanged(object sender, EventArgs e)
	{
		List<Layer> selectedLayers = new List<Layer>();

		foreach (LayerItem i in layerList.Items)
		{
			i.Font = new Font(i.Font, i.Selected ? FontStyle.Bold : FontStyle.Regular);
			if (i.Selected)
				selectedLayers.Add(i.layer);
		}

		Editor.CurrentEditor.SetActiveLayers(selectedLayers);

		if (layerList.SelectedItems.Count > 0)
			visibleCheckBox.Checked = (layerList.SelectedItems[0] as LayerItem).layer.Visible;

		deleteButton.Enabled = editButton.Enabled = visibleCheckBox.Enabled = layerList.SelectedItems.Count > 0;
	}

	private void AddButtonClicked(object sender, EventArgs e)
	{
		CreateLayerForm f = new CreateLayerForm();

		if (f.ShowDialog() == DialogResult.OK)
			Editor.CurrentEditor.CreateLayer(f.LayerName);

		UpdateLayers();
	}

	private void DeleteButtonClicked(object sender, EventArgs e)
	{
		if (MessageBox.Show(string.Format("Are you sure you want to delete ({0}) layers?", layerList.SelectedItems.Count), "Confirm", MessageBoxButtons.YesNo) == DialogResult.No) return;
		foreach (LayerItem l in layerList.SelectedItems)
			Editor.CurrentEditor.RemoveLayer(l.layer);

		UpdateLayers();
	}

	private void EditButtonClicked(object sender, EventArgs e)
	{
		Layer[] selectedLayers = new Layer[layerList.SelectedItems.Count];

		for (int i = 0; i < layerList.SelectedItems.Count; i++)
			selectedLayers[i] = (layerList.SelectedItems[i] as LayerItem).layer;

		EditLayer(selectedLayers);
	}

	private void OnMouseDoubleClick(object sender, MouseEventArgs e)
	{
		EditLayer(new Layer[] { (layerList.SelectedItems[0] as LayerItem).layer });
	}

	private void EditLayer(IList<Layer> l)
	{
		new EditLayerForm(l[0]).ShowDialog();
		UpdateLayers();
	}

	private void ToggleVisible(object sender, EventArgs e)
	{
		foreach (LayerItem i in layerList.SelectedItems)
		{
			i.layer.Visible = visibleCheckBox.Checked;
			i.ForeColor = visibleCheckBox.Checked ? Color.Black : Color.Gray;
		}
	}

	private void OnClose(object sender, FormClosingEventArgs e)
	{
		this.Hide();
		e.Cancel = true;
	}
}
