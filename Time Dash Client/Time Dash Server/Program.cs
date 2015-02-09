using EZUDP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
	static bool running = true;
	static Game game;

	public static void Main(string[] args)
	{
		Console.Write("Server name: ");
		Game.serverName = Console.ReadLine();

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