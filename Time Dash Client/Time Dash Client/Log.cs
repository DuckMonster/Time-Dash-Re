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

	private static int[] fpsAvg = new int[20];
	private static int fpsAvgIndex = 0;

	public static void Logic()
	{
		if (CanDebug) debugTimer = debugInterval;
		debugTimer -= Game.delta;

		ShowFPS();
	}

	static void ShowFPS()
	{
		if (CanDebug)
		{
			Console.Clear();

			fpsAvg[fpsAvgIndex] = (int)(1.0 / Game.delta);

			//Calculate average
			int avg = 0;

			for (int i = 0; i < fpsAvg.Length; i++)
				avg += fpsAvg[i];

			avg /= fpsAvg.Length;
			//

			Debug("FPS: {0:0} ~ {1}", fpsAvg[fpsAvgIndex], avg);

			fpsAvgIndex = (fpsAvgIndex + 1) % fpsAvg.Length;
		}
	}
}
