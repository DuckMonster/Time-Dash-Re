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
	Scale = 3,
	Paint = 4
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
	public LayerNode rootLayer;

	public List<TextureSet> textureSetList = new List<TextureSet>();
	public VertexCollection selectedVertices = new VertexCollection();

	SelectMode selectMode = SelectMode.Mesh;
	public SelectMode SelectMode
	{
		get { return selectMode; }
	}

	public Camera editorCamera;

	Model gridModel;

	public SelectionBox selectionBox;
	public MeshCreator meshCreator;

	ManipulatorMode manipulatorMode = ManipulatorMode.None;
	Manipulator[] manipulators;

	public bool DisableSelect
	{
		get { return Manipulator.Active || Manipulator.Hovered || cameraControl.Active || manipulatorMode == ManipulatorMode.Paint; }
	}

	public IEnumerable<Layer> Layers
	{
		get { foreach (Layer l in rootLayer.LayersInverted) yield return l; }
	}

	public List<Layer> ActiveLayers
	{
		get
		{
			return activeLayers;
		}
	}

	public HistorySystem ActiveHistory
	{
		get
		{
			if (ActiveLayers.Count > 0)
				return ActiveLayers[0].History;
			else
				return null;
		}
	}

	public VertexCollection SelectedVertices
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
			foreach (Layer l in Layers)
				foreach (EMesh m in l)
					yield return m;
		}
	}

	public IEnumerable<EMesh> SelectedMeshes
	{
		get
		{
			foreach (EMesh m in Meshes)
				if (m.Selected)
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
			}
			else
			{
				if (Program.outlinerForm.HoveredNode != null)
					return new EMesh[] { Program.outlinerForm.HoveredNode };

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

		manipulators = new Manipulator[] { new Manipulator(this), new ManipulatorTranslate(this), new ManipulatorRotate(this), new ManipulatorScale(this), new VertexPen(this) };

		gridModel = new Model();
		gridModel.PrimitiveType = PrimitiveType.Lines;

		List<Vector3> gridVertices = new List<Vector3>();
		for (int i = -50; i <= 50; i++)
		{
			gridVertices.Add(new Vector3(-50f, i, 0));
			gridVertices.Add(new Vector3(50f, i, 0));
			gridVertices.Add(new Vector3(i, -50f, 0));
			gridVertices.Add(new Vector3(i, 50f, 0));
		}

		gridModel.VertexPosition = gridVertices.ToArray();

		cameraControl = new CameraControl(this);
		selectionBox = new SelectionBox(this);
		meshCreator = new MeshCreator(this);

		rootLayer = new LayerNode("root", this);
	}
	public Editor(EditorForm form, string filename)
		: this(form)
	{
		LoadFrom(filename);
	}

	public void SetActiveLayers(IList<Layer> layers)
	{
		foreach (Layer l in activeLayers)
			if (!layers.Contains(l))
				foreach (EMesh m in l)
					if (m.Selected) m.Selected = false;

		activeLayers.Clear();
		ActiveLayers.AddRange(layers);

		Program.outlinerForm.UpdateUI();
	}

	public void CreateTextureSet(TextureSet s)
	{
		textureSetList.Add(s);

		if (Program.tilePicker != null)
			Program.tilePicker.UpdateUI();
	}

	public void RemoveTextureSet(TextureSet s)
	{
		textureSetList.Remove(s);
		s.Texture.Dispose();
		s.Bitmap.Dispose();

		if (Program.tilePicker != null)
			Program.tilePicker.UpdateUI();
	}

	public void CreateMesh(System.Drawing.RectangleF rect, MeshDesign design)
	{
		if (activeLayers.Count == 0 || activeLayers.Count > 1) return;

		EMesh mesh = new EMesh(rect, design, activeLayers[0], this);

		activeLayers[0].AddMesh(mesh);
		SetSelected(new EMesh[] { mesh });

		ActiveHistory.Add(new CreateAction(mesh, ActiveHistory));
	}

	public void Update()
	{
		mouse.PlaneDistance = editorCamera.Position.Z;

		cameraControl.Logic();
		editorCamera.Position = CameraControl.Position;
		editorCamera.Target = CameraControl.Position * new Vector3(1, 1, 0);

		if (keyboard.KeyPressed(Key.Tab))
			SwitchSelectMode();

		if (keyboard.KeyReleased(Key.Delete))
		{
			List<EMesh> meshes = new List<EMesh>();
			foreach (EMesh m in SelectedMeshes)
				meshes.Add(m);

			if (meshes.Count > 0)
			{
				ActiveHistory.Add(new RemoveAction(meshes, ActiveHistory));

				foreach (EMesh m in meshes)
					m.Remove();

				DeselectAll();
			}
		}

		/*
		if (manipulatorMode == ManipulatorMode.Paint)
		{
			form.CursorVisible = false;
		}
		else
			form.CursorVisible = true;
			*/

		foreach (Manipulator m in manipulators)
			m.UpdatePivot();

		//SOME HOTKEYS
		if (form.Focused && !Manipulator.Active)
		{
			if (keyboard[Key.LControl])
			{
				//OPTIONS
				if (keyboard[Key.LShift])
				{
					if (keyboard.KeyPressed(Key.G)) OptionsForm.options.ShowGrid = !OptionsForm.options.ShowGrid;
					if (keyboard.KeyPressed(Key.L)) OptionsForm.options.FocusLayer = !OptionsForm.options.FocusLayer;
					if (keyboard.KeyPressed(Key.B)) OptionsForm.options.MeshBorders = !OptionsForm.options.MeshBorders;
				}
				else //OTHER EDITOR HOTKEYS
				{
					if (keyboard.KeyPressed(Key.D))
					{
						CopySelected();
						PasteSelected();
					}

					if (keyboard.KeyPressed(Key.C))
						CopySelected();
					if (keyboard.KeyPressed(Key.V))
						PasteSelected();

					if (keyboard.KeyPressed(Key.Z) && ActiveLayers.Count > 0)
						ActiveLayers[0].History.Undo();
					if (keyboard.KeyPressed(Key.Y) && ActiveLayers.Count > 0)
						ActiveLayers[0].History.Redo();

					if (keyboard.KeyPressed(Key.B))
						(manipulators[(int)ManipulatorMode.Paint] as VertexPen).ShowPalette();
				}
			}
			else
			{
				//MANIPULATOR CONTROL
				if (keyboard.KeyPressed(Key.Q)) manipulatorMode = ManipulatorMode.None;
				if (keyboard.KeyPressed(Key.W)) manipulatorMode = ManipulatorMode.Translate;
				if (keyboard.KeyPressed(Key.E)) manipulatorMode = ManipulatorMode.Rotate;
				if (keyboard.KeyPressed(Key.R)) manipulatorMode = ManipulatorMode.Scale;
				if (keyboard.KeyPressed(Key.B)) manipulatorMode = ManipulatorMode.Paint;
			}
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
			gridModel.Color = new Color(0.4f, 0.4f, 0.4f, a);

			GL.LineWidth(1f);

			gridModel.Reset();
			gridModel.Scale(size);
			gridModel.Draw();

			gridModel.Color = new Color(0.8f, 0.8f, 0.8f, a);

			GL.LineWidth(3f);

			gridModel.Scale(10f);
			gridModel.Draw();

			GL.LineWidth(1f);
		}

		GL.Enable(EnableCap.StencilTest);
		GL.Clear(ClearBufferMask.StencilBufferBit);
		GL.Disable(EnableCap.StencilTest);

		EMesh.MeshProgram["view"].SetValue(editorCamera.View);
		EMesh.MeshProgram["projection"].SetValue(editorCamera.Projection);
		foreach (EMesh m in Meshes)
			m.Draw();
		foreach (EMesh m in Meshes)
			m.DrawUI();

		GL.Enable(EnableCap.StencilTest);
		GL.Clear(ClearBufferMask.StencilBufferBit);
		GL.Disable(EnableCap.StencilTest);

		Manipulator.Draw();

		selectionBox.Draw();
		meshCreator.Draw();
	}
}