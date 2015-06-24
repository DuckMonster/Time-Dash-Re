using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TKTools.Context.Input;

public partial class DebugForm : Form
{
	MouseWatch mouse;

	public DebugForm()
	{
		InitializeComponent();

		mouse = new MouseWatch();
		mouse.Perspective = Editor.CurrentEditor.editorCamera;
	}

	public void Logic()
	{
		mouse.PlaneDistance = mouse.Perspective.Position.Z;

		textPositionX.Text = "X: " + mouse.Position.X;
		textPositionY.Text = "Y: " + mouse.Position.Y;
	}
}
