using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor
{
	public enum ActionType
	{
		Move
	}

	public class VertexAction
	{
		public Vertex vertex;
		public ActionType type;

		public Vector2 factor;

		public VertexAction(Vertex vertex, ActionType type, Vector2 factor)
		{
			this.vertex = vertex;
			this.type = type;
			this.factor = factor;			
		}

		public void Redo()
		{
			switch (type)
			{
				case ActionType.Move:
					vertex.Move(factor);
					break;
			}
		}

		public void Undo()
		{
			switch (type)
			{
				case ActionType.Move:
					vertex.Move(-factor);
					break;
			}
		}
	}

	public class Action
	{
		Editor editor;
		List<VertexAction> vertexActionList = new List<VertexAction>();

		public Action(Editor editor)
		{
			this.editor = editor;
		}

		public void AddVertexAction(Vertex vertex, ActionType type, Vector2 factor)
		{
			AddVertexAction(new VertexAction(vertex, type, factor));
		}
		public void AddVertexAction(VertexAction va)
		{
			vertexActionList.Add(va);
		}

		public void Redo()
		{
			List<Vertex> vertexList = new List<Vertex>();

			foreach (VertexAction a in vertexActionList)
			{
				a.Redo();
				vertexList.Add(a.vertex);
			}

			editor.Select(vertexList.ToArray());
		}

		public void Undo()
		{
			List<Vertex> vertexList = new List<Vertex>();

			foreach (VertexAction a in vertexActionList)
			{
				a.Undo();
				vertexList.Add(a.vertex);
			}

			editor.Select(vertexList.ToArray());
		}
	}
}
