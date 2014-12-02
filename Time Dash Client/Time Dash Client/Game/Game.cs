using System;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

public class Game
{
	public static float delta;
	public static ShaderProgram defaultShader;

	Stopwatch watch;
	Mesh mesh;

	public Game()
	{
		defaultShader = new ShaderProgram("Shaders/standardShader.glsl");
		defaultShader["view"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.UnitY));

		mesh = Mesh.Box;
		mesh.Texture = new Texture("Res/circlebig.png");
	}

	public void UpdateProjection(Matrix4 proj)
	{
		defaultShader["projection"].SetValue(proj);
	}

	public void Logic()
	{
		CalculateDelta();
		Log.Logic();
		Log.Debug("Calculations: {0}\nDraw Calls: {1}", Mesh.CALCULATIONS, Mesh.DRAW_CALLS);
	}

	public void CalculateDelta()
	{
		if (watch == null) watch = Stopwatch.StartNew();

		watch.Stop();
		delta = watch.ElapsedTicks / (float)Stopwatch.Frequency;
		watch.Restart();
	}

	public void Draw()
	{
		Mesh.DRAW_CALLS = Mesh.CALCULATIONS = 0;

		Mesh m = Mesh.Box;
		m.Texture = new Texture("Res/circlebig.png");

		m.Draw();

		m.Texture.Dispose();
		m.Dispose();
	}
}