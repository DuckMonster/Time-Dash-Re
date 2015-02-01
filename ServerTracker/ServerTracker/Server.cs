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
		Program program;

		public string ip;
		IPEndPoint endpoint;
		Socket socket;

		public Server(Socket s, Program prog)
		{
			this.program = prog;
			socket = s;

			endpoint = (IPEndPoint)s.RemoteEndPoint;
			ip = endpoint.Address.ToString();
		}

		public void WriteInfoTo(MessageBuffer msg)
		{
			msg.WriteString(ip);
		}

		public void Ping()
		{
			try
			{
				socket.Send(new byte[] { 0 });
			}
			catch (Exception e)
			{
				program.ServerDisconnected(this);
			}
		}
	}
}
