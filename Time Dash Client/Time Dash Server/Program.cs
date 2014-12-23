using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
	static bool running = true;
	static Game game;

	static Stopwatch logicTime;

	public static void Main(string[] args)
	{
		bool valid = false;

		while (!valid)
		{
			Console.Write("Host locally? ");
			char answer = Console.ReadKey().KeyChar;

			if (answer == 'y') { Game.hostIP = "127.0.0.1"; valid = true; }
			else if (answer == 'n') { valid = true; }
			else
			{
				Console.Clear();
				Console.WriteLine("y/n only!");
			}
		}

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