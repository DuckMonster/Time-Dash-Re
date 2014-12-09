using System;
using System.Collections.Generic;

using EZUDP;
using EZUDP.Server;
using System.Diagnostics;

public class Game
{
	public const int portTCP = Port.TCP, portUDP = Port.UDP;

	public static EzServer server;

	public static float delta;
	Stopwatch tickWatch;

	List<Client> clientList = new List<Client>();
	List<Map> mapList = new List<Map>();

	public Game()
	{
		Log.Init();

		server = new EzServer(portTCP, portUDP);

		server.OnConnect += OnConnect;
		server.OnDisconnect += OnDisconnect;
		server.OnStart += OnStart;
		server.OnMessage += OnMessage;

		server.StartUp();

		Map m = new Map();
		server.OnMessage += m.MessageHandle;

		mapList.Add(m);
	}

	public void Dispose()
	{
		Log.ShutDown();
		server.Close();
		server = null;
	}

	public void Logic()
	{
		CalculateDelta();
		Log.Logic();

		server.Update();
		Log.Debug("Number of clients: {0}", clientList.Count);
	}

	public void CalculateDelta()
	{
		if (tickWatch == null) tickWatch = Stopwatch.StartNew();

		tickWatch.Stop();
		delta = tickWatch.ElapsedTicks / (float)Stopwatch.Frequency;
		tickWatch.Restart();

		Log.CalculateTick(delta);
	}

	//ONLINE
	public void OnStart()
	{
		Log.Write(ConsoleColor.Green, "Server started!");
	}

	public void OnConnect(Client c)
	{
		Log.Write(ConsoleColor.Green, "{0}[{1}, {2}] connected!", c.ID, c.tcpAdress, c.udpAdress);
		clientList.Add(c);

		mapList[0].PlayerJoin(c);
	}

	public void OnDisconnect(Client c)
	{
		Log.Write(ConsoleColor.DarkRed, "{0}[{1}, {2}] disconnected!", c.ID, c.tcpAdress, c.udpAdress);
		clientList.Remove(c);

		mapList[0].PlayerLeave(c);
	}

	public void OnMessage(Client c, MessageBuffer msg)
	{
		try
		{
			msg.Reset();
		}		
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Red, "Packet corrupt!\n" + e.Message);
		}
	}

	public void OnException(Exception e)
	{
		Log.Write(ConsoleColor.Red, e.Message);
	}
}