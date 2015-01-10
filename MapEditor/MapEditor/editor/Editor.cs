using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using TKTools;

namespace MapEditor
{
	public class Editor
	{
		public static ShaderProgram program = new ShaderProgram("shaders/standardShader.glsl");
		public static float screenWidth = 20, screenHeight;

		Mesh m = Mesh.Box;
		Camera camera = new Camera();

		public Editor()
		{
		}

		public void UpdateProjection(Vector2 size)
		{
			float ratio = size.Y / size.X;
			screenHeight = screenWidth * ratio;

			Matrix4 proj = Matrix4.CreatePerspectiveOffCenter(-screenWidth / 2, screenWidth / 2, -screenHeight / 2, screenHeight / 2, 1, 1000);
			program["projection"].SetValue(proj);
		}

		public void Logic()
		{
			camera.Logic();
		}

		public void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			program["view"].SetValue(camera.ViewMatrix);

			for (int x = -1; x <= 1; x++)
				for (int y = -1; y <= 1; y++)
				{
					m.Reset();
					m.Translate(x * 20f, y * 20f);
					m.Scale(10f);
					m.Draw();
				}
		}
	}
}