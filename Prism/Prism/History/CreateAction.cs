using System;
using System.Collections.Generic;

public class CreateAction : HistorySystem.HistoryAction
{
	List<EMesh> meshes = new List<EMesh>();

	public CreateAction(IEnumerable<EMesh> m, HistorySystem s)
		:base(s)
	{
		meshes.AddRange(m);
	}
	public CreateAction(EMesh mesh, HistorySystem s)
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
		Editor.Deselect(meshes);

		foreach (EMesh m in meshes)
			m.Layer = null;
	}

	public override void Redo()
	{
		foreach (EMesh m in meshes)
			m.Layer = Layer;

		Editor.SetSelected(meshes);
	}
}