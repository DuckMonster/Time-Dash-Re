using System;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

public class Game
{
	public static float delta;

	Map map;
	Stopwatch tickWatch, frameWatch;

	public Game()
	{
		map = new Map();
	}

	public void UpdateProjection(Matrix4 proj)
	{
		Map.defaultProgram["projection"].SetValue(proj);
	}

	public void Logic()
	{
		CalculateDelta();
		Log.Logic();
		Log.Debug("Calculations: {0}\nDraw Calls: {1}", Mesh.CALCULATIONS, Mesh.DRAW_CALLS);

		map.Logic();
	}

	public void CalculateDelta()
	{
		if (tickWatch == null) tickWatch = Stopwatch.StartNew();

		tickWatch.Stop();
		delta = tickWatch.ElapsedTicks / (float)Stopwatch.Frequency;
		tickWatch.Restart();

		Log.CalculateTick(delta);
	}

	public void CalculateFrameDelta()
	{
		if (frameWatch == null) frameWatch = Stopwatch.StartNew();

		frameWatch.Stop();
		float d = frameWatch.ElapsedTicks / (float)Stopwatch.Frequency;
		frameWatch.Restart();

		Log.CalculateFrame(d);
	}

	public void Draw()
	{
		CalculateFrameDelta();
		Mesh.DRAW_CALLS = Mesh.CALCULATIONS = 0;

		map.Draw();
	}
}