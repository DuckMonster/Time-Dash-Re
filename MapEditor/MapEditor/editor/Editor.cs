using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;
using System.Diagnostics;
using TKTools;

namespace MapEditor
{
	using Manipulators;
	using System;
	using System.Windows.Forms;

	public enum EditMode
	{
		Select,
		Move,
		Scale,
		Rotate
	}

	public partial class Editor : IDisposable
	{
		public static ShaderProgram program = new ShaderProgram(Container.FindLocalFile("shaders/standardShader.glsl")), uiProgram = new ShaderProgram(Container.FindLocalFile("shaders/standardShader.glsl"));
		public static float screenWidth = 20, screenHeight;
		public static Camera camera;

		public static float delta = 0f;
		Stopwatch tickWatch;

		public string mapName = null;
		public int gameModeID = 0;
		LevelDataForm dataForm;

		public Container container;

		public List<Layer> layerList = new List<Layer>();
		public List<Vertex> selectedList = new List<Vertex>(50);

		LayerCreator layerCreator;

		int selectedLayerIndex = 0;

		Dictionary<EditMode, Manipulator> manipulators = new Dictionary<EditMode, Manipulator>();

		public EditMode editMode = EditMode.Move;

		Background background;

		SelectionBox selectionBox;
		public Mesh gridMesh;
		public Mesh originMesh;
		bool showGrid = true;

		public bool preview = false;
		public bool hideVertices = false;
		public bool hideSolids = false;

		public Manipulator CurrentManipulator
		{
			get
			{
				switch (editMode)
				{
					case EditMode.Move:
						return manipulators[EditMode.Move];

					case EditMode.Rotate:
						return manipulators[EditMode.Rotate];

					case EditMode.Scale:
						return manipulators[EditMode.Scale];

					default:
						return manipulators[EditMode.Select];
				}
			}
		}

		public bool Paused
		{
			get
			{
				return !CurrentManipulator.Active && (KeyboardInput.Current[Key.LAlt] || templateMenu.Hovered || templateCreator.Active || layerCreator.Active);
			}
		}

		public Layer ActiveLayer
		{
			get
			{
				return layerList[selectedLayerIndex];
			}
		}

		public List<EditorObject> ActiveObjects
		{
			get
			{
				return ActiveLayer.Objects;
			}
		}

		public Editor(Container c)
		{
			container = c;

			layerList.Add(new SolidLayer(this));
			layerList.Add(new Layer(1, 0, this));

			Init();
		}

		public Editor(string filename, Container c)
		{
			container = c;
			Init();

			LoadMap(filename);
		}

		public void Init()
		{
			camera = new Camera(this);
			background = new Background();

			templateMenu = new TemplateMenu(this);
			tilesetList = new TilesetList(this);

			manipulators.Add(EditMode.Select, new SelectManipulator(this));
			manipulators.Add(EditMode.Move, new MoveManipulator(this));
			manipulators.Add(EditMode.Rotate, new RotateManipulator(this));
			manipulators.Add(EditMode.Scale, new ScaleManipulator(this));

			gridMesh = new Mesh(PrimitiveType.Quads);
			gridMesh.Color = new Color(1, 1, 1, 0.2f);

			List<Vector2> gridVectorList = new List<Vector2>();

			for (int x = -50; x <= 50; x++)
			{
				gridVectorList.Add(new Vector2(1 * x - 0.01f, -50));
				gridVectorList.Add(new Vector2(1 * x + 0.01f, -50));
				gridVectorList.Add(new Vector2(1 * x + 0.01f, 50));
				gridVectorList.Add(new Vector2(1 * x - 0.01f, 50));

				gridVectorList.Add(new Vector2(-50, 1 * x - 0.01f));
				gridVectorList.Add(new Vector2(-50, 1 * x + 0.01f));
				gridVectorList.Add(new Vector2(50, 1 * x + 0.01f));
				gridVectorList.Add(new Vector2(50, 1 * x - 0.01f));
			}

			gridMesh.Vertices = gridVectorList.ToArray();

			originMesh = new Mesh(PrimitiveType.Quads);
			originMesh.Color = new Color(1, 1, 1, 0.4f);

			gridVectorList.Clear();

			gridVectorList.Add(new Vector2(- 0.02f, -50));
			gridVectorList.Add(new Vector2(+ 0.02f, -50));
			gridVectorList.Add(new Vector2(+ 0.02f, 50));
			gridVectorList.Add(new Vector2(- 0.02f, 50));

			gridVectorList.Add(new Vector2(-50, - 0.02f));
			gridVectorList.Add(new Vector2(-50, + 0.02f));
			gridVectorList.Add(new Vector2(50, + 0.02f));
			gridVectorList.Add(new Vector2(50, - 0.02f));

			originMesh.Vertices = gridVectorList.ToArray();

			templateCreator = new TemplateCreator(this);
			layerCreator = new LayerCreator(this);
		}

