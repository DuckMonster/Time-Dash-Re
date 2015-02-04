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

	bool connected = true;
	Stopwatch retryTimer;

	public TrackerHandler(Game g)
	{
		game = g;
		Connect();
	}

	public void Connect()
	{
		new Thread(ConnectThread).Start();
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
		if (connected)
		{
			try
			{
				trackerSocket.Send(new byte[] { 0 });
			}
			catch (Exception e)
			{
				connected = false;
				Log.Write(ConsoleColor.Red, "Disconnected from tracker server... Trying to reconnect...");
			}
		}
		else
		{
			if (retryTimer == null) retryTimer = Stopwatch.StartNew();

			if (retryTimer.Elapsed.Seconds > 2)
			{
				Connect();
				retryTimer = null;
			}
		}
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