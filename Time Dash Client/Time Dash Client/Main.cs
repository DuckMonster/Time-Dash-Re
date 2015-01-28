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
		Console.Write("Name: ");
		Game.myName = Console.ReadLine();
		Console.Clear();

		new ServerList(ConnectTo);
	}

	static void ConnectTo(string ip)
	{
		Game.hostIP = ip;

		using (Program p = new Program(1025, 768, new GraphicsMode(new ColorFormat(32), 24, 8, 3)))
		{
			p.Run(200.0);
		}
	}
}