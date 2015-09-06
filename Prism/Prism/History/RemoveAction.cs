using System;
using System.Collections.Generic;

public class RemoveAction : HistorySystem.HistoryAction
{
	List<EMesh> meshes = new List<EMesh>();

	public RemoveAction(IEnumerable<EMesh> m, HistorySystem s)
		: base(s)
	{
		meshes.AddRange(m);
	}
	public RemoveAction(EMesh mesh, HistorySystem s)
		: this(new EMesh[] { mesh }, s)
	{
	}

	public override void Dispose()
	{
		foreach (EMesh m in meshes)
			m.Dispose();
	}

	public override void Undo()
	{
		foreach (EMesh m in meshes)
			m.Layer = Layer;

		Editor.SetSelected(meshes);
	}

	public override void Redo()
	{
		Editor.Deselect(meshes);

		foreach (EMesh m in meshes)
			m.Layer = null;
	}
}