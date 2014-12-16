using System;
using System.Collections.Generic;

using EZUDP;
using EZUDP.Server;
using System.Diagnostics;

public class Game
{
	public const int portTCP = Port.TCP, portUDP = Port.UDP;
	public static string hostIP;

	public static EzServer server;

	public static float delta;
	Stopwatch tickWatch;

	List<Client> clientList = new List<Client>();
	Map currentMap;

	public Game()
	{
		Log.Init();

		server = new EzServer(portTCP, portUDP);

		EzServer.DebugInfo.downData = true;
		EzServer.DebugInfo.upData = true;
		EzServer.DebugInfo.acceptData = true;

		server.OnConnect += OnConnect;
		server.OnDisconnect += OnDisconnect;
		server.OnStart += OnStart;
		server.OnMessage += OnMessage;

		if (hostIP == null)
			server.StartUp();
		else
			server.StartUp(hostIP);

		currentMap = new Map();
		server.OnMessage += currentMap.MessageHandle;
		server.OnDebug += OnDebug;
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

		if (currentMap != null) currentMap.Logic();
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

		currentMap.PlayerJoin(c);

		c.Ping();
	}

	public void OnDisconnect(Client c)
	{
		Log.Write(ConsoleColor.DarkRed, "{0}[{1}, {2}] disconnected!", c.ID, c.tcpAdress, c.udpAdress);
		clientList.Remove(c);

		currentMap.PlayerLeave(c);
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

	public void OnDebug(string s)
	{
		Log.Write(ConsoleColor.Green, s);
	}
}