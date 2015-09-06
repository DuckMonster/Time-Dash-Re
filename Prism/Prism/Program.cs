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
	public static LayerFormAdv layerForm;
	public static TextureForm textureForm;
	public static OutlinerForm outlinerForm;
	public static MeshPicker meshPicker;
	public static TilePicker tilePicker;

	KeyboardWatch keyboard = new KeyboardWatch();

	static IEnumerable<EditorUIForm> Forms
	{
		get
		{
			yield return layerForm;
			yield return textureForm;
			yield return meshPicker;
			yield return debugForm;
			yield return outlinerForm;
		}
	}

	static IEnumerable<EditorUIControl> Controls
	{
		get
		{
			yield return tilePicker;
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
		debugForm.Visible = false;

		layerForm = new LayerFormAdv();
		layerForm.Visible = false;

		textureForm = new TextureForm();
		textureForm.Visible = false;

		meshPicker = new MeshPicker();
		meshPicker.Visible = false;

		outlinerForm = new OutlinerForm();
		outlinerForm.Visible = false;

		tilePicker = meshPicker.TilePicker;
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
			meshPicker.Visible = !meshPicker.Visible;
		if (keyboard.KeyReleased(Key.F5))
			outlinerForm.Visible = !outlinerForm.Visible;

		if (keyboard.KeyReleased(Key.F12))
			new OptionsForm().Show();
	}

	public static void NewEditorInstance(Editor e)
	{
		foreach (EditorUIForm f in Forms)
			f.Editor = e;

		foreach (EditorUIControl c in Controls)
			c.Editor = e;
	}

	public static void UpdateUI()
	{
		foreach (EditorUIForm f in Forms)
			f.UpdateUI();

		foreach (EditorUIControl c in Controls)
			c.UpdateUI();
	}
}