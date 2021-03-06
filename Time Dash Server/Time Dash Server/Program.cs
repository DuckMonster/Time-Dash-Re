﻿using EZUDP;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{
	static bool running = true;
	static Game game;

	public static void Main(string[] args)
	{
		if (args.Length == 0)
		{
			Console.Write("Server name: ");
			Game.serverName = Console.ReadLine();
		} else
		{
			Game.serverName = args[0];
			TrackerHandler.doConnect = false;
		}

		UdpClient client = new UdpClient();

		MessageBuffer msg = new MessageBuffer();
		msg.WriteByte(1);
		msg.WriteShort(Port.UDP);

		client.Send(msg.Array, msg.Size, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));
		client.Close();

		game = new Game();

		Multimedia.Timer timer = new Multimedia.Timer();
		timer.Period = 7;
		timer.Resolution = 0;
		timer.Mode = Multimedia.TimerMode.Periodic;
		timer.Tick += timer_Tick;
		timer.Start();

		while (running)
		{
			Thread.Sleep(500);
			if (!timer.IsRunning) Console.WriteLine("Timer stopped....");
		}
	}

	static void timer_Tick(object sender, EventArgs e)
	{
		game.Logic();
	}

	static void shit_Tick(object sender, EventArgs e)
	{
		Console.WriteLine("Shit");
	}

	public static void ShutDown()
	{
		running = false;
		game.Dispose();
	}
}