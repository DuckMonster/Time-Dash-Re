using EZUDP;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class ServerList
{
	class Server
	{
		ServerList list;

		public int index;

		public string name;
		public string mapname;
		public int numberOfPlayers;
		public string ip;
		long? ping = null;

		bool connected = true;

		public IPEndPoint IP
		{
			get
			{
				return new IPEndPoint(IPAddress.Parse(ip), Port.UDP);
			}
		}

		public string PingString
		{
			get
			{
				if (ping == null) return "???";
				else return ping.ToString();
			}
		}

		public Server(int index, string ip, ServerList list)
		{
			this.index = index;
			this.list = list;
			this.ip = ip;
		}

		public void RequestInfo()
		{
			new Thread(RequestInfoThread).Start();
		}

		void RequestInfoThread()
		{
			try
			{
				IPEndPoint serverIP = IP;

				MessageBuffer msg = new MessageBuffer();
				msg.WriteShort((short)Protocol.RequestInfo);

				list.client.Send(msg.Array, msg.Size, serverIP);
				var answer = list.client.Receive(ref serverIP);

				msg = new MessageBuffer(answer);

				name = msg.ReadString();
				mapname = msg.ReadString();
				numberOfPlayers = msg.ReadInt();
			}
			catch (Exception e)
			{
				connected = false;
			}

			Print();
		}

		public void Ping()
		{
			new Thread(PingThread).Start();
		}

		void PingThread()
		{
			ping = EZUDP.Client.EzClient.Ping(IP);
			Print();
		}

		public static object Lock = new object();

		public void Print()
		{
			lock (Lock)
			{
				Console.SetCursorPosition(0, index);

				if (list.cursor == index)
				{
					Console.ForegroundColor = ConsoleColor.Black;
					Console.BackgroundColor = ConsoleColor.White;
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.BackgroundColor = ConsoleColor.Black;
				}

				if (!connected)
					Console.ForegroundColor = ConsoleColor.Red;

				Console.WriteLine("{0} [{1}] | Map: {2} | {3}/6 players - Ping: {4}", name, ip, mapname, numberOfPlayers, connected ? PingString : "DISCONNECTED");

				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
			}
		}
	}

	public delegate void ConnectTo(string ip);

	IPEndPoint trackerIP = new IPEndPoint(Dns.GetHostAddresses("tracker.timedashgame.com")[0], 1260);

	List<Server> serverList = null;

	int cursor = 0;
	string ip = null;

	UdpClient client;

	Server SelectedServer
	{
		get
		{
			if (serverList.Count == 0) return null;
			return serverList[cursor];
		}
	}

	public ServerList(ConnectTo connectMethod)
	{
		client = new UdpClient();
		client.Client.ReceiveTimeout = 1000;

		FetchServers();

		while (ip == null)
		{
			Update();
		}

		connectMethod(ip);
	}

	void Update()
	{
		if (serverList != null)
			foreach (Server s in serverList)
			{
				s.RequestInfo();
				s.Ping();
			}

		DrawList();

		ConsoleKey key = Console.ReadKey().Key;
		switch (key)
		{
			case ConsoleKey.Enter:
				if (serverList != null && SelectedServer != null)
					ip = SelectedServer.ip;
				break;

			case ConsoleKey.UpArrow:
				if (serverList != null)
					if (cursor > 0) cursor--;
				break;

			case ConsoleKey.DownArrow:
				if (serverList != null)
					if (cursor < serverList.Count-1) cursor++;
				break;

			case ConsoleKey.R:
				FetchServers();
				break;

			case ConsoleKey.C:
				Console.Clear();
				Console.Write("Server IP: ");
				Console.CursorVisible = true;
				ip = Console.ReadLine();
				break;
		}
	}

	void RequestServerInfo(Server s)
	{
		IPEndPoint serverIP = new IPEndPoint(IPAddress.Parse(s.ip), Port.UDP);

		MessageBuffer msg = new MessageBuffer();
		msg.WriteShort((short)Protocol.RequestInfo);

		client.Send(msg.Array, msg.Size, serverIP);
		var answer = client.Receive(ref serverIP);

		msg = new MessageBuffer(answer);

		s.name = msg.ReadString();
	}

	public void DrawList()
	{
		Console.Clear();

		if (serverList != null)
		{
			if (serverList.Count > 0)
				foreach (Server s in serverList)
					s.Print();
			else
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("No servers online at the moment. Why not start one?");
			}
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Failed to connect to server tracker");
		}

		lock (Server.Lock)
		{
			if (serverList != null)
				Console.SetCursorPosition(0, serverList.Count);

			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;

			Console.WriteLine("\nArrow keys to navigate, Enter to connect");
			Console.WriteLine("(C) Connect to IP, (R) Refresh server list");
		}

		Console.CursorVisible = false;
	}

	void FetchServers()
	{
		if (serverList != null) serverList.Clear();
		serverList = null;

		cursor = 0;

		try
		{
			client.Send(new byte[] { 0 }, 1, trackerIP);
			var data = client.Receive(ref trackerIP);

			MessageBuffer msg = new MessageBuffer(data);
			int nmbr = msg.ReadInt();

			serverList = new List<Server>(nmbr);

			for (int i = 0; i < nmbr; i++) 
				serverList.Add(new Server(i, msg.ReadString(), this));
		}
		catch (Exception e)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Couldn't connect to server tracker");
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
}