		public void Logic()
		{
			CalculateDelta();
			UpdateEditMode();

			tilesetList.Logic();
			templateCreator.Logic();
			templateMenu.Logic();
			layerCreator.Logic();

			camera.Logic();

			CurrentManipulator.Logic();

			if (!Paused)
			{
				{
					float mousex = MouseInput.Current.Position.X;
					float mousey = MouseInput.Current.Position.Y;

					mousex = (float)Math.Round(mousex);
					mousey = (float)Math.Round(mousey);

					Console.Clear();
					Console.WriteLine(mousex + ", " + mousey);
				}

				if (KeyboardInput.Current[Key.LControl])
				{
					if (KeyboardInput.KeyPressed(Key.Z))
						Undo();

					if (KeyboardInput.KeyPressed(Key.Y))
						Redo();

					if (KeyboardInput.KeyPressed(Key.D))
						DuplicateSelected();

					if (KeyboardInput.KeyPressed(Key.B))
						background.LoadTexture();

					if (KeyboardInput.KeyPressed(Key.Delete) && MessageBox.Show("Are you sure you want to delete this layer?", "Please confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
						DeleteLayer(ActiveLayer);
				}
				else
				{
					if (KeyboardInput.KeyPressed(Key.G))
						showGrid = !showGrid;

					if (KeyboardInput.KeyPressed(Key.B))
						background.show = !background.show;

					if (KeyboardInput.KeyPressed(Key.L))
						preview = !preview;

					if (KeyboardInput.KeyPressed(Key.H))
						hideVertices = !hideVertices;

					if (KeyboardInput.KeyPressed(Key.Z))
						hideSolids = !hideSolids;

					if (KeyboardInput.KeyPressed(Key.F1))
					{
						if (dataForm == null)
						{
							dataForm = new LevelDataForm(this);
							dataForm.Show();
						}
						else
						{
							dataForm.Dispose();
							dataForm = null;
						}
					}

					if (KeyboardInput.KeyPressed(Key.F2))
					{
						List<EventObject> events = new List<EventObject>();

						foreach(EditorObject o in ActiveObjects)
						{
							if ((o is EventObject) && o.Selected)
								events.Add(o as EventObject);
						}

						if (events.Count > 0)
							new EventForm(events.ToArray()).Show();
					}

					if (KeyboardInput.KeyPressed(Key.Delete))
						DeleteSelected();
				}

				if (KeyboardInput.KeyPressed(Key.Minus))
					MoveSelected(1);
				if (KeyboardInput.KeyPressed(Key.Slash))
					MoveSelected(-1);

				if (MouseInput.ButtonPressed(MouseButton.Right)) DeselectAll();
				if (MouseInput.ButtonPressed(MouseButton.Left) && !CurrentManipulator.Hovered && !templateMenu.Hovered)
				{
					Vertex v = GetVertexAt(MouseInput.Current.Position);
					EditorObject obj = GetObjectAt(MouseInput.Current.Position);
					Layer layer = GetHoveredLayer();

					if (layer != null)
						SetActiveLayer(layer);
					if (v != null)
						Select(v);
					else if (obj != null)
						obj.Select();
					else
						selectionBox = new SelectionBox(MouseInput.Current.Position, this);
				}

				if (MouseInput.ButtonReleased(MouseButton.Left) && selectionBox != null)
				{
					if (KeyboardInput.Current[Key.LControl])
						Deselect(selectionBox.GetObjects().ToArray());
					else
						Select(selectionBox.GetObjects().ToArray());

					selectionBox.Dispose();
					selectionBox = null;
				}

				if (selectionBox != null) selectionBox.Logic();

				ActiveLayer.Logic();
			}
		}

		public void UpdateProjection(Vector2 size)
		{
			float ratio = size.Y / size.X;
			screenHeight = screenWidth * ratio;

			//Matrix4 proj = Matrix4.CreatePerspectiveOffCenter(-screenWidth / 2, screenWidth / 2, -screenHeight / 2, screenHeight / 2, 1f, 1000);
			Matrix4 proj = Matrix4.CreatePerspectiveOffCenter(-1, 1, -ratio, ratio, 1f, 1000);
			program["projection"].SetValue(proj);
			uiProgram["projection"].SetValue(Matrix4.CreateOrthographic(screenWidth, screenHeight, 1f, 1000));
			uiProgram["view"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 2), Vector3.Zero, Vector3.UnitY));
		}

