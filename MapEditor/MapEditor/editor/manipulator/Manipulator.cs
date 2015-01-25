using System.Collections.Generic;
using DRAW = System.Drawing;
using TKTools;
using OpenTK;
using OpenTK.Input;
using System.Drawing;
using System;

namespace MapEditor.Manipulators
{
	public class Manipulator : System.IDisposable
	{
		public static Vertex snapVertex;
		protected Editor editor;

		protected Dictionary<Vertex, Vector2> vertexOffsetList = new Dictionary<Vertex, Vector2>();

		bool useEgdeNormal = true;
		bool active = false;

		Vector2 startOrigin;

		public virtual bool Hovered
		{
			get
			{
				return false;
			}
		}

		public bool Enabled
		{
			get
			{
				return editor.selectedList.Count > 0;
			}
		}

		public bool Active
		{
			get
			{
				return active;
			}
		}

		public virtual Vector2 Origin
		{
			get
			{
				if (snapVertex != null) return snapVertex.Position;

				Polygon p = new Polygon();
				foreach (Vertex v in editor.selectedList)
					p.AddPoint(v);

				RectangleF rect = p.Bounds;

				return new Vector2(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			}
		}

		public virtual Vector2 Normal
		{
			get
			{
				if (useEgdeNormal && editor.selectedList.Count >= 2 && (editor.selectedList[0].Position - editor.selectedList[1].Position != Vector2.Zero))
					return Vertex.GetNormal(editor.selectedList[0], editor.selectedList[1]);
				else
					return new Vector2(0, 1);
			}
		}

		public Manipulator(Editor editor)
		{
			this.editor = editor;
		}

		public virtual void Dispose()
		{
		}

		public virtual void Enable(Vector2 pos)
		{
			vertexOffsetList.Clear();

			startOrigin = Origin;

			foreach (Vertex v in editor.selectedList)
				vertexOffsetList.Add(v, v.Position - startOrigin);

			active = true;
		}

		public virtual void Disable()
		{
			active = false;

			Action a = new Action(editor);

			foreach (Vertex v in editor.selectedList)
			{
				Vector2 delta = (v.Position - startOrigin) - vertexOffsetList[v];
				a.AddVertexAction(v, ActionType.Move, delta);
			}

			editor.AddAction(a);
		}

		public virtual void Logic()
		{
			if (!Enabled) return;

			if (!editor.Paused)
			{
				if (!KeyboardInput.Current[Key.LControl])
				{
					if (KeyboardInput.Current[Key.N] && !KeyboardInput.Previous[Key.N]) useEgdeNormal = !useEgdeNormal;
					if (KeyboardInput.Current[Key.S]) snapVertex = null;
					if (KeyboardInput.Current[Key.D])
					{
						Vertex closest = null;
						float closestDistance = 0;

						foreach (Vertex v in editor.selectedList)
						{
							if (closest == null)
							{
								closest = v;
								closestDistance = (v.Position - MouseInput.Current.Position).Length;
							}

							float vDistance = (v.Position - MouseInput.Current.Position).Length;
							if (vDistance < closestDistance)
							{
								closest = v;
								closestDistance = vDistance;
							}
						}

						snapVertex = closest;
					}
				}

				if (MouseInput.ButtonPressed(MouseButton.Left) && Hovered) Enable(MouseInput.Current.Position);
			}

			if (Active && !MouseInput.Current[MouseButton.Left]) Disable();

			if (Active)
				Manipulate();
		}

		public virtual void Manipulate()
		{
		}

		public virtual void Draw()
		{
			if (!Enabled) return;
		}
	}
}