using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerTracker
{
	class Program
	{
		static void Main(string[] args)
		{
			Program program = new Program();

			while (true)
			{
				program.Update();
				Thread.Sleep(2000);
			}
		}

		public TcpListener listener;
		List<Server> serverList = new List<Server>();

		public Program()
		{
			listener = new TcpListener(IPAddress.Any, 1260);
			new Thread(ListenThread).Start();
			new Thread(AcceptThread).Start();

			PrintInfo();
		}

		public void Update()
		{
			Server[] serverBuffer = serverList.ToArray();

			foreach (Server s in serverBuffer)
				s.Ping();
		}

		public void AcceptThread()
		{
			listener.Start();

			while (true)
			{
				Socket s = listener.AcceptSocket();

				if (!ServerExists(s))
				{
					serverList.Add(new Server(s, this));
					PrintInfo();
				}
			}
		}

		public void ListenThread()
		{
			UdpClient udp = new UdpClient(1260);

			while (true)
			{
				IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);

				try
				{
					var data = udp.Receive(ref ip);

					MessageBuffer msg = new MessageBuffer(data);
					int type = msg.ReadByte();

					if (type == 0)
					{
						MessageBuffer serverMsg = new MessageBuffer();
						serverMsg.WriteInt(serverList.Count);

						foreach (Server s in serverList)
							s.WriteInfoTo(serverMsg);

						udp.SendAsync(serverMsg.Array, serverMsg.Size, ip);
					}

					Console.WriteLine("Sent serverlist to " + ip);
				}
				catch (Exception e)
				{
				}
			}
		}

		public bool ServerExists(Socket s)
		{
			foreach (Server ss in serverList)
			{
				if (ss.ip == ((IPEndPoint)s.RemoteEndPoint).Address.ToString())
					return true;
			}

			return false;
		}

		public void ServerDisconnected(Server s)
		{
			serverList.Remove(s);
			PrintInfo();
		}

		public void PrintInfo()
		{
			lock (serverList)
			{
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Yellow;

				Console.WriteLine("Active servers: {0}", serverList.Count);

				Console.CursorVisible = false;
			}
		}
	}
}
