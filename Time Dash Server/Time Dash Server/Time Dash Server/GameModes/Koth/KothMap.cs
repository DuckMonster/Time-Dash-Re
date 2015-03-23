using System;
using EZUDP;
using EZUDP.Server;
using OpenTK;
using System.Drawing;

public class KothMap : Map
{
	public KothPoint point;

	public KothMap(string filename)
		: base(filename, GameMode.KingOfTheHill)
	{
	}

	public KothMap(string filename, Player[] players)
		: base(filename, GameMode.KingOfTheHill, players)
	{
	}

	public override void SceneZone(int typeID, TKTools.Polygon p)
	{
		RectangleF rect = p.Bounds;

		if (typeID == 1)
		{
			point = new KothPoint(new Vector2(rect.X + rect.Width/2, rect.Y), this);
		}
		base.SceneZone(typeID, p);
	}

	public override Player PlayerJoin(Client c, string name)
	{
		Player p = base.PlayerJoin(c, name);
		point.SendScoreboardToPlayer(p);

		return p;
	}

	public override Player CreatePlayer(int id, string name, Client c)
	{
		return new KothPlayer(id, name, c, Vector2.Zero, this);
	}

	public override void Logic()
	{
		base.Logic();
		point.Logic();
	}

	public override OpenTK.Vector2 GetFreeSpawnPosition(Player p)
	{
		Vector2 pos;

		do
		{
			double x = rng.NextDouble() - 0.5, y = rng.NextDouble() - 0.5;
			pos = new Vector2((float)x * scene.Width, (float)y * scene.Height) + scene.originOffset;
		} while (GetCollision(pos, p.size) || (point.position - pos).Length <= 15f);

		return pos;
	}

	public override void MessageHandle(Client c, MessageBuffer msg)
	{
		try
		{
			if ((Protocol)msg.ReadShort() == Protocol.MapArgument)
			{
				switch ((Protocol_KOTH)msg.ReadShort())
				{
				}
			}
		}
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Red, "Packet corrupt!\n" + e);
			Log.Write(ConsoleColor.DarkRed, e.StackTrace);
		}

		if (msg != null) msg.Reset();

		base.MessageHandle(c, msg);
	}
}