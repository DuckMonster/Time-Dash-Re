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

	public enum EditMode
	{
		Select,
		Move,
		Scale,
		Rotate
	}

	public partial class Editor : IDisposable
	{
		public static ShaderProgram program = new ShaderProgram("shaders/standardShader.glsl"), uiProgram = new ShaderProgram("shaders/standardShader.glsl");
		public static float screenWidth = 20, screenHeight;
		public static Camera camera = new Camera();

		public static float delta = 0f;
		Stopwatch tickWatch;

		public Container container;

		public List<EditorObject> objectList = new List<EditorObject>();
		public List<Vertex> selectedList = new List<Vertex>(50);

		Dictionary<EditMode, Manipulator> manipulators = new Dictionary<EditMode, Manipulator>();

		public EditMode editMode = EditMode.Move;

		SelectionBox selectionBox;
		Mesh gridMesh;

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
				return !CurrentManipulator.Active && (KeyboardInput.Current[Key.LAlt] || templateMenu.Hovered || templateCreator.Active);
			}
		}

		public Editor(Container c)
		{
			container = c;
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

			templateCreator = new TemplateCreator(this);
		}

		public void Logic()
		{
			CalculateDelta();
			UpdateEditMode();

			tilesetList.Logic();
			templateCreator.Logic();
			templateMenu.Logic();
			camera.Logic();

			CurrentManipulator.Logic();

			if (KeyboardInput.Current[Key.LControl])
			{
				if (KeyboardInput.KeyPressed(Key.Z))
					Undo();

				if (KeyboardInput.KeyPressed(Key.Y))
					Redo();

				if (KeyboardInput.KeyPressed(Key.D))
					DuplicateSelected();
			}

			if (!Paused)
			{
				if (KeyboardInput.KeyPressed(Key.Delete))
					DeleteSelected();

				if (MouseInput.ButtonPressed(MouseButton.Right)) DeselectAll();
				if (MouseInput.ButtonPressed(MouseButton.Left) && !KeyboardInput.Current[Key.AltLeft] && !CurrentManipulator.Hovered && !templateMenu.Hovered)
				{
					Vertex v = GetVertexAt(MouseInput.Current.Position);
					EditorObject obj = GetObjectAt(MouseInput.Current.Position);

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

				foreach (EditorObject obj in objectList) obj.Logic();
			}
		}

		public void UpdateProjection(Vector2 size)
		{
			float ratio = size.Y / size.X;
			screenHeight = screenWidth * ratio;

			Matrix4 proj = Matrix4.CreatePerspectiveOffCenter(-screenWidth / 2, screenWidth / 2, -screenHeight / 2, screenHeight / 2, 1f, 1000);
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

		public void CalculateDelta()
		{
			if (tickWatch == null) tickWatch = Stopwatch.StartNew();

			tickWatch.Stop();
			delta = tickWatch.ElapsedTicks / (float)Stopwatch.Frequency;
			if (delta > 0.2f) delta = 0;
			tickWatch.Restart();
		}

		public void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			program["view"].SetValue(camera.ViewMatrix);

			foreach (EditorObject obj in objectList) obj.Draw();
			if (selectionBox != null) selectionBox.Draw();
			CurrentManipulator.Draw();

			templateMenu.Draw();

			gridMesh.Draw();

			templateCreator.Draw();
		}
	}
}