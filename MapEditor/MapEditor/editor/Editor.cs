using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using TKTools;

namespace MapEditor
{
	public class Editor
	{
		public static ShaderProgram program = new ShaderProgram("shaders/standardShader.glsl");
		public static float screenWidth = 20, screenHeight;
		public static Camera camera = new Camera();

		public static float delta = 0f;

		List<EditorObject> objectList = new List<EditorObject>();

		Stopwatch tickWatch;

		public Editor()
		{
			objectList.Add(new EditorObject(this));
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
			CalculateDelta();

			camera.Logic();
			foreach (EditorObject obj in objectList) obj.Logic();
		}

		public void CalculateDelta()
		{
			if (tickWatch == null) tickWatch = Stopwatch.StartNew();

			tickWatch.Stop();
			delta = tickWatch.ElapsedTicks / (float)Stopwatch.Frequency;
			if (delta > 0.2f) delta = 0;
			tickWatch.Restart();
		}

		public void Draw()
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);
			program["view"].SetValue(camera.ViewMatrix);

			foreach (EditorObject obj in objectList) obj.Draw();
		}
	}
}