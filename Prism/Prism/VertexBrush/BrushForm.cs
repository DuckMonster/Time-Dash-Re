using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKTools.Mathematics;

public partial class BrushForm : Form
{
	class TargetItem
	{
		readonly PenTarget target;
		public PenTarget Target { get { return target; } }

		public TargetItem(PenTarget t)
		{
			target = t;
		}

		public override string ToString()
		{
			return target.ToString();
		}
	}

	VertexPen painter;

	public float Hue
	{
		get
		{
			return GetValue(hueBar) * 180f;
		}
		set
		{
			float v = MathHelper.Clamp(value + 180, 0f, 360f);
			hueBar.Value = (int)(v / 360f * hueBar.Maximum);
		}
	}

	public float Saturation
	{
		get
		{
			float v = GetValue(saturationBar);
			if (v > 0f) v *= 5f;

			return v + 1f;
		}
		set
		{
			float v = MathHelper.Clamp(value, 0f, 6f);
			if (v > 1f) v /= 5f;

			saturationBar.Value = (int)(v / 2f * saturationBar.Maximum);
		}
	}

	public float Lightness
	{
		get
		{
			float v = GetValue(lightnessBar);
			if (v > 0f) v *= 5f;

			return v + 1f;
		}
		set
		{
			float v = MathHelper.Clamp(value, 0f, 6f);
			if (v > 1f) v /= 5f;

			lightnessBar.Value = (int)(v / 2f * lightnessBar.Maximum);
		}
	}

	public float Opacity
	{
		get { return opacityBar.Value / (float)opacityBar.Maximum; }
		set
		{
			float v = MathHelper.Clamp(value, 0f, 1f);
			opacityBar.Value = (int)(v * opacityBar.Maximum);
		}
	}
	public float Hardness
	{
		get { return hardnessBar.Value / (float)hardnessBar.Maximum; }
		set
		{
			float v = MathHelper.Clamp(value, 0f, 1f);
			hardnessBar.Value = (int)(v * hardnessBar.Maximum);
		}
	}
	public float Size
	{
		get
		{
			float v = (sizeBar.Value / (float)sizeBar.Maximum - 0.5f) * 2;
			return (float)Math.Pow(6.7f, v);
		}
		set
		{
			float v = MathHelper.Clamp(((float)Math.Log(value, 6.7f) + 1) / 2, 0f, 1f);
			sizeBar.Value = (int)(v * sizeBar.Maximum);
		}
	}

	public BrushForm(VertexPen painter)
	{
		InitializeComponent();
		this.painter = painter;

		Hue = painter.Brush.HSL.H;
		Saturation = painter.Brush.HSL.S;
		Lightness = painter.Brush.HSL.L;

		Opacity = painter.Brush.Opacity;
		Hardness = painter.Brush.Hardness;
		Size = painter.Brush.Size;

		targetBox.Items.Add(new TargetItem(PenTarget.All));
		targetBox.Items.Add(new TargetItem(PenTarget.Layer));
		targetBox.Items.Add(new TargetItem(PenTarget.Selected));

		targetBox.SelectedIndex = (int)painter.Target;
	}

	float GetValue(TrackBar bar)
	{
		return ((bar.Value / (float)bar.Maximum) - 0.5f) * 2f;
	}

	void UpdateBrush()
	{
		painter.Brush = new VertexBrush(new TKTools.ColorHSL(Hue, Saturation, Lightness), Size, Opacity, Hardness);
	}

	bool disableSliderChange = false;
	protected void HSLSliderChanged(object sender, EventArgs e)
	{
		if (disableSliderChange) return;

		UpdateHSLBoxes();
		UpdateBrush();
	}

	bool disableBoxChange = false;
	protected void HSLBoxChanged(object sender, EventArgs e)
	{
		if (disableBoxChange) return;

		UpdateHSLSliders();
		UpdateBrush();
	}

	void UpdateHSLSliders()
	{
		disableSliderChange = true;

		float h = 0f, s = 1f, l = 1f;
		Single.TryParse(hueBox.Text, NumberStyles.Any, null, out h);
		Single.TryParse(saturationBox.Text, NumberStyles.Any, null, out s);
		Single.TryParse(lightnessBox.Text, NumberStyles.Any, null, out l);

		Hue = h;
		Saturation = s;
		Lightness = l;

		disableSliderChange = false;
	}

	void UpdateHSLBoxes()
	{
		disableBoxChange = true;

		hueBox.Text = Hue.ToString();
		saturationBox.Text = Saturation.ToString();
		lightnessBox.Text = Lightness.ToString();

		disableBoxChange = false;
	}

	protected void SliderClicked(object sender, MouseEventArgs e)
	{
		TrackBar b = sender as TrackBar;

		if (e.Button == MouseButtons.Left && ModifierKeys.HasFlag(Keys.Shift))
		{
			b.Value = b.Maximum / 2;
			UpdateHSLBoxes();
		}
	}

	protected void OHSSliderChanged(object sender, EventArgs e)
	{
		UpdateBrush();
		//brushPreview.Invalidate();
	}

	protected void OHSBoxChanged(object sender, EventArgs e)
	{
		UpdateBrush();
		//new Thread(UpdateBrushPreview).Start();
	}

	protected void OHMouseReleased(object sender, MouseEventArgs e)
	{
		brushPreview.Invalidate();
	}

	protected void BrushPreviewPaint(object sender, PaintEventArgs e)
	{
		Graphics g = e.Graphics;
		int w = e.ClipRectangle.Width, h = e.ClipRectangle.Height;

		g.FillRectangle(new SolidBrush(Color.Black), e.ClipRectangle);

		for (int x = 0; x < w; x++)
			for (int y = 0; y < h; y++)
			{
				Vector2 point = new Vector2((x / (float)w - 0.5f) * 2, (y / (float)h - 0.5f) * 2);

				float a = 1f - point.Length;

				if (point.Length > Hardness)
					a = a / (1f - Hardness);
				else
					a = 1f;

				a = MathHelper.Clamp(a, 0f, 1f) * Opacity;

				g.FillRectangle(new SolidBrush(Color.FromArgb((int)(a * 255), 255, 255, 255)), x, y, 1, 1);
			}
	}

	private void TargetChanged(object sender, EventArgs e)
	{
		painter.Target = (targetBox.SelectedItem as TargetItem).Target;
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		painter.ClosePalette();
	}
}