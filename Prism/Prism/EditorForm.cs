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
	#region Screen Clearing
	const string screenClearVertex = @"
#version 330

in vec2 vertexPosition;
in vec3 vertexColor;

out vec3 color;

void main() {
	color = vertexColor;
	gl_Position = vec4(vertexPosition, 0.0, 1.0);
}
";

	const string screenClearFragment = @"
#version 330

in vec3 color;
out vec4 fragment;

void main() {
	fragment = vec4(color, 1.0);
}
";

	ShaderProgram screenClearProgram;
	Mesh screenClearMesh;
	#endregion

	Editor editor;

	public EditorForm()
	{
		OnBegin += Begin;
		OnUpdate += Update;
		OnRender += Render;
	}

	void UpdateTitle()
	{
		string title = "Prism Editor - ";

		if (editor.Filename == null)
			title += "Untitled Map";
		else
			title += editor.Filename;

		Title = title;
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

					Program.NewEditorInstance(editor);
				}
			}
		}
	}

	void Begin()
	{
		editor = new Editor(this);
		Program.NewEditorInstance(editor);

		screenClearProgram = new ShaderProgram(screenClearVertex, screenClearFragment);
		screenClearMesh = new Mesh(screenClearProgram);

		screenClearMesh.GetAttribute<Vector2>("vertexPosition").Data = new Vector2[]
		{
			new Vector2(-1f, -1f),
			new Vector2(1f, -1f),
			new Vector2(1f, 1f),
			new Vector2(-1f, 1f)
		};

		EMesh.CompileProgram();
	}

	void Update()
	{
		editor.Update();
		UpdateTitle();
	}

	void Render()
	{
		System.Drawing.Color color1 = OptionsForm.options.BackgroundColorLeft, color2 = OptionsForm.options.BackgroundColorRight;
		Vector3 c1 = new Vector3(color1.R / 255f, color1.G / 255f, color1.B / 255f), 
			c2 = new Vector3(color2.R / 255f, color2.G / 255f, color2.B / 255f);

		screenClearMesh.GetAttribute<Vector3>("vertexColor").Data = new Vector3[]
		{
			c2, c2, c1, c1
		};

		screenClearMesh.Draw();

		editor.Render();
	}
}