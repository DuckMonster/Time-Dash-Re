using EZUDP;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class MainClass
{
	static void Main(string[] args)
	{
		if (args.Length < 2)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("Welcome to Time Dash Alpha!\n\n");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("This game is current in early development");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(", so please don't take anything you see as representative of the final product.\nWe are working hard to bring this project to a release! You can join us in our\nadventure on:");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write("\nreddit.com/r/timedash");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(" or ");
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("timedashgame.com");

			Console.ForegroundColor = ConsoleColor.Gray;

			Console.WriteLine("\nCome and hang and drop a comment of what you think of the game if you'd like!\nIn either case, we hope you enjoy our game :)\n\n");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine("\t--DuckMonster");
			Console.WriteLine("\n\nPress any key to continue...");

			Console.ReadKey();
			Console.Clear();

			Console.Write("Name: ");
			Game.myName = Console.ReadLine();
			Console.Clear();

			new ServerList(ConnectTo);
		} else
		{
			Game.myName = args[0];
			ConnectTo(args[1]);
		}
	}
	
	static void ConnectTo(string ip)
	{
		Game.hostIP = ip;

		using (Program p = new Program(1025, 768, new GraphicsMode(new ColorFormat(32), 24, 8,8)))
		{
			p.Run(142.0, 120.0);
		}
	}
}