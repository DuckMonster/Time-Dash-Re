using System;
using System.Collections.Generic;
using OpenTK;

using MapEditor.Manipulators;
using OpenTK.Input;

namespace MapEditor
{
	public partial class Editor
	{
		public List<Action> actionList = new List<Action>(100);
		int actionIndex = -1;

		public EditorObject GetObjectAt(Vector2 pos)
		{
			for (int i = ActiveObjects.Count - 1; i >= 0; i--)
				if (ActiveObjects[i].Hovered) return ActiveObjects[i];
			return null;
		}

		public Vertex GetVertexAt(Vector2 pos)
		{
			foreach (EditorObject obj in ActiveObjects)
				foreach (Vertex v in obj.Vertices)
					if (v.Hovered) return v;

			return null;
		}

		public void AddAction(Action a)
		{
			saveFlag = true;

			if (actionList.Count > actionIndex + 1)
				actionList.RemoveRange(actionIndex + 1, actionList.Count - (actionIndex + 1));

			actionIndex++;
			actionList.Add(a);
		}

		public void Undo()
		{
			if (actionIndex < 0) return;

			actionList[actionIndex].Undo();
			actionIndex--;
		}

		public void Redo()
		{
			if (actionIndex >= actionList.Count - 1) return;

			actionIndex++;
			actionList[actionIndex].Redo();
		}


		public void DeleteSelected()
		{
			List<EditorObject> deletedObjects = new List<EditorObject>();

			foreach (EditorObject obj in ActiveObjects)
			{
				if (obj.Selected) deletedObjects.Add(obj);
			}

			foreach (EditorObject obj in deletedObjects)
				DeleteObject(obj);
		}

		public void DeleteObject(EditorObject obj)
		{
			obj.layer.Objects.Remove(obj);
			Deselect(obj.Vertices);

			obj.template.references.Remove(obj);
			obj.Dispose();
		}

		public void CreateObject(EditorObject obj)
		{
			ActiveLayer.CreateObject(obj);
		}

		public void DuplicateSelected()
		{
			ActiveLayer.DuplicateSelected();
		}

		public void SelectAt(Vector2 pos)
		{
			Vertex v = GetVertexAt(pos);
			Select(v);
		}

		public void Select(params Vertex[] objects)
		{
			if (!KeyboardInput.Current[Key.LShift]) selectedList.Clear();
			SelectAdd(objects);
		}

		public void SelectAdd(params Vertex[] objects)
		{
			Manipulator.snapVertex = null;

			foreach (Vertex v in objects)
			{
				if (v != null && !selectedList.Contains(v))
					selectedList.Add(v);
			}
		}

		public void Deselect(params Vertex[] objects)
		{
			Manipulator.snapVertex = null;

			foreach (Vertex v in objects)
				selectedList.Remove(v);
		}

		public void DeselectAll()
		{
			Manipulator.snapVertex = null;

			selectedList.Clear();
		}
	}
}