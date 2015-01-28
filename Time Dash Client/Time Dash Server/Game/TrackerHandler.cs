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

	void ConnectThread()
	{
		try
		{
			trackerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			trackerSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345));

			connected = true;
			Log.Write(ConsoleColor.Green, "Connected to server tracker");
			Log.Write(ConsoleColor.Green, "Your server will show up in the server list");
		}
		catch (Exception e)
		{
		}
	}

	public void CheckConnection()
	{
		if (connected)
		{
			try
			{
				trackerSocket.Send(new byte[] { });
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
		msg.WriteInt(game.map.NumberOfPlayers);

		Game.server.SendExternal(msg, ip);
	}
}