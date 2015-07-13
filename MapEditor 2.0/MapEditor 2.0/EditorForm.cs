using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;
using System.Windows.Forms;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public class EditorForm : Context
{
	Editor editor;

	public EditorForm()
	{
		OnBegin += Begin;
		OnUpdate += Update;
		OnRender += Render;
	}

	protected override void OnKeyDown(KeyboardKeyEventArgs e)
	{
		if (e.Keyboard.IsKeyDown(Key.LControl))
		{
			if (e.Key == Key.S) editor.Save();
			if (e.Key == Key.O)
			{
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = "Prism Map (*.pm)|*.pm";
				dialog.Multiselect = false;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					editor = new Editor(this, dialog.FileName);
				}
			}
		}
	}

	void Begin()
	{
		editor = new Editor(this);
		DebugForm.editor = editor;
	}

	void Update()
	{
		editor.Update();
	}

	void Render()
	{
		editor.Render();
	}
}