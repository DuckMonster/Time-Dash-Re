﻿using System;
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
	public static string debugString = "";

	private const int WM_NCHITTEST = 0x84;
	private const int HTCLIENT = 0x1;
	private const int HTCAPTION = 0x2;

	public static Editor editor;
	MouseWatch mouse;

	public DebugForm()
	{
		InitializeComponent();

		mouse = new MouseWatch();
		mouse.Perspective = Editor.CurrentEditor.editorCamera;
	}

	protected override void WndProc(ref Message m)
	{
		switch (m.Msg)
		{
			case WM_NCHITTEST:
				base.WndProc(ref m);
				if ((int)m.Result == HTCLIENT)
					m.Result = (IntPtr)HTCAPTION;
				return;
		}
		base.WndProc(ref m);
	}

	public void Logic()
	{
		variousBox.Text = debugString;
	}
}