		public void UpdateEditMode()
		{
			if (CurrentManipulator.Active) return;

			if (KeyboardInput.Current.KeyDown(Key.Q)) SetEditMode(EditMode.Select);
			if (KeyboardInput.Current.KeyDown(Key.W)) SetEditMode(EditMode.Move);
			if (KeyboardInput.Current.KeyDown(Key.E)) SetEditMode(EditMode.Rotate);
			if (KeyboardInput.Current.KeyDown(Key.R)) SetEditMode(EditMode.Scale);
		}

		public void SetEditMode(EditMode em)
		{
			editMode = em;
		}

		public Layer GetHoveredLayer()
		{
			Layer closestLayer = null;

			foreach (Layer l in layerList)
			{
				if (l.ButtonHovered && (closestLayer == null || l.Z < closestLayer.Z))
					closestLayer = l;
			}

			return closestLayer;
		}

		public void CreateLayer(float depth)
		{
			Layer l = new Layer(layerList.Count, depth, this);
			layerList.Add(l);
			SetActiveLayer(l);
		}

		public void SetActiveLayer(Layer l)
		{
			if (l == ActiveLayer) return;

			selectedLayerIndex = l.ID;
			DeselectAll();

			gridMesh.Reset();
			gridMesh.Translate(0, 0, -l.Z);

			originMesh.Reset();
			originMesh.Translate(0, 0, -l.Z);
		}

		public void DeleteLayer(Layer l)
		{
			if (l.ID == 0) return;
			if (l == ActiveLayer) SetActiveLayer(layerList[0]);

			l.Dispose();

			for (int i = l.ID; i < layerList.Count; i++)
				layerList[i].ID--;

			layerList.Remove(l);
		}

		public void CalculateDelta()
		{
			if (tickWatch == null) tickWatch = Stopwatch.StartNew();

			tickWatch.Stop();
			delta = tickWatch.ElapsedTicks / (float)Stopwatch.Frequency;
			if (delta > 0.2f) delta = 0f;
			tickWatch.Restart();
		}

		Stopwatch frameWatch;
		float displayFrameFreq = 0.2f;
		float displayFrameTime = 0f;

		public void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.DepthFunc(DepthFunction.Lequal);
			program["view"].SetValue(camera.ViewMatrix);

			background.Draw();

			foreach (Layer l in layerList) if (l != ActiveLayer || preview) l.Draw();
			if (!preview) ActiveLayer.Draw();

			if (selectionBox != null) selectionBox.Draw();

			if (showGrid && !layerCreator.Active)
			{
				gridMesh.Draw();
				originMesh.Draw();
			}

			templateMenu.Draw();
			CurrentManipulator.Draw();
			templateCreator.Draw();

			layerCreator.Draw();
		}
	}
}