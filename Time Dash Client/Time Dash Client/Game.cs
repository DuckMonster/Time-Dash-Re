using System;
using System.Diagnostics;
using OpenGL;

class Game
{
	public static float delta;
	public static ShaderProgram defaultShader;

	Stopwatch watch;

	Camera currentCamera;

	public Game()
	{
		defaultShader = Program.CreateShaderProgram("shaders/vertexShader.cpp", "shaders/fragmentShader.cpp");
		currentCamera = new Camera();
	}

	public void Logic()
	{
		CalculateDelta();
		Log.Logic();

		currentCamera.Logic();
	}

	public void CalculateDelta()
	{
		if (watch == null)
		{
			watch = Stopwatch.StartNew();
		}

		watch.Stop();
		delta = watch.ElapsedTicks / (float)Stopwatch.Frequency;
		watch.Restart();
	}

	public void Draw()
	{
		Mesh.CALCULATIONS = 0;
		Mesh.DRAW_CALLS = 0;

		float ratio = 768f / 1024f;

		defaultShader["projection_matrix"].SetValue(Matrix4.CreatePerspectiveOffCenter(-1f, 1f, -ratio, ratio, 1, 1000));
		defaultShader["view_matrix"].SetValue(currentCamera.GetViewMatrix());

		using (Mesh m = Mesh.Box)
		{
			m.Draw();
		}

		Log.Debug("Calculations: {0}\nDraw calls: {1}", Mesh.CALCULATIONS, Mesh.DRAW_CALLS);
	}
}