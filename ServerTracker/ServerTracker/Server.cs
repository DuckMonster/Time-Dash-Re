using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerTracker
{
	class Server
	{
		string name;
		public IPEndPoint ip;

		MessageBuffer buffer;

		public Server(IPEndPoint ip, string name)
		{
			this.ip = ip;
			this.name = name;

			buffer = new MessageBuffer();
			buffer.WriteString(name);
			buffer.WriteString(ip.Address.ToString());
			buffer.WriteShort(ip.Port);

			Console.WriteLine("Server \"{0}\" started at {1}", name, ip.ToString());
		}

		public void SendInfoTo(IPEndPoint ip, UdpClient client)
		{
			client.Send(buffer.Array, buffer.Size, ip);
		}

		public void Ping()
		{
		}
	}
}
