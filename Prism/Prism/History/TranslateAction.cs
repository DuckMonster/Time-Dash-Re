using System;
using OpenTK;
using System.Collections.Generic;

public class TranslateAction : HistorySystem.HistoryAction
{
	struct TranslateInfo
	{
		EVertex vertex;
		Vector2 begin, end;

		public EVertex Vertex { get { return vertex; } }
		public Vector2 Begin { get { return begin; } }
		public Vector2 End { get { return end; } }

		public TranslateInfo(EVertex v, Vector2 b, Vector2 e)
		{
			vertex = v;
			begin = b;
			end = e;
		}
	}

	TranslateInfo[] translates;

	public TranslateAction(IList<EVertex> vertices, IList<Vector2> begin, IList<Vector2> end, HistorySystem s)
		: base(s)
	{
		translates = new TranslateInfo[vertices.Count];

		for (int i = 0; i < vertices.Count; i++)
			translates[i] = new TranslateInfo(vertices[i], begin[i], end[i]);
	}

	public override void Redo()
	{
		Editor.DeselectAll();

		foreach (TranslateInfo i in translates)
		{
			i.Vertex.Position = i.End;
			i.Vertex.Selected = true;
		}
    }

	public override void Undo()
	{
		Editor.DeselectAll();

		foreach (TranslateInfo i in translates)
		{
			i.Vertex.Position = i.Begin;
			i.Vertex.Selected = true;
		}
	}
}