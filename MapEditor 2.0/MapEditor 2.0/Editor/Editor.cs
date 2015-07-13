using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;
using System.Windows.Forms;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

public enum SelectMode
{
	Mesh,
	Vertices
}

public enum ManipulatorMode
{
	None = 0,
	Translate = 1,
	Rotate = 2,
	Scale = 3
}

public partial class Editor
{
	public const uint FileStructureVersion = 0;

	public static Editor CurrentEditor;
	public static MouseWatch mouse;
	public static KeyboardWatch keyboard;

	public EditorForm form;

	CameraControl cameraControl;

	public List<Layer> activeLayers = new List<Layer>();
	public List<Layer> layerList = new List<Layer>();

	public List<TextureSet> textureSetList = new List<TextureSet>();

	public List<EVertex> selectedVertices = new List<EVertex>();

	SelectMode selectMode = SelectMode.Mesh;
	public SelectMode SelectMode
	{
		get { return selectMode; }
	}

	public Camera editorCamera;

	Mesh gridMesh;

	public SelectionBox selectionBox;
	public MeshCreator meshCreator;

	ManipulatorMode manipulatorMode = ManipulatorMode.None;
	Manipulator[] manipulators;

	public bool DisableSelect
	{
		get { return Manipulator.Active || Manipulator.Hovered || cameraControl.Active; }
	}

	public List<Layer> ActiveLayers
	{
		get
		{
			return activeLayers;
		}
	}

	public List<EVertex> SelectedVertices
	{
		get
		{
			return selectedVertices;
		}
	}

	public IEnumerable<EMesh> Meshes
	{
		get
		{
			foreach (Layer l in layerList)
				foreach (EMesh m in l)
					yield return m;
		}
	}

	public IList<EMesh> HoveredMeshes
	{
		get
		{
			if (selectionBox.Active)
			{
				return selectionBox.GetSelectedMeshes();
			} else
			{
				foreach (Layer l in activeLayers)
					for (int i = l.Meshes.Count - 1; i >= 0; i--)
						if (l.Meshes[i].Intersects(new Polygon(new Vector3[] { mouse.Position })))
							return new EMesh[] { l.Meshes[i] };
			}

			return new EMesh[] { };
		}
	}

	public Manipulator Manipulator
	{
		get
		{
			return manipulators[(int)manipulatorMode];
		}
	}

	public Editor(EditorForm form)
	{
		CurrentEditor = this;

		this.form = form;

		editorCamera = new Camera();
		editorCamera.Use();

		mouse = new MouseWatch();
		mouse.Perspective = editorCamera;

		keyboard = new KeyboardWatch();

		manipulators = new Manipulator[] { new Manipulator(this), new ManipulatorTranslate(this), new ManipulatorRotate(this), new ManipulatorScale(this) };

		gridMesh = new Mesh();
		gridMesh.PrimitiveType = PrimitiveType.Lines;

		List<Vector2> gridVertices = new List<Vector2>();
		for (int i = -50; i <= 50; i++)
		{
			gridVertices.Add(new Vector2(-50f, i));
			gridVertices.Add(new Vector2(50f, i));
			gridVertices.Add(new Vector2(i, -50f));
			gridVertices.Add(new Vector2(i, 50f));
		}

		gridMesh.Vertices2 = gridVertices.ToArray();

		cameraControl = new CameraControl(this);
		selectionBox = new SelectionBox(this);
		meshCreator = new MeshCreator(this);

		activeLayers.Add(CreateLayer("default"));
    }
	public Editor(EditorForm form, string filename)
		: this(form)
	{
		LoadFrom(filename);
	}

	public Layer CreateLayer(string n) { return CreateLayer(new Layer(n, this)); }
	public Layer CreateLayer(Layer l)
	{
		layerList.Add(l);

		if (Program.layerForm != null)
			Program.layerForm.UpdateLayers();
		return l;
	}

	public void RemoveLayer(Layer l)
	{
		layerList.Remove(l);

		if (Program.layerForm != null)
			Program.layerForm.UpdateLayers();
	}

