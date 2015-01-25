using OpenTK;
using System;
using System.Collections.Generic;
public class CPMap : Map
{
	List<CPPoint> pointList = new List<CPPoint>();

	public CPMap(int id, string filename)
		: base(id, filename, GameMode.ControlPoints)
	{
	}

	public override void Logic()
	{
		base.Logic();
		foreach (CPPoint p in pointList) p.Logic();
	}

	public override void Draw()
	{
		foreach (CPPoint p in pointList) p.Draw();
		base.Draw();
	}

	public override void MessageHandle(EZUDP.MessageBuffer msg)
	{
		try
		{
			if ((Protocol)msg.ReadShort() == Protocol.MapArgument)
			{
				switch ((Protocol_CP)msg.ReadShort())
				{
					case Protocol_CP.TeamOwner:
						pointList[msg.ReadByte()].SetOwner(GetTeam(msg.ReadByte()));
						break;

					case Protocol_CP.TeamProgress:
						pointList[msg.ReadByte()].SetProgress(msg.ReadFloat());
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