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
		game = new Game();

		while (running)
		{
			game.Logic();
			Thread.Sleep(1);
		}
	}

	public static void ShutDown()
	{
		running = false;
		game.Dispose();
	}
}