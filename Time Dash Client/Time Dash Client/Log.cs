﻿using EZUDP.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

enum LogMode
{
	Debug,
	Message,
	Neutral
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
		}
	}

	static bool running = false;

	static List<LogMessage> logMessageList = new List<LogMessage>();
	static LogMode mode = LogMode.Message;
	static Thread inputThread;

	static Stopwatch networkWatch;
	static int currentUp = 0, currentDown = 0, maxUp = 0, maxDown = 0;
	static int ping = 0;
	static void Ping(int millis) { ping = millis; }

	public static void Init()
	{
		if (inputThread != null) return;

		running = true;

		ShowMessages();
		inputThread = new Thread(InputThread);
		inputThread.Start();

		Game.client.OnPing += Ping;
	}

	public static void ShutDown()
	{
		running = false;

		inputThread.Abort();
		inputThread.Join();
		inputThread = null;
	}

	public static float debugTimer = 0f, debugInterval = 0.1f;
	public static bool CanDebug
	{
		get
		{
			return debugTimer <= 0f && mode == LogMode.Debug;
		}
	}

	public static void Debug(object o) { Debug(o.ToString()); }
	public static void Debug(string text, params object[] args)
	{
		if (CanDebug)
		{
			Console.WriteLine(text, args);
			Console.WriteLine("--");
		}
	}

	public static void Write(ConsoleColor c, object o)
	{
		Write(c, o.ToString());
	}
	public static void Write(object o)
	{
		Write(o.ToString());
	}
	public static void Write(string text, params object[] args)
	{
		Write(ConsoleColor.Gray, text, args);
	}
	public static void Write(ConsoleColor c, string text, params object[] args)
	{
		LogMessage msg = new LogMessage(string.Format(text, args), c);

		logMessageList.Add(msg);
		if (mode == LogMode.Message) msg.Print();
	}

	public static void Clear()
	{
		logMessageList.Clear();
	}

	private static int currentFrame = 0;
	private static int[] tickAvg = new int[150], frameAvg = new int[500];
	private static int tickAvgIndex = 0, frameAvgIndex = 0;

	public static void Logic()
	{
		if (CanDebug) debugTimer = debugInterval;
		debugTimer -= Game.delta;

		ShowFPS();

		if (networkWatch == null) networkWatch = Stopwatch.StartNew();
		if (networkWatch.Elapsed.Seconds >= 1)
		{
			CalculateNetworkData();
			networkWatch.Restart();
		}

		ShowNetworkData();
	}

	public static void CalculateTick(float t)
	{
		tickAvg[tickAvgIndex] = (int)(1.0 / t);
		tickAvgIndex = (tickAvgIndex + 1) % tickAvg.Length;

		frameAvg[frameAvgIndex] = currentFrame;
		frameAvgIndex = (frameAvgIndex + 1) % frameAvg.Length;
	}

	public static void CalculateFrame(float t)
	{
		currentFrame = (int)(1.0 / t);
	}

	public static void CalculateNetworkData()
	{
		if (Game.client.Connected)
		{
			currentDown = EzClient.DownBytes;
			currentUp = EzClient.UpBytes;

			if (currentDown > maxDown) maxDown = currentDown;
			if (currentUp > maxUp) maxUp = currentUp;

			Game.client.Ping();
		}
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

	static void ShowNetworkData()
	{
		Debug("U: {0} B/s  \tMax: {4} b/s\nD: {1} B/s  \tMax: {5} b/s\n\nTotal\nU: {2} B\nD: {3} B\n\nPing: {6}",
			currentUp, currentDown, EzClient.UpBytesTotal, EzClient.DownBytesTotal, maxUp, maxDown, ping);
	}

	static void ShowMessages()
	{
		Console.Clear();

		List<LogMessage> buffer = new List<LogMessage>();
		buffer.AddRange(logMessageList.ToArray());

		foreach (LogMessage s in buffer)
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

					case ConsoleKey.N:
						mode = LogMode.Neutral;
						Console.Clear();
						break;
				}
			}

			Thread.Sleep(50);
		}
	}
}