	public void SetActiveLayers(IList<Layer> layers)
	{
		foreach (Layer l in activeLayers)
			if (!layers.Contains(l))
				foreach (EMesh m in l)
					if (m.Selected) m.Selected = false;

		activeLayers.Clear();
		ActiveLayers.AddRange(layers);

		if (Program.layerForm != null)
			Program.layerForm.UpdateLayers();
	}

	public void CreateTextureSet(TextureSet s)
	{
		textureSetList.Add(s);

		if (Program.tilePicker != null)
			Program.tilePicker.AddTexture(s);
	}

	public void RemoveTextureSet(TextureSet s)
	{
		textureSetList.Remove(s);
		s.Texture.Dispose();
		s.Bitmap.Dispose();

		if (Program.tilePicker != null)	
			Program.tilePicker.RemoveTexture(s);
	}

	public void CreateMesh(System.Drawing.RectangleF rect, TextureSet.Tile tile)
	{
		if (activeLayers.Count == 0 || activeLayers.Count > 1) return;

		EMesh mesh;

		if (tile != null)
			mesh = new EMesh(rect, tile, activeLayers[0], this);
		else
			mesh = new EMesh(rect, activeLayers[0], this);

		activeLayers[0].AddMesh(mesh);
		SetSelected(new EMesh[] { mesh });
	}

	public void RemoveMesh(EMesh m)
	{
		m.Selected = false;

		m.Dispose();
		m.Layer.RemoveMesh(m);
	}

	public void Update()
	{
		mouse.PlaneDistance = editorCamera.Position.Z;

		cameraControl.Logic();
		editorCamera.Position = CameraControl.Position;
		editorCamera.Target = CameraControl.Position * new Vector3(1, 1, 0);

		if (keyboard.KeyPressed(Key.Tab))
			selectMode = selectMode == SelectMode.Mesh ? SelectMode.Vertices : SelectMode.Mesh;

		if (keyboard.KeyReleased(Key.Delete))
		{
			foreach (EMesh m in Meshes)
				if (m.Selected)
					RemoveMesh(m);

			DeselectAll();
		}

		//MANIPULATOR CONTROL
		if (keyboard.KeyPressed(Key.Q)) manipulatorMode = ManipulatorMode.None;
		if (keyboard.KeyPressed(Key.W)) manipulatorMode = ManipulatorMode.Translate;
		if (keyboard.KeyPressed(Key.E)) manipulatorMode = ManipulatorMode.Rotate;
		if (keyboard.KeyPressed(Key.R)) manipulatorMode = ManipulatorMode.Scale;

		foreach (Manipulator m in manipulators)
			m.UpdatePivot();

		//OPTIONS HOTKEYS
		if (keyboard[Key.LControl] && keyboard[Key.LShift])
		{
			if (keyboard.KeyPressed(Key.G)) OptionsForm.options.ShowGrid = !OptionsForm.options.ShowGrid;
			if (keyboard.KeyPressed(Key.L)) OptionsForm.options.FocusLayer = !OptionsForm.options.FocusLayer;
			if (keyboard.KeyPressed(Key.B)) OptionsForm.options.MeshBorders = !OptionsForm.options.MeshBorders;
		}

		selectionBox.Logic();
		meshCreator.Logic();

		foreach (EMesh m in Meshes)
			m.Logic();

		Manipulator.Logic();
	}

	public void Render()
	{
		if (OptionsForm.options.ShowGrid)
		{
			float a = OptionsForm.options.GridOpacity;
			float size = OptionsForm.options.GridSize;
			gridMesh.Color = new Color(0.4f, 0.4f, 0.4f, a);

			GL.LineWidth(1f);

			gridMesh.Reset();
			gridMesh.Scale(size);
			gridMesh.Draw();

			gridMesh.Color = new Color(0.8f, 0.8f, 0.8f, a);

			GL.LineWidth(3f);

			gridMesh.Scale(10f);
			gridMesh.Draw();

			GL.LineWidth(1f);
		}

		foreach (EMesh m in Meshes)
			m.Draw();
		foreach (EMesh m in Meshes)
			m.DrawUI();

		Manipulator.Draw();

		selectionBox.Draw();
		meshCreator.Draw();
	}
}