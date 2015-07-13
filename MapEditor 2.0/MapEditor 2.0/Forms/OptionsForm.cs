using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public struct EditorOptions
{
	bool showGrid, focusLayer, meshBorders;
	float gridOpacity, layerOpacity, meshBorderOpacity;
	float gridSize;

	public bool ShowGrid { get { return showGrid; } set { showGrid = value; } }
	public bool FocusLayer { get { return focusLayer; } set { focusLayer = value; } }
	public bool MeshBorders { get { return meshBorders; } set { meshBorders = value; } }

	public float GridOpacity { get { return gridOpacity; } set { gridOpacity = value; } }
	public float GridSize { get { return gridSize; } set { gridSize = value; } }
	public float LayerOpacity { get { return layerOpacity; } set { layerOpacity = value; } }
	public float MeshBorderOpacity { get { return meshBorderOpacity; } set { meshBorderOpacity = value; } }

	public EditorOptions(bool showGrid, float gridSize, bool focusLayer, bool meshBorders,
		float gridOpacity, float layerOpacity, float meshBorderOpacity)
	{
		this.showGrid = showGrid;
		this.gridSize = gridSize;
		this.focusLayer = focusLayer;
		this.meshBorders = meshBorders;

		this.gridOpacity = gridOpacity;
		this.layerOpacity = layerOpacity;
		this.meshBorderOpacity = meshBorderOpacity;
	}
}

public partial class OptionsForm : Form
{
	public static EditorOptions options = new EditorOptions(true, 1f, true, true, 1f, 0.4f, 1f);

	public OptionsForm()
	{
		InitializeComponent();

		showGridBox.Checked = options.ShowGrid;
		gridSizeBox.Text = options.GridSize.ToString();
		gridOpacityBar.Value = (int)(options.GridOpacity * gridOpacityBar.Maximum);

		focusLayerBox.Checked = options.FocusLayer;
		layerOpacityBar.Value = (int)(options.LayerOpacity * layerOpacityBar.Maximum);

		meshBorderBox.Checked = options.MeshBorders;
		meshBorderOpacity.Value = (int)(options.MeshBorderOpacity * meshBorderOpacity.Maximum);
	}

	private void ShowGridChanged(object sender, EventArgs e)
	{
		options.ShowGrid = showGridBox.Checked;
	}

	private void GridOpacityChanged(object sender, EventArgs e)
	{
		options.GridOpacity = (float)gridOpacityBar.Value / gridOpacityBar.Maximum;
	}

	private void FocusLayerChanged(object sender, EventArgs e)
	{
		options.FocusLayer = focusLayerBox.Checked;
	}

	private void LayerOpacityChanged(object sender, EventArgs e)
	{
		options.LayerOpacity = (float)layerOpacityBar.Value / layerOpacityBar.Maximum;
	}

	private void MeshBordersChanged(object sender, EventArgs e)
	{
		options.MeshBorders = meshBorderBox.Checked;
	}

	private void MeshBorderOpacityChanged(object sender, EventArgs e)
	{
		options.MeshBorderOpacity = (float)meshBorderOpacity.Value / meshBorderOpacity.Maximum;
	}

	private void GridSizeChanged(object sender, EventArgs e)
	{
		float f = 0f;

		if (Single.TryParse(gridSizeBox.Text, out f))
		{
			options.GridSize = f;
			gridSizeBox.BackColor = Color.White;
		}
		else
		{
			gridSizeBox.BackColor = Color.Red;
		}
	}
}
