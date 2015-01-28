using EZUDP;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
public class MainClass
{
	class Server
	{
		public string name;
		public string ip;

		public Server(string n, string i)
		{
			name = n;
			ip = i;
		}
	}

	static List<Server> serverList = new List<Server>();

	static void Main(string[] args)
	{
		#region Name

		Console.Write("Name: ");
		Game.myName = Console.ReadLine();
		Console.Clear();

		#endregion

		FetchServers();

		bool valid = false;
		int cursor = 0;

		while (!valid)
		{
			Console.Clear();

			UdpClient udpClient = new UdpClient();

			for (int i = 0; i < serverList.Count; i++)
			{
				IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse(serverList[i].ip), Port.UDP);

				MessageBuffer msg = new MessageBuffer();
				msg.WriteShort((short)Protocol.RequestInfo);

				udpClient.Send(msg.Array, msg.Size, serverIP);
				var answer = udpClient.Receive(ref serverIP);

				msg = new MessageBuffer(answer);

				serverList[i].name = msg.ReadString();
			}

			for (int i = 0; i < serverList.Count + 1; i++)
			{
				if (cursor == i)
				{
					Console.ForegroundColor = ConsoleColor.Black;
					Console.BackgroundColor = ConsoleColor.White;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.BackgroundColor = ConsoleColor.Black;
				}

				if (i < serverList.Count)
					Console.WriteLine("({0}) {1} [{2}]", i, serverList[i].name, serverList[i].ip);
				else
					Console.WriteLine("(N) Custom...");
			}

			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;

			ConsoleKey key = Console.ReadKey().Key;
			switch (key)
			{
				case ConsoleKey.Enter:
					Game.hostIP = serverList[cursor].ip;
					valid = true;
					break;

				case ConsoleKey.UpArrow:
					if (cursor > 0) cursor--;
					break;

				case ConsoleKey.DownArrow:
					if (cursor < serverList.Count) cursor++;
					break;
			}
		}

		using (Program p = new Program(1025, 768, new GraphicsMode(new ColorFormat(32), 24, 8, 3)))
		{
			p.Run(200.0);
		}
	}

	static void FetchServers()
	{
		IPEndPoint serverTrackerIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
		UdpClient client = new UdpClient();

		client.Connect(serverTrackerIP);
		client.Send(new byte[] { 0 }, 1);

		int nmbr = BitConverter.ToInt32(client.Receive(ref serverTrackerIP), 0);

		for(int i=0; i<nmbr; i++) {
			var data = client.Receive(ref serverTrackerIP);
			BinaryReader stream = new BinaryReader(new MemoryStream(data));

			int nameStringLen = stream.ReadInt32();
			string name = "";
			for(int j=0; j<nameStringLen; j++)
				name += stream.ReadChar();

			int ipStringLen = stream.ReadInt32();
			string ip = "";
			for(int j=0; j<ipStringLen; j++)
				ip += stream.ReadChar();

			serverList.Add(new Server(name, ip));

			stream.Dispose();
		}
	}
}