using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;
namespace MapEditor
{
	public class SelectionBox : IDisposable
	{
		Editor editor;
		Vector2 origin;

		Mesh mesh;

		public SelectionBox(Vector2 origin, Editor e)
		{
			editor = e;

			this.origin = origin;

			mesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
			mesh.Color = new Color(1, 1, 1, 0.2f);
		}

		public void Dispose()
		{
			mesh.Dispose();
		}

		public List<EditorObject> GetObjects()
		{
			List<EditorObject> returnList = new List<EditorObject>();
			Polygon poly = mesh.Polygon;

			foreach (EditorObject obj in editor.objectList)
			{
				if (obj.GetCollision(poly)) returnList.Add(obj);
			}

			return returnList;
		}

		public void Logic()
		{
			Vector2 mouse = MouseInput.Current.Position;

			mesh.Vertices = new Vector2[] {
				origin,
				new Vector2(origin.X, mouse.Y),
				mouse,
				new Vector2(mouse.X, origin.Y)
			};
		}

		public void Draw()
		{
			mesh.Draw();
		}
	}
}