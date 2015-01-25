using System;
using OpenTK;
using EZUDP;

public class DMMap : Map
{
	public Scoreboard scoreboard;

	public DMMap(int id, string filename)
		: base(id, filename, GameMode.DeathMatch)
	{
		scoreboard = new Scoreboard(20, 5f, 6f, new Vector2(5, 5), this);
	}

	public override void PlayerJoin(int id, string name)
	{
		playerList[id] = new DMPlayer(id, name, new Vector2(4, 10), this);
		scoreboard.SetName(id, name);
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void Logic()
	{
		base.Logic();
		if (scoreboard != null) scoreboard.Logic();
	}

	public override void Draw()
	{
		//scoreboard.Draw();
		base.Draw();
	}

	public override void MessageHandle(MessageBuffer msg)
	{
		try
		{
			if ((Protocol)msg.ReadShort() == Protocol.MapArgument)
			{
				switch ((Protocol_DM)msg.ReadShort())
				{
					case Protocol_DM.PlayerScore:
						scoreboard.SetScore(msg.ReadByte(), msg.ReadShort());
						break;
				}
			}
		}
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Yellow, "Packet corrupt!");
			Log.Write(ConsoleColor.Red, e.Message);
			Log.Write(ConsoleColor.DarkRed, e.StackTrace);
		}

		if (msg != null) msg.Reset();
			
		base.MessageHandle(msg);
	}
}