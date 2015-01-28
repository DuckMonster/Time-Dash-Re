using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TrackerTester
{
	class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				ConsoleKey key = Console.ReadKey().Key;
				if (key == ConsoleKey.Escape) break;

				IPEndPoint trackerIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6556);

				UdpClient client = new UdpClient();
				client.Connect(trackerIP);

				if (key == ConsoleKey.R)
				{
					MessageBuffer msg = new MessageBuffer();
					msg.WriteByte(0);
					client.Send(msg.Array, msg.Size);

					int nmbr = BitConverter.ToInt32(client.Receive(ref trackerIP), 0);
					for (int i = 0; i < nmbr; i++)
					{
						var servInfo = client.Receive(ref trackerIP);
						MessageBuffer info = new MessageBuffer(servInfo);

						Console.WriteLine(info.ReadString() + "[" +
							info.ReadString() + ":" + info.ReadShort() + "]");
					}
				}
				else if (key == ConsoleKey.N)
				{
					MessageBuffer msg = new MessageBuffer();
					msg.WriteByte(1);
					msg.WriteString("Awesome server!");

					client.Send(msg.Array, msg.Size);
				}

				Console.WriteLine("---------");

				client.Close();
			}
		}
	}
}
