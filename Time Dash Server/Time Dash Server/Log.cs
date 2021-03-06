﻿using EZUDP.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

enum LogMode
{
	Debug,
	Message,
	Neutral,
	Command,
	CommandResult
}

class Log
{
	class LogMessage
	{
		public string message;
		public ConsoleColor color;

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

		public void Print(string[] filter)
		{
			bool pass = false;

			if (filter != null)
			{
				foreach (string s in filter)
					if (message.ToLower().Contains(s.ToLower()))
						pass = true;

				if (!pass) return;
			}

			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}

	static DateTime startTime;
	static bool running = false;

	static string[] filterList;
	static List<LogMessage> logMessageList = new List<LogMessage>();
	public static LogMode mode = LogMode.Message;
	static Thread inputThread;

	private static int[] tickAvg = new int[150];
	private static int tickAvgIndex = 0;

	static Stopwatch networkWatch;
	static int currentUp = 0, currentDown = 0, maxUp = 0, maxDown = 0;

	public static void Init()
	{
		if (inputThread != null) return;

		startTime = DateTime.Now;

		running = true;

		ShowMessages();
		inputThread = new Thread(InputThread);
		inputThread.Start();

		LogCommand.Init();
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

	public static void Write(string text, params object[] args)
	{
		Write(ConsoleColor.Gray, text, args);
	}
	public static void Write(ConsoleColor c, string text, params object[] args)
	{
		LogMessage msg = new LogMessage(string.Format(text, args), c);

		logMessageList.Add(msg);
		if (mode == LogMode.Message) msg.Print(filterList);
	}

	public static void Clear()
	{
		logMessageList.Clear();
	}

	public static void Logic()
	{
		if (CanDebug) debugTimer = debugInterval;
		debugTimer -= Game.delta;

		if (CanDebug)
		{
			Console.Clear();
		}

		ShowUptime();
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
	}

	public static void CalculateNetworkData()
	{
		currentDown = EzServer.DownBytes;
		currentUp = EzServer.UpBytes;

		if (currentDown > maxDown) maxDown = currentDown;
		if (currentUp > maxUp) maxUp = currentUp;
	}

	static void ShowUptime()
	{
		TimeSpan time = DateTime.Now - startTime;
		Debug("Uptime {0:00}:{1:00}:{2:00}", time.Hours, time.Minutes, time.Seconds);
	}

	static void ShowFPS()
	{
		if (CanDebug)
		{
			//Calculate average
			int tick = 0;

			for (int i = 0; i < tickAvg.Length; i++)
				tick += tickAvg[i];

			tick /= tickAvg.Length;

			//

			Debug("Ticks/S: {1} ~ {0:0}", tickAvg[tickAvgIndex], tick);
		}
	}

	static void ShowNetworkData()
	{
		Debug("U: {0} B/s  \tMax: {4} b/s\nD: {1} B/s  \tMax: {5} b/s\n\nTotal\nU: {2} B\nD: {3} B",
			currentUp, currentDown, EzServer.UpBytesTotal, EzServer.DownBytesTotal, maxUp, maxDown);
	}

	public static void ShowMessages()
	{
		Console.Clear();

		if (filterList != null)
			PrintFilter();

		mode = LogMode.Message;

		foreach (LogMessage s in logMessageList)
		{
			s.Print(filterList);
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
						ClearFilter();
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

					case ConsoleKey.Enter:
						mode = LogMode.Command;
						CommandInput();
						break;
				}
			}

			Thread.Sleep(50);
		}
	}

	static void CommandInput()
	{
		Console.Clear();
		Console.Write('>');
		string[] command = Console.ReadLine().Split(' ');

		mode = LogMode.CommandResult;

		Console.Clear();
		LogCommand.RunCommand(command);

		ShowMessages();
	}

	public static void SetMessageFilter(string[] filter)
	{
		filterList = filter;
		PrintFilter();
	}

	static void PrintFilter()
	{
		Console.Clear();
		Console.ForegroundColor = ConsoleColor.Cyan;
		Console.Write("Filter: ");
		foreach (string s in filterList)
			Console.Write(s + " ");

		Console.WriteLine();
	}

	public static void ClearFilter()
	{
		filterList = null;
	}
}
