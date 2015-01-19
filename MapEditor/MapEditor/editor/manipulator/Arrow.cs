using OpenTK;
namespace MapEditor.Manipulators
{
	public class ArrowButton : Button
	{
		public ArrowButton(Editor e)
			: base(e)
		{
			mesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
			mesh.Vertices = new Vector2[] {
				new Vector2(-0.5f, -0.5f),
				new Vector2(0f, 0.5f),
				new Vector2(0.5f, -0.5f)
			};
		}
	}
}