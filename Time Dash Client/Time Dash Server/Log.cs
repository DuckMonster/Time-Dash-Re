using System;
using System.Collections.Generic;
using System.Threading;

enum LogMode
{
	Debug,
	Message
}

class Log
{
	class LogMessage
	{
		string message;
		ConsoleColor color;

		public LogMessage(string msg)
		{
			message = msg;
			color = ConsoleColor.Gray;
		}
		public LogMessage(string msg, ConsoleColor clr)
		{
			message = msg;
			color = clr;
		}

		public void Print()
		{
			Console.ForegroundColor = color;

			Console.WriteLine(message);

			Console.ForegroundColor = ConsoleColor.Gray;

			Console.WriteLine("---");
		}
	}

	static bool running = false;

	static List<LogMessage> logMessageList = new List<LogMessage>();
	static LogMode mode = LogMode.Message;
	static Thread inputThread;

	public static void Init()
	{
		if (inputThread != null) return;

		running = true;

		ShowMessages();
		inputThread = new Thread(InputThread);
		inputThread.Start();
	}

	public static void ShutDown()
	{
		running = false;

		inputThread.Abort();
		inputThread.Join();
		inputThread = null;
	}

	public static float debugTimer = 0f, debugInterval = 0.05f;
	public static bool CanDebug
	{
		get
		{
			return debugTimer <= 0f && mode == LogMode.Debug;
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

	public static void Write(string text, params object[] args)
	{
		Write(ConsoleColor.Gray, text, args);
	}
	public static void Write(ConsoleColor c, string text, params object[] args)
	{
		logMessageList.Add(new LogMessage(string.Format(text, args), c));
		if (mode == LogMode.Message) ShowMessages();
	}

	public static void Clear()
	{
		logMessageList.Clear();
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

	static void ShowMessages()
	{
		Console.Clear();
		foreach (LogMessage s in logMessageList)
		{
			s.Print();
		}
	}

	static void InputThread()
	{
		while (running)
		{
			if (Console.KeyAvailable)
			{
				ConsoleKey k = Console.ReadKey(true).Key;

				switch (k)
				{
					case ConsoleKey.M:
						mode = LogMode.Message;
						ShowMessages();
						break;

					case ConsoleKey.D:
						mode = LogMode.Debug;
						Console.Clear();
						break;
				}
			}

			Thread.Sleep(50);
		}
	}
}
