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

		public string name;
		public string mapname;
		public int numberOfPlayers;
		public string ip;
		long? ping = null;

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

		public Server(string ip, ServerList list)
		{
			this.list = list;
			this.ip = ip;
		}

		public void RequestInfo()
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

		public void Ping()
		{
			new Thread(PingThread).Start();
		}

		void PingThread()
		{
			ping = EZUDP.Client.EzClient.Ping(IP);
		}
	}

	public delegate void ConnectTo(string ip);

	IPEndPoint trackerIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);

	List<Server> serverList = new List<Server>();

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
		FetchServers();

		while (ip == null)
		{
			Update();
		}

		connectMethod(ip);
	}

	void Update()
	{
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
				ip = SelectedServer.ip;
				break;

			case ConsoleKey.UpArrow:
				if (cursor > 0) cursor--;
				break;

			case ConsoleKey.DownArrow:
				if (cursor < serverList.Count-1) cursor++;
				break;

			case ConsoleKey.R:
				serverList.Clear();
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

		for (int i = 0; i < serverList.Count; i++)
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
				Console.WriteLine("{1} [{2}] | Map: {3} | {4}/6 players - Ping: {5}", i, serverList[i].name, serverList[i].ip, serverList[i].mapname, serverList[i].numberOfPlayers, serverList[i].PingString);
		}

		Console.ForegroundColor = ConsoleColor.White;
		Console.BackgroundColor = ConsoleColor.Black;

		Console.WriteLine("-------");
		Console.WriteLine("(C) Custom, (R) Refresh server list");

		Console.CursorVisible = false;
	}

	void FetchServers()
	{
		try
		{
			client.Send(new byte[] { 0 }, 1, trackerIP);
			var data = client.Receive(ref trackerIP);

			MessageBuffer msg = new MessageBuffer(data);
			int nmbr = msg.ReadInt();

			for (int i = 0; i < nmbr; i++)
				serverList.Add(new Server(msg.ReadString(), this));
		}
		catch (Exception e)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Couldn't connect to server tracker");
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}
}