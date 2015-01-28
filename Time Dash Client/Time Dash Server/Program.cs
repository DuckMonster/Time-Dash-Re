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
		string name = Console.ReadLine();

		UdpClient client = new UdpClient();
		client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));

		MemoryStream stream = new MemoryStream();
		stream.WriteByte(1);
		stream.Write(BitConverter.GetBytes(name.Length), 0, 4);
		foreach (char c in name)
			stream.WriteByte((byte)c);

		var data = stream.ToArray();
		client.Send(data, data.Length);

		stream.Dispose();
		client.Close();

		game = new Game();

		Multimedia.Timer timer = new Multimedia.Timer();
		timer.Period = 5;
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

	public static void ShutDown()
	{
		running = false;
		game.Dispose();
	}
}