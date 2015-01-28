﻿using System;
using System.Collections.Generic;

using EZUDP;
using EZUDP.Server;
using System.Diagnostics;
using System.IO;

public class Game
{
	public const int portTCP = Port.TCP, portUDP = Port.UDP;
	public static string hostIP;

	public static Game currentGame;

	public static EzServer server;
	
	public static float delta;
	Stopwatch tickWatch;

	List<Client> clientList = new List<Client>();
	Map map;

	public Game()
	{
		currentGame = this;

		Log.Init();

		server = new EzServer(portTCP, portUDP);

		server.OnConnect += OnConnect;
		server.OnDisconnect += OnDisconnect;
		server.OnStart += OnStart;
		server.OnMessage += OnMessage;

		if (hostIP == null)
			server.StartUp();
		else
			server.StartUp(hostIP);

		server.OnDebug += OnDebug;
		server.OnException += OnException;

		LoadMap("temple");
	}

	public void LoadMap(string filename)
	{
		if (!filename.EndsWith(".tdm")) filename += ".tdm";

		if (!File.Exists("Maps/" + filename))
		{
			Log.Write(ConsoleColor.Red, "Map \"" + filename + "\" doesn't exist!");
			return;
		}

		if (map != null)
		{
			server.OnMessage -= map.MessageHandle;
			map = null;
		}

		string mapname;
		GameMode mode;
		string modeName = "Unknown";

		using (BinaryReader reader = new BinaryReader(new FileStream("Maps/" + filename, FileMode.Open)))
		{
			mapname = reader.ReadString();
			mode = (GameMode)reader.ReadInt32();
		}

		switch (mode)
		{
			case GameMode.KingOfTheHill:
				map = new KothMap(filename);
				modeName = "King of the Hill";
				break;

			case GameMode.DeathMatch:
				map = new DMMap(filename);
				modeName = "Deathmatch";
				break;

			case GameMode.ControlPoints:
				map = new CPMap(filename);
				modeName = "Control Points";
				break;
		}

		server.OnMessage += map.MessageHandle;

		Log.Write(ConsoleColor.Yellow, "Loaded \"" + mapname + "\" | " + modeName);
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

		if (map != null) map.Logic();
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

		c.Ping();
	}

	public void OnDisconnect(Client c)
	{
		Log.Write(ConsoleColor.DarkRed, "{0}[{1}, {2}] disconnected!", c.ID, c.tcpAdress, c.udpAdress);
		clientList.Remove(c);

		map.PlayerLeave(c);
	}

	public void OnMessage(Client c, MessageBuffer msg)
	{
		try
		{
			switch ((Protocol)msg.ReadShort())
			{
				case Protocol.PlayerName:
					map.PlayerJoin(c, msg.ReadString());
					break;
			}

			msg.Reset();
		}		
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Yellow, "Packet corrupt!");
			Log.Write(ConsoleColor.Red, e.Message);
			Log.Write(ConsoleColor.DarkRed, e.StackTrace);
		}
	}

	public void OnException(Exception e)
	{
		Log.Write(ConsoleColor.Yellow, "Exception!");
		Log.Write(ConsoleColor.Red, e.Message);
		Log.Write(ConsoleColor.DarkRed, e.StackTrace);
	}

	public void OnDebug(string s)
	{
		Log.Write(ConsoleColor.Green, s);
	}
}