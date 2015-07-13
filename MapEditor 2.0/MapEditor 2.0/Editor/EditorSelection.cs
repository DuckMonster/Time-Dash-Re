using OpenTK.Input;
using System;
using System.Collections.Generic;

public partial class Editor
{
	public void OnSelect()
	{
		switch (SelectMode)
		{
			case SelectMode.Mesh:
				if (keyboard[Key.LShift])
					Select(selectionBox.GetSelectedMeshes());
				else if (keyboard[Key.LControl])
					Deselect(selectionBox.GetSelectedMeshes());
				else
					SetSelected(selectionBox.GetSelectedMeshes());

				break;

			case SelectMode.Vertices:
				List<EVertex> v = selectionBox.GetSelectedVertices();

				if (v.Count > 0)
				{
					if (keyboard[Key.LShift])
						Select(v);
					else if (keyboard[Key.LControl])
						Deselect(v);
					else
						SetSelected(v);
				}
				else
				{
					if (keyboard[Key.LShift])
						Select(selectionBox.GetSelectedMeshes());
					else if (keyboard[Key.LControl])
						Deselect(selectionBox.GetSelectedMeshes());
					else
						SetSelected(selectionBox.GetSelectedMeshes());
				}

				break;
		}
	}

	public void DeselectAll()
	{
		foreach (Manipulator m in manipulators)
			m.ResetPivot();

		selectedVertices.Clear();
	}

	public void Select(IList<EVertex> vertices)
	{
		foreach (Manipulator m in manipulators)
			m.ResetPivot();

		foreach (EVertex v in vertices)
			if (!selectedVertices.Contains(v)) selectedVertices.Add(v);
	}

	public void Select(IList<EMesh> meshes)
	{
		List<EVertex> vList = new List<EVertex>();
		foreach (EMesh m in meshes)
			foreach (EVertex v in m)
				vList.Add(v);

		Select(vList);
	}

	public void Deselect(IList<EVertex> vertices)
	{
		foreach (Manipulator m in manipulators)
			m.ResetPivot();

		foreach (EVertex v in vertices)
			selectedVertices.Remove(v);
	}

	public void Deselect(IList<EMesh> meshes)
	{
		foreach (EMesh m in meshes)
			Deselect(m.Vertices);
	}

	public void SetSelected(IList<EVertex> vertices)
	{
		selectedVertices.Clear();
		Select(vertices);
	}

	public void SetSelected(IList<EMesh> meshes)
	{
		selectedVertices.Clear();
		Select(meshes);
	}
}