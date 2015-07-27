using OpenTK.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using TKTools.Context;
using TKTools.Context.Input;

public class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		Program p = new Program();
	}

	EditorForm e;

	public static DebugForm debugForm;
	public static LayerForm layerForm;
	public static TextureForm textureForm;
	public static TilePicker tilePicker; 

	KeyboardWatch keyboard = new KeyboardWatch();

	static IEnumerable<EditorUIForm> Forms
	{
		get
		{
			yield return layerForm;
			yield return textureForm;
			yield return tilePicker;
			yield return debugForm;
		}
	}

	public Program()
	{
		Application.EnableVisualStyles();

		LoadForms();

		e = new EditorForm();
		e.OnUpdate += Update;
		e.Run();
	}

	void LoadForms()
	{
		debugForm = new DebugForm();
		debugForm.Show();
		debugForm.Visible = false;

		layerForm = new LayerForm();
		layerForm.Show();
		layerForm.Visible = false;

		textureForm = new TextureForm();
		textureForm.Show();
		textureForm.Visible = false;

		tilePicker = new TilePicker();
		tilePicker.Show();
		tilePicker.Visible = false;
	}

	void Update()
	{
		debugForm.Logic();

		if (keyboard.KeyReleased(Key.F1))
			debugForm.Visible = !debugForm.Visible;
		if (keyboard.KeyReleased(Key.F2))
			layerForm.Visible = !layerForm.Visible;
		if (keyboard.KeyReleased(Key.F3))
			textureForm.Visible = !textureForm.Visible;
		if (keyboard.KeyReleased(Key.F4))
			tilePicker.Visible = !tilePicker.Visible;

		if (keyboard.KeyReleased(Key.F12))
			new OptionsForm().Show();
	}

	public static void NewEditorInstance(Editor e)
	{
		foreach (EditorUIForm f in Forms)
			f.Editor = e;
	}

	public static void UpdateUI()
	{
		foreach (EditorUIForm f in Forms)
			f.UpdateUI();
	}
}