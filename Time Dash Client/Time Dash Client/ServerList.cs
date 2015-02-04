using EZUDP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class ServerList
{
	class Server
	{
		IPEndPoint endpoint;
		public string ip;

		string name = null;
		long ping;
		int players;

		bool connected = true;

		public Server(string ip)
		{
			this.ip = ip;
			endpoint = new IPEndPoint(IPAddress.Parse(ip), Port.UDP);
		}

		public void Print()
		{
			if (name != null) {
				Console.WriteLine("{0} [{1}] | Players: {2}/6 | Ping: {3}", name, ip, players, ping);
			}
			else
			{
				if (!connected)
					Console.ForegroundColor = ConsoleColor.Red;

				Console.WriteLine(ip);
			}

			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public void FetchInfo()
		{
			using (UdpClient client = new UdpClient())
			{
				client.Client.ReceiveTimeout = 500;

				try
				{
					client.Connect(endpoint);

					MessageBuffer msg = new MessageBuffer();
					msg.WriteShort((short)Protocol.RequestInfo);

					client.Send(msg.Array, msg.Size);
					msg = new MessageBuffer(client.Receive(ref endpoint));

					name = msg.ReadString();
					msg.ReadString();
					players = msg.ReadByte();

					ping = EZUDP.Client.EzClient.Ping(endpoint);
				}
				catch (Exception e)
				{
					connected = false;
				}
			}
		}
	}

	public delegate void ConnectMethod(string ip);
	List<Server> serverList = new List<Server>();

	public ServerList(ConnectMethod method)
	{
		IPEndPoint trackerIP = new IPEndPoint(Dns.GetHostAddresses("tracker.timedashgame.com")[0], 1260);
		Console.Clear();

		try
		{
			UdpClient client = new UdpClient();
			client.Client.ReceiveTimeout = 2000;

			client.Connect(trackerIP);
			client.Send(new byte[] { 0 }, 1);
			MessageBuffer msg = new MessageBuffer(client.Receive(ref trackerIP));

			int nmbr = msg.ReadInt();

			if (nmbr > 0)
			{
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine("Choose a server:");

				for (int i = 0; i < nmbr; i++)
				{
					serverList.Add(new Server(msg.ReadString()));
					serverList[i].FetchInfo();

					Console.ForegroundColor = ConsoleColor.Gray;
					Console.Write("({0}) ", i);
					serverList[i].Print();
				}
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("No servers online. Why not start one?");
			}
		}
		catch (Exception e)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Couldn't connect to server tracker :(");
		}

		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine();

		if (serverList.Count > 1)
			Console.WriteLine("(0-{0}) Connect to server", serverList.Count-1);
		if (serverList.Count == 1)
			Console.WriteLine("(0) Connect to server", serverList.Count);

		Console.WriteLine("(C) Connect to custom IP");

		string ip = null;

		while (ip == null)
		{
			try
			{
				char c = Console.ReadKey().KeyChar;

				if (c == 'c' || c == 'C')
				{
					Console.Clear();
					Console.Write("Connect to: ");
					ip = Console.ReadLine();
				}
				else
				{
					int index = int.Parse(c.ToString());
					ip = serverList[index].ip;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("What?");
			}
		}

		method(ip);
	}
}