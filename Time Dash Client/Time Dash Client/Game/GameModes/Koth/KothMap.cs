using System;
using OpenTK;
using EZUDP;
using System.Drawing;

public class KothMap : Map
{
	public KothPoint point;

	public KothMap(int id, string filename)
		: base(id, filename, GameMode.KingOfTheHill)
	{
	}

	/*
	public override void SceneEvent(int typeID, TKTools.Polygon p)
	{
		RectangleF rect = p.Bounds;

		if (typeID == 1)
		{
			point = new KothPoint(new Vector2(rect.X + rect.Width / 2, rect.Y), this);
		}
		base.SceneEvent(typeID, p);
	}
	*/

	public override void PlayerJoin(int id, string name)
	{
		playerList[id] = new KothPlayer(id, name, new Vector2(4, 10), this);
		point.SetName(id, name);
	}

	public override void Dispose()
	{
		base.Dispose();
		point.Dispose();
	}

	public override void Logic()
	{
		base.Logic();
		point.Logic();
	}

	public override void Draw()
	{
		point.Draw();
		base.Draw();
	}

	public override void MessageHandle(MessageBuffer msg)
	{
		try
		{
			if ((Protocol)msg.ReadShort() == Protocol.MapArgument)
			{
				switch ((Protocol_KOTH)msg.ReadShort())
				{
					case Protocol_KOTH.PlayerScore:
						point.SetScore(msg.ReadByte(), msg.ReadShort());
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