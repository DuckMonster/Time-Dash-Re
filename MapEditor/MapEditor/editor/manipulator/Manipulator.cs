using System.Collections.Generic;
using DRAW = System.Drawing;
using TKTools;
using OpenTK;
using System;

namespace MapEditor.Manipulators
{
	public class Manipulator : IDisposable
	{
		protected Editor editor;
		protected Mesh mesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);

		public bool Hovered
		{
			get
			{
				if (!Enabled) return false;
				return mesh.Polygon.Intersects(new Polygon(MouseInput.Current.Position));
			}
		}

		public bool Enabled
		{
			get
			{
				return editor.selectedList.Count > 0;
			}
		}

		public Manipulator(Editor editor)
		{
			this.editor = editor;
		}

		public virtual void Dispose()
		{
			mesh.Dispose();
		}

		public virtual void Logic()
		{
			if (!Enabled) return;

			Polygon combined = new Polygon();

			foreach (EditorObject obj in editor.selectedList)
				combined.AddPoint(obj.mesh.Polygon);

			DRAW.RectangleF bounds = combined.Bounds;

			mesh.Vertices = new Vector2[] {
				new Vector2(bounds.X, bounds.Y),
				new Vector2(bounds.X + bounds.Width, bounds.Y),
				new Vector2(bounds.X + bounds.Width, bounds.Y + bounds.Height),
				new Vector2(bounds.X, bounds.Y + bounds.Height)
			};
		}

		public virtual void Draw()
		{
			if (!Enabled) return;

			mesh.Color = new Color(1, 1, 1, Hovered ? 0.2f : 0.05f);
			mesh.Draw();
		}
	}
}