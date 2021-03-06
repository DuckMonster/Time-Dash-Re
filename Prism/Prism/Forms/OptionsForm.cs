﻿using INI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class OptionsForm : Form
{
	public static EditorOptions options = new EditorOptions();

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

		colorBoxL.BackColor = options.BackgroundColorLeft;
		colorBoxR.BackColor = options.BackgroundColorRight;
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

	protected override void OnClosing(CancelEventArgs e)
	{
		options.Save();
	}

	private void BackColorLeftClick(object sender, EventArgs e)
	{
		if (colorDialog.ShowDialog() == DialogResult.OK)
		{
			colorBoxL.BackColor = colorDialog.Color;
			options.BackgroundColorLeft = colorDialog.Color;
		}

		backColorPreview.Invalidate();
	}

	private void BackColorRightClick(object sender, EventArgs e)
	{
		if (colorDialog.ShowDialog() == DialogResult.OK)
		{
			colorBoxR.BackColor = colorDialog.Color;
			options.BackgroundColorRight = colorDialog.Color;
		}

		backColorPreview.Invalidate();
	}

	private void PreviewPaint(object sender, PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		int width = backColorPreview.Width;

		LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(width, 0), options.BackgroundColorLeft, options.BackgroundColorRight);
		g.FillRectangle(brush, new Rectangle(Point.Empty, backColorPreview.Size));
	}
}


public class EditorOptions
{
	bool showGrid, focusLayer, meshBorders;
	float gridOpacity, layerOpacity, meshBorderOpacity;
	float gridSize;
	Color backgroundColorLeft, backgroundColorRight;

	public bool ShowGrid { get { return showGrid; } set { showGrid = value; } }
	public bool FocusLayer { get { return focusLayer; } set { focusLayer = value; } }
	public bool MeshBorders { get { return meshBorders; } set { meshBorders = value; } }

	public float GridOpacity { get { return gridOpacity; } set { gridOpacity = value; } }
	public float GridSize { get { return gridSize; } set { gridSize = value; } }
	public float LayerOpacity { get { return layerOpacity; } set { layerOpacity = value; } }
	public float MeshBorderOpacity { get { return meshBorderOpacity; } set { meshBorderOpacity = value; } }

	public Color BackgroundColorLeft { get { return backgroundColorLeft; } set { backgroundColorLeft = value; } }
	public Color BackgroundColorRight { get { return backgroundColorRight; } set { backgroundColorRight = value; } }

	public EditorOptions()
	{
		Load();
	}

	public EditorOptions(bool showGrid, float gridSize, bool focusLayer, bool meshBorders,
		float gridOpacity, float layerOpacity, float meshBorderOpacity, Color backgroundColorLeft, Color backgroundColorRight)
	{
		this.showGrid = showGrid;
		this.gridSize = gridSize;
		this.focusLayer = focusLayer;
		this.meshBorders = meshBorders;

		this.gridOpacity = gridOpacity;
		this.layerOpacity = layerOpacity;
		this.meshBorderOpacity = meshBorderOpacity;

		this.backgroundColorLeft = backgroundColorLeft;
		this.backgroundColorRight = backgroundColorRight;

	}

	void SetDefault()
	{
		showGrid = true;
		gridSize = 1f;
		gridOpacity = 1f;

		focusLayer = true;
		layerOpacity = 0.4f;

		meshBorders = true;
		meshBorderOpacity = 1f;

		backgroundColorLeft = Color.FromArgb(0, 0, 22);
		backgroundColorRight = Color.Black;
	}

	public void Load()
	{
		try
		{
			IniFile file = new IniFile("config.cfg");

			IniFile.Section s = file["editor"];
			ShowGrid = s["showGrid"] == "1" ? true : false;
			GridOpacity = Single.Parse(s["gridOpacity"], System.Globalization.NumberStyles.Any);
			GridSize = Single.Parse(s["gridSize"], System.Globalization.NumberStyles.Any);

			FocusLayer = s["focusLayer"] == "1" ? true : false;
			LayerOpacity = Single.Parse(s["layerOpacity"], System.Globalization.NumberStyles.Any);

			MeshBorders = s["meshBorders"] == "1" ? true : false;
			MeshBorderOpacity = Single.Parse(s["meshBorderOpacity"], System.Globalization.NumberStyles.Any);

			BackgroundColorLeft = Color.FromArgb(int.Parse(s["backgroundColorLeft"]));
			BackgroundColorRight = Color.FromArgb(int.Parse(s["backgroundColorRight"]));
		}
		catch (Exception e)
		{
			SetDefault();
		}
	}

	public void Save()
	{
		IniFile file = new IniFile();

		IniFile.Section s = file["editor"];

		s["showGrid"] = showGrid ? "1" : "0";
		s["gridOpacity"] = gridOpacity.ToString();
		s["gridSize"] = gridSize.ToString();

		s["focusLayer"] = focusLayer ? "1" : "0";
		s["layerOpacity"] = LayerOpacity.ToString();

		s["meshBorders"] = meshBorders ? "1" : "0";
		s["meshBorderOpacity"] = meshBorderOpacity.ToString();

		s["backgroundColorLeft"] = backgroundColorLeft.ToArgb().ToString();
		s["backgroundColorRight"] = backgroundColorRight.ToArgb().ToString();

		file.SaveTo("config.cfg");
	}

	void CreateStandard()
	{
		IniFile file = new IniFile("config.cfg");
	}
}