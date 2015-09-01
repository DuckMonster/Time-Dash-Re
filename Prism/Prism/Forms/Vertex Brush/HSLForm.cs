using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class HSLForm : Form
{
	EVertex[] vertices;

	public HSLForm(IList<EVertex> vertices)
	{
		InitializeComponent();

		this.vertices = new EVertex[vertices.Count];
		for (int i = 0; i < vertices.Count; i++)
			this.vertices[i] = vertices[i];
	}

	float GetValue(TrackBar bar)
	{
		return ((bar.Value / (float)bar.Maximum) - 0.5f) * 2f;
	}

	protected void HueChanged(object s, EventArgs e)
	{
		float value = GetValue(hueBar);

		foreach (EVertex v in vertices)
			v.Hue = value * 180f;
	}

	protected void SaturationChanged(object s, EventArgs e)
	{
		float value = GetValue(saturationBar);
		if (value > 0f) value *= 5f;

		foreach (EVertex v in vertices)
			v.Saturation = value + 1f;
	}

	protected void LightnessChanged(object s, EventArgs e)
	{
		float value = GetValue(lightnessBar);
		if (value > 0f) value *= 5f;

		foreach (EVertex v in vertices)
			v.Lightness = value + 1f;
	}
}
