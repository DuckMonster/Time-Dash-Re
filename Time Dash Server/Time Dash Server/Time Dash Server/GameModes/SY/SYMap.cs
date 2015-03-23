using OpenTK;
using System.Drawing;
using System.Collections.Generic;
using System;

public class SYMap : Map
{
	public SYScrap[] scrapList = new SYScrap[500];
	int scrapIndex = 0;

	SpawnPoint[] spawnPoints = new SpawnPoint[2];
	Timer tempTimer = new Timer(1f, false);

	SYStash[] stashList = new SYStash[10];
	SYCreep[] creepList = new SYCreep[50];
	int stashIndex = 0;
	int creepIndex = 0;

	SYTower[] towerList = new SYTower[4];
	int NextTowerID
	{
		get
		{
			for (int i = 0; i < towerList.Length; i++)
				if (towerList[i] == null) return i;

			return -1;
		}
	}

	List<SYCreepCamp> campList = new List<SYCreepCamp>();

	int NextEnemyIndex
	{
		get
		{
			int tries = creepList.Length;

			while(tries > 0)
			{
				if (creepList[creepIndex] == null) return creepIndex;
				creepIndex = (creepIndex + 1) % creepList.Length;
			}

			return -1;
		}
	}
	

	public override IEnumerable<Actor> Actors
	{
		get
		{
			foreach (SYCreep e in creepList)
				if (e != null) yield return e;

			foreach (Actor a in base.Actors)
				yield return a;
		}
	}

	public SYMap(string filename, Player[] players)
		: base(filename, GameMode.ScrapYard, players)
	{
	}

	public SYTower SpawnTower(SYTowerPoint point)
	{
		int id = NextTowerID;
		SYTower tower = new SYTower(id, point, point.position, this);
		towerList[id] = tower;

		return tower;
	}

	public SYCreep SpawnEnemy(Vector2 position, SYCreepCamp camp)
	{
		int id = NextEnemyIndex;

		SYCreep c = new SYFlyer(id, position, camp, this);

		creepList[id] = c;
		return c;
	}

	public void RemoveEnemy(SYCreep e) { if (e != null) RemoveEnemy(e.id); }
	public void RemoveEnemy(int id)
	{
		creepList[id] = null;
	}

	public void CreateScrap(Vector2 position)
	{
		while (scrapList[scrapIndex] != null) scrapIndex = (scrapIndex + 1) % scrapList.Length;

		scrapList[scrapIndex] = new SYScrap(scrapIndex, position, this);
		scrapIndex = (scrapIndex + 1) % scrapList.Length;
	}

	public void RemoveScrap(SYScrap scrap) { RemoveScrap(scrap.id); }
	public void RemoveScrap(int id)
	{
		scrapList[id] = null;
	}

	public override Player CreatePlayer(int id, string name, EZUDP.Server.Client c)
	{
		return new SYPlayer(id, name, c, Vector2.Zero, this);
	}

	public override Player PlayerJoin(EZUDP.Server.Client c, string name)
	{
		Player p = base.PlayerJoin(c, name);
		PlayerJoinTeam(p, p.id % 2);

		p.position = GetFreeSpawnPosition(p);
		p.SendPositionToPlayerForce(playerList);

		foreach (SYScrap s in scrapList)
			if (s != null) s.SendExistanceToPlayer(p);

		foreach (SYStash s in stashList)
			if (s != null) s.SendScrapAmountToPlayer(p);

		foreach (SYCreep e in creepList)
			if (e != null) e.SendExistanceToPlayer(p);

		foreach (SYTower t in towerList)
			if (t != null) t.SendExistanceToPlayer(p);

		return p;
	}

	public override void SceneZone(int typeID, TKTools.Polygon p)
	{
		base.SceneZone(typeID, p);

		RectangleF rect = p.Bounds;

		if (typeID == 0)
			spawnPoints[0] = new SpawnPoint(p.Center, new Vector2(rect.Width, rect.Height), this);
		if (typeID == 1)
			spawnPoints[1] = new SpawnPoint(p.Center, new Vector2(rect.Width, rect.Height), this);

		if (typeID == 2)
			stashList[0] = new SYBase(0, 0, new Vector2(rect.X + rect.Width / 2, rect.Y), this);
		if (typeID == 3)
			stashList[1] = new SYBase(1, 1, new Vector2(rect.X + rect.Width / 2, rect.Y), this);

		if (typeID == 5)
			campList.Add(new SYCreepCamp(rect, this));

		if (typeID == 10)
		{
			stashList[5 + stashIndex] = new SYTowerPoint(5 + stashIndex, new Vector2(rect.X + rect.Width / 2, rect.Y), this);
			stashIndex++;
		}
	}

	public override void Logic()
	{
		base.Logic();
		foreach (SYScrap s in scrapList) if (s != null) s.Logic();
		foreach (SYStash s in stashList) if (s != null) s.Logic();
		foreach (SYCreepCamp c in campList) c.Logic();
		foreach (SYTower t in towerList) if (t != null) t.Logic();

		if (playerList[0] != null)
		{
			tempTimer.Logic();
			if (tempTimer.IsDone)
			{
				//CreateScrap(playerList[0].position);
				tempTimer.Reset();
			}
		}
	}

	public override Vector2 GetFreeSpawnPosition(Player p)
	{
		if (p.team == null) return Vector2.Zero;

		Vector2 pos;

		do
		{
			pos = spawnPoints[p.team.id].GetSpawnPosition();
		} while (GetCollision(pos, p.size));

		return pos;
	}
}