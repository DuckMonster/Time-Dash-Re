using System;
using System.Drawing;
using System.Windows.Forms;

public partial class SolidColorPicker : EditorUIControl
{
	Color selectedColor = Color.Red;
	public TKTools.Color SelectedColor { get { return new TKTools.Color(selectedColor); } }

	public SolidColorPicker()
	{
		InitializeComponent();
		UpdateBox();
	}

	protected void ColorButtonClicked(object sender, EventArgs e)
	{
		ColorDialog diag = new ColorDialog();
		if (diag.ShowDialog() == DialogResult.OK)
		{
			selectedColor = diag.Color;
			UpdateBox();
		}
	}

	void UpdateBox()
	{
		colorBox.BackColor = selectedColor;
	}
}
