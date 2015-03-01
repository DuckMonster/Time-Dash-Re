using EZUDP;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class TrackerHandler
{
	Game game;
	Socket trackerSocket;

	bool connected = false;
	Stopwatch retryTimer;

	public TrackerHandler(Game g)
	{
		game = g;
		Connect();
	}

	public void Connect()
	{
		//new Thread(ConnectThread).Start();
	}

	static object Lock = new object();

	void ConnectThread()
	{
		lock (Lock)
		{
			if (connected) return;

			try
			{
				trackerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
				trackerSocket.Connect(new IPEndPoint(Dns.GetHostAddresses("tracker.timedashgame.com")[0], 1260));
				//trackerSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1260));
				trackerSocket.SendTimeout = 100;

				connected = true;
				Log.Write(ConsoleColor.Green, "Connected to server tracker");
				Log.Write(ConsoleColor.Green, "Your server will show up in the server list");
			}
			catch (Exception e)
			{
			}
		}
	}

	public void CheckConnection()
	{
		if (retryTimer == null) retryTimer = Stopwatch.StartNew();

		if (retryTimer.Elapsed.Seconds > 2)
		{
			new Thread(CheckConnectionThread).Start();
			retryTimer = null;
		}
	}

	public void CheckConnectionThread()
	{
		if (connected)
		{
			try
			{
				if (trackerSocket == null) return;

				trackerSocket.Send(new byte[] { 0 });
				if (trackerSocket.Available > 0)
					trackerSocket.Receive(new byte[10]);
			}
			catch (Exception e)
			{
				connected = false;
				Log.Write(ConsoleColor.Red, "Disconnected from tracker server... Trying to reconnect...");
			}
		}
		else
			Connect();
	}

	public void SendInfoTo(IPEndPoint ip)
	{
		MessageBuffer msg = new MessageBuffer();
		msg.WriteString(Game.serverName);
		msg.WriteString(game.map.filename);
		msg.WriteByte(game.map.NumberOfPlayers);

		Game.server.SendExternal(msg, ip);
	}
}