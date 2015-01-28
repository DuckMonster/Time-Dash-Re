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
				Thread.Sleep(100);
			}
		}

		UdpClient socket;
		List<Server> serverList = new List<Server>();

		public Program()
		{
			socket = new UdpClient(12345);
			new Thread(ListenThread).Start();
		}

		public void Update()
		{
		}

		public void ListenThread()
		{
			while (true)
			{
				IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
				var data = socket.Receive(ref ip);

				MessageBuffer msg = new MessageBuffer(data);
				int type = msg.ReadByte();

				if (type == 0)
				{
					Console.WriteLine("Sending servers to " + ip);
					socket.Send(BitConverter.GetBytes(serverList.Count), 4, ip);

					foreach (Server s in serverList)
						s.SendInfoTo(ip, socket);
				}
				if (type == 1)
				{
					serverList.Add(new Server(ip, msg.ReadString()));
				}
			}
		}

		public void AddServer(string name, IPEndPoint ip)
		{
			serverList.Add(new Server(ip, name));
		}
	}
}
