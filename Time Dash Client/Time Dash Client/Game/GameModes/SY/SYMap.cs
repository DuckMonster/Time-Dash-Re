using OpenTK;
using System;
using System.Drawing;
using System.Collections.Generic;

public class SYMap : Map
{
	SYScrap[] scrapList = new SYScrap[500];
	SYStash[] stashList = new SYStash[10];
	SYCreep[] creepList = new SYCreep[50];

	SYTower[] towerList = new SYTower[4];

	int stashIndex = 0;

	public override IEnumerable<Actor> Actors
	{
		get
		{
			foreach (Actor a in base.Actors)
				yield return a;

			foreach (SYCreep c in creepList)
				yield return c;
		}
	}

	public SYMap(int id, string filename)
		: base(id, filename, GameMode.ScrapYard)
	{
	}

	public void SpawnCreep(CreepType type, int id, Vector2 position, Vector2 velocity)
	{
		if (creepList[id] != null) RemoveCreep(id);

		switch(type)
		{
			case CreepType.Scroot:
				creepList[id] = new SYScroot(id, position, velocity, this);
				break;
		}
	}

	public void RemoveCreep(SYCreep e) { if (e != null) RemoveCreep(e.id); }
	public void RemoveCreep(int id)
	{
		if (creepList[id] == null) return;

		creepList[id].Dispose();
		creepList[id] = null;
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

	public void TowerDestroyed(SYTower t)
	{
		towerList[t.id] = null;
		t.Dispose();
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

		if (typeID == 2)
			stashList[0] = new SYBase(0, new Vector2(bounds.X + bounds.Width / 2, bounds.Y), this);
		if (typeID == 3)
			stashList[1] = new SYBase(1, new Vector2(bounds.X + bounds.Width / 2, bounds.Y), this);

		if (typeID == 10)
		{
			stashList[5 + stashIndex] = new SYTowerPoint(5 + stashIndex, new Vector2(bounds.X + bounds.Width / 2, bounds.Y), this);
			stashIndex++;
		}
	}

	public override void Logic()
	{
		base.Logic();
		foreach (SYScrap s in scrapList) if (s != null) s.Logic();
		foreach (SYStash b in stashList) if (b != null) b.Logic();
		foreach (SYCreep c in creepList) if (c != null) c.Logic();
		foreach (SYTower t in towerList) if (t != null) t.Logic();
	}

	public override void Draw()
	{
		UpdateView();
		DrawBackground();
		foreach (SYScrap s in scrapList) if (s != null) s.Draw();
		foreach (SYStash b in stashList) if (b != null) b.Draw();
		foreach (SYCreep c in creepList) if (c != null) c.Draw();
		foreach (SYTower t in towerList) if (t != null) t.Draw();
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

					case Protocol_SY.CreepExistance:
						SpawnCreep((CreepType)msg.ReadByte(), msg.ReadByte(), msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol_SY.CreepPosition:
						creepList[msg.ReadByte()].ReceivePosition(msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol_SY.CreepHit:
						creepList[msg.ReadByte()].ReceiveHit(msg.ReadFloat(), msg.ReadFloat(), msg);
						break;

					case Protocol_SY.CreepDie:
						creepList[msg.ReadByte()].Die(msg.ReadVector2());
						break;

					case Protocol_SY.CreepCustom:
						creepList[msg.ReadByte()].ReceiveCustom(msg);
						break;

					case Protocol_SY.TowerExistance:
						{
							int id = msg.ReadByte();
							SYTowerPoint stash = stashList[msg.ReadByte()] as SYTowerPoint;

							towerList[id] = new SYTower(id, stash, stash.Position, this);
							break;
						}

					case Protocol_SY.TowerRotation:
						towerList[msg.ReadByte()].ReceiveRotation(msg.ReadFloat());
						break;

					case Protocol_SY.TowerTarget:
						towerList[msg.ReadByte()].ReceiveTarget(msg.ReadByte());
						break;

					case Protocol_SY.TowerShoot:
						towerList[msg.ReadByte()].ReceiveShoot(msg.ReadVector2(), msg.ReadByte());
						break;

					case Protocol_SY.TowerHit:
						towerList[msg.ReadByte()].ReceiveHit(msg.ReadFloat(), msg.ReadFloat(), msg.ReadByte(), msg.ReadVector2());
						break;

					case Protocol_SY.TowerDie:
						towerList[msg.ReadByte()].ReceiveDie();
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