using EZUDP.Server;
using System;
using System.Collections.Generic;
using System.IO;
public class LogCommand
{
	public delegate void CommandAction(string[] args);
	static List<LogCommand> commandList = new List<LogCommand>();
	public static void Init()
	{
		AddCommand("print", Foo);
		AddCommand("grep", Grep);
		AddCommand("clear", Clear);
		AddCommand("debug", Debug);
		AddCommand("kill", KillPlayer);
		AddCommand("stat", PlayerStats);
		AddCommand("load", Load);
	}
	static void AddCommand(string comm, CommandAction action)
	{
		commandList.Add(new LogCommand(comm, action));
	}

	public static void RunCommand(string[] args)
	{
		try
		{
			foreach (LogCommand c in commandList)
				c.Run(args);
		}
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Yellow, "Running the command went wrong...");
			Log.Write(ConsoleColor.Red, e.Message);
			Log.Write(ConsoleColor.DarkRed, e.StackTrace);
		}
	}


	string[] command;
	CommandAction action;

	public LogCommand(string command, CommandAction action)
	{
		this.command = command.Split(' ');
		this.action = action;
	}

	public void Run(string[] args)
	{
		for (int i = 0; i < command.Length; i++)
			if (command[i] != args[i]) return;

		action(args);
	}



	//Actions
	static void Foo(string[] args)
	{
		Console.WriteLine(args[1]);
	}

	static void Grep(string[] args)
	{
		Log.SetMessageFilter(args[1].Split(','));
		Log.ShowMessages();
	}

	static void Clear(string[] args)
	{
		Log.Clear();
		Log.ShowMessages();
	}

	static void Debug(string[] args)
	{
		bool value = int.Parse(args[2]) == 1;

		switch (args[1])
		{
			case "serverup":
				EzServer.DebugInfo.upData = value;
				break;

			case "serverdown":
				EzServer.DebugInfo.downData = value;
				break;

			case "serveraccept":
				EzServer.DebugInfo.acceptData = value;
				break;

			case "position":
				Player.SEND_SERVER_POSITION = value;
				break;
		}

		Log.ShowMessages();
	}

	static void KillPlayer(string[] args)
	{
		byte id = byte.Parse(args[1]);

		Player p = Map.currentMap.GetPlayer(id);
		if (p != null) p.Die();
	}

	static void PlayerStats(string[] args)
	{
		byte id = byte.Parse(args[1]);

		Player p = Map.currentMap.GetPlayer(id);
		if (p != null)
		{
			Log.Write(ConsoleColor.Green, "Stats for {0}", id);
			Log.Write(ConsoleColor.Yellow, "Health {0} | Position {1}", p.health, p.Position);
		}
		else
		{
			Log.Write(ConsoleColor.Red, "That player doesn't exist");
		}
	}

	static void Load(string[] args)
	{
		if (!File.Exists("Maps/" + args[1] + ".tdm"))
		{
			Log.Write(ConsoleColor.Red, "Map \"" + args[1] + "\" doesn't exist!");
			return;
		}

		Game.currentGame.LoadMap(args[1]);
	}
}