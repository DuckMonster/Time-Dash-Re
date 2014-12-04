using System;

class Log
{
	public static float debugTimer = 0f, debugInterval = 0.05f;
	public static bool CanDebug
	{
		get
		{
			return debugTimer <= 0f;
		}
	}

	public static void Debug(string text, params object[] args)
	{
		if (CanDebug)
		{
			Console.WriteLine(text, args);
			Console.WriteLine("--");
		}
	}

	private static int[] tickAvg = new int[150], frameAvg = new int[500];
	private static int tickAvgIndex = 0, frameAvgIndex = 0;

	public static void Logic()
	{
		if (CanDebug) debugTimer = debugInterval;
		debugTimer -= Game.delta;

		ShowFPS();
	}

	public static void CalculateTick(float t)
	{
		tickAvg[tickAvgIndex] = (int)(1.0 / t);
		tickAvgIndex = (tickAvgIndex + 1) % tickAvg.Length;
	}

	public static void CalculateFrame(float t)
	{
		frameAvg[frameAvgIndex] = (int)(1.0 / t);
		frameAvgIndex = (frameAvgIndex + 1) % frameAvg.Length;
	}

	static void ShowFPS()
	{
		if (CanDebug)
		{
			Console.Clear();

			//Calculate average
			int tick = 0, frame = 0;

			for (int i = 0; i < tickAvg.Length; i++)
				tick += tickAvg[i];

			for (int i = 0; i < frameAvg.Length; i++)
				frame += frameAvg[i];

			tick /= tickAvg.Length;
			frame /= frameAvg.Length;

			//

			Debug("Ticks/S: {1} ~ {0:0}\nFrames/S: {3} ~ {2:0}", tickAvg[tickAvgIndex], tick, frameAvg[frameAvgIndex], frame);
		}
	}
}
