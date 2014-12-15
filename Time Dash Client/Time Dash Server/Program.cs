using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
	static bool running = true;
	static Game game;

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

		while (running)
		{
			game.Logic();
			Thread.Sleep((int)(1000.0 / 180));
		}
	}

	public static void ShutDown()
	{
		running = false;
		game.Dispose();
	}
}