using System;
using OpenTK;
using System.Collections.Generic;
using TKTools;

public class PaintAction : HistorySystem.HistoryAction
{
	struct PaintInfo
	{
		EVertex vertex;
		ColorHSL begin, end;

		public EVertex Vertex { get { return vertex; } }
		public ColorHSL Begin { get { return begin; } }
		public ColorHSL End { get { return end; } }

		public PaintInfo(EVertex v, ColorHSL b, ColorHSL e)
		{
			vertex = v;
			begin = b;
			end = e;
		}
	}

	PaintInfo[] paints;

	public PaintAction(IList<EVertex> vertices, IList<ColorHSL> begin, IList<ColorHSL> end, HistorySystem s)
		: base(s)
	{
		paints = new PaintInfo[vertices.Count];

		for (int i = 0; i < vertices.Count; i++)
			paints[i] = new PaintInfo(vertices[i], begin[i], end[i]);
	}

	public override void Redo()
	{
		Editor.DeselectAll();

		foreach (PaintInfo i in paints)
		{
			i.Vertex.HSL = i.End;
			i.Vertex.Selected = true;
		}
	}

	public override void Undo()
	{
		Editor.DeselectAll();

		foreach (PaintInfo i in paints)
		{
			i.Vertex.HSL = i.Begin;
			i.Vertex.Selected = true;
		}
	}
}