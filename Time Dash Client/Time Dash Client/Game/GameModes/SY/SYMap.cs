using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;

public class SYMap : Map
{
	SYScrap[] scrapList = new SYScrap[500];
	SYStash[] stashList = new SYStash[10];

	public SYMap(int id, string filename)
		: base(id, filename, GameMode.ScrapYard)
	{
	}

	public void CreateScrap(int id, Vector2 position, Vector2 velocity)
	{
		if (scrapList[id] != null)
			RemoveScrap(id);

		scrapList[id] = new SYScrap(id, position, velocity, this);
	}

	public void RemoveScrap(SYScrap scrap) { RemoveScrap(scrap.id); }
	public void RemoveScrap(int id)
	{
		scrapList[id].Dispose();
		scrapList[id] = null;
	}

	public override void PlayerJoin(int id, string name)
	{
		playerList[id] = new SYPlayer(id, name, Vector2.Zero, this);
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void SceneZone(int typeID, TKTools.Polygon pos)
	{
		base.SceneZone(typeID, pos);

		RectangleF bounds = pos.Bounds;

		if (typeID == 3)
			stashList[0] = new SYStash(0, new Vector2(bounds.X + bounds.Width / 2, bounds.Y), this);
		if (typeID == 4)
			stashList[1] = new SYStash(1, new Vector2(bounds.X + bounds.Width / 2, bounds.Y), this);
	}

	public override void Logic()
	{
		base.Logic();
		foreach (SYScrap s in scrapList) if (s != null) s.Logic();
		foreach (SYStash b in stashList) if (b != null) b.Logic();
	}

	public override void Draw()
	{
		UpdateView();
		DrawBackground();
		foreach (SYScrap s in scrapList) if (s != null) s.Draw();
		foreach (SYStash b in stashList) if (b != null) b.Draw();
		DrawMap();
	}

	public override void MessageHandle(EZUDP.MessageBuffer msg)
	{
		try
		{
			if ((Protocol)msg.ReadShort() == Protocol.MapArgument)
			{
				switch ((Protocol_SY)msg.ReadShort())
				{
					case Protocol_SY.ScrapExistance:
						CreateScrap(msg.ReadShort(), msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol_SY.ScrapCollect:
						{
							SYScrap scrap = scrapList[msg.ReadShort()];
							SYPlayer player = (SYPlayer)playerList[msg.ReadByte()];

							player.CollectScrap(scrap);

							break;
						}

					case Protocol_SY.ScrapReturn:
						((SYPlayer)playerList[msg.ReadByte()]).ReturnScrap(stashList[msg.ReadByte()]);
						break;

					case Protocol_SY.StashScrapAmount:
						stashList[msg.ReadByte()].SetScrap(msg.ReadShort());
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