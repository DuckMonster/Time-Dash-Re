using OpenTK.Input;
using System;
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
	public static OptionsForm optionsForm;

	KeyboardWatch keyboard = new KeyboardWatch();

	public Program()
	{
		Application.EnableVisualStyles();

		e = new EditorForm();

		e.OnUpdate += Update;
		e.OnBegin += Load;

		e.Run();
	}

	void Load()
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

		optionsForm = new OptionsForm();
		optionsForm.Show();
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
	}
}