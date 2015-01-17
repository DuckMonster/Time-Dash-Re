using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections.Generic;
using System.Diagnostics;
using TKTools;

namespace MapEditor
{
	using Manipulators;

	public enum EditMode
	{
		Select,
		Move,
		Scale,
		Rotate
	}

	public class Editor
	{
		public static ShaderProgram program = new ShaderProgram("shaders/standardShader.glsl");
		public static float screenWidth = 20, screenHeight;
		public static Camera camera = new Camera();

		public static float delta = 0f;

		public List<EditorObject> objectList = new List<EditorObject>();
		public List<EditorObject> selectedList = new List<EditorObject>(50);

		Stopwatch tickWatch;

		public EditMode editMode = EditMode.Move;
		public bool manipulating = false;

		SelectionBox selectionBox;
		Manipulator manipulator;

		public Editor()
		{
			objectList.Add(new EditorObject(this));
			objectList.Add(new EditorObject(this));

			manipulator = new MoveManipulator(this);
		}

		public void UpdateProjection(Vector2 size)
		{
			float ratio = size.Y / size.X;
			screenHeight = screenWidth * ratio;

			Matrix4 proj = Matrix4.CreatePerspectiveOffCenter(-screenWidth / 2, screenWidth / 2, -screenHeight / 2, screenHeight / 2, 1, 1000);
			program["projection"].SetValue(proj);
		}

		public EditorObject GetObjectAt(Vector2 pos)
		{
			foreach (EditorObject obj in objectList) if (obj.Hovered) return obj;
			return null;
		}

		public void Logic()
		{
			CalculateDelta();

			UpdateEditMode();

			if (MouseInput.ButtonPressed(MouseButton.Left) && !manipulator.Hovered)
			{
				EditorObject obj = GetObjectAt(MouseInput.Current.Position);
				Select(obj);

				if (obj == null) selectionBox = new SelectionBox(MouseInput.Current.Position, this);
			}

			if (MouseInput.ButtonReleased(MouseButton.Left) && selectionBox != null)
			{
				Select(selectionBox.GetObjects().ToArray());

				selectionBox.Dispose();
				selectionBox = null;
			}

			manipulator.Logic();
			if (selectionBox != null) selectionBox.Logic();

			camera.Logic();
			foreach (EditorObject obj in objectList) obj.Logic();
		}

		public void UpdateEditMode()
		{
			if (KeyboardInput.Current.KeyDown(Key.Q)) SetEditMode(EditMode.Select);
			if (KeyboardInput.Current.KeyDown(Key.W)) SetEditMode(EditMode.Move);
			if (KeyboardInput.Current.KeyDown(Key.E)) SetEditMode(EditMode.Scale);
			if (KeyboardInput.Current.KeyDown(Key.R)) SetEditMode(EditMode.Rotate);
		}

		public void SelectAt(Vector2 pos)
		{
			EditorObject obj = GetObjectAt(pos);
			Select(obj);
		}

		public void Select(params EditorObject[] objects)
		{
			if (!KeyboardInput.Current[Key.LShift]) selectedList.Clear();
			foreach (EditorObject obj in objects)
			{
				if (obj != null && !selectedList.Contains(obj))
					selectedList.Add(obj);
			}
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
			manipulator.Draw();
		}
	}
}