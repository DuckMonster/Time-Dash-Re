﻿using OpenTK;
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
		public List<Vertex> selectedList = new List<Vertex>(50);

		Dictionary<EditMode, Manipulator> manipulators = new Dictionary<EditMode, Manipulator>();

		Stopwatch tickWatch;

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

		public Editor()
		{
			manipulators.Add(EditMode.Select, new SelectManipulator(this));
			manipulators.Add(EditMode.Move, new MoveManipulator(this));
			manipulators.Add(EditMode.Rotate, new RotateManipulator(this));
			manipulators.Add(EditMode.Scale, new ScaleManipulator(this));

			objectList.Add(new EditorObject(this));
			objectList.Add(new EditorObject(this));

			gridMesh = new Mesh(PrimitiveType.Quads);
			gridMesh.Color = new Color(1, 1, 1, 0.2f);

			Polygon poly = new Polygon();

			for (int x = -50; x <= 50; x++)
			{
				poly.AddPoint(new Vector2(1 * x - 0.01f, -50));
				poly.AddPoint(new Vector2(1 * x + 0.01f, -50));
				poly.AddPoint(new Vector2(1 * x + 0.01f, 50));
				poly.AddPoint(new Vector2(1 * x - 0.01f, 50));

				poly.AddPoint(new Vector2(-50, 1 * x - 0.01f));
				poly.AddPoint(new Vector2(-50, 1 * x + 0.01f));
				poly.AddPoint(new Vector2(50, 1 * x + 0.01f));
				poly.AddPoint(new Vector2(50, 1 * x - 0.01f));
			}

			gridMesh.Vertices = poly;
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

		public Vertex GetVertexAt(Vector2 pos)
		{
			foreach (EditorObject obj in objectList)
				foreach (Vertex v in obj.Vertices)
					if (v.Hovered) return v;

			return null;
		}

		public void Logic()
		{
			CalculateDelta();

			UpdateEditMode();

			if (MouseInput.ButtonPressed(MouseButton.Right)) DeselectAll();
			if (MouseInput.ButtonPressed(MouseButton.Left) && !CurrentManipulator.Hovered)
			{
				Vertex v = GetVertexAt(MouseInput.Current.Position);
				EditorObject obj = GetObjectAt(MouseInput.Current.Position);

				if (v != null)
					Select(v);
				else if (obj != null)
					Select(obj.Vertices);
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

			CurrentManipulator.Logic();
			if (selectionBox != null) selectionBox.Logic();

			camera.Logic();
			foreach (EditorObject obj in objectList) obj.Logic();
		}

		public void UpdateEditMode()
		{
			if (KeyboardInput.Current.KeyDown(Key.Q)) SetEditMode(EditMode.Select);
			if (KeyboardInput.Current.KeyDown(Key.W)) SetEditMode(EditMode.Move);
			if (KeyboardInput.Current.KeyDown(Key.E)) SetEditMode(EditMode.Rotate);
			if (KeyboardInput.Current.KeyDown(Key.R)) SetEditMode(EditMode.Scale);
		}

		public void SelectAt(Vector2 pos)
		{
			Vertex v = GetVertexAt(pos);
			Select(v);
		}

		public void Select(params Vertex[] objects)
		{
			Manipulator.snapVertex = null;

			if (!KeyboardInput.Current[Key.LShift]) selectedList.Clear();
			foreach (Vertex v in objects)
			{
				if (v != null && !selectedList.Contains(v))
					selectedList.Add(v);
			}
		}

		public void Deselect(params Vertex[] objects)
		{
			foreach (Vertex v in objects)
				selectedList.Remove(v);
		}

		public void DeselectAll()
		{
			selectedList.Clear();
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

			gridMesh.Draw();

			foreach (EditorObject obj in objectList) obj.Draw();
			if (selectionBox != null) selectionBox.Draw();
			CurrentManipulator.Draw();
		}
	}
}