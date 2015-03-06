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
	int creepIndex = 0;

	int NextEnemyIndex
	{
		get
		{
			int tries = creepList.Length;

			while(tries > 0)
			{
				if (creepList[creepIndex] == null) return creepIndex;
				creepIndex++;
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
		for (int i = 0; i < 1; i++)
			SpawnEnemyTemp();
	}

	Random rng2 = new Random();

	public void SpawnEnemyTemp()
	{
		Vector2 pos;

		do
		{
			double x = rng.NextDouble() - 0.5, y = rng.NextDouble() - 0.5;
			pos = new Vector2((float)x * scene.Width, (float)y * scene.Height);
		} while (GetCollision(pos, new Vector2(1, 1)));

		SpawnEnemy(pos);
	}

	public void SpawnEnemy(Vector2 position)
	{
		int id = NextEnemyIndex;
		creepList[id] = new SYFlyer(id, position, this);
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

		return p;
	}

	public override void SceneZone(int typeID, TKTools.Polygon p)
	{
		base.SceneZone(typeID, p);

		RectangleF rect = p.Bounds;

		if (typeID == 1)
			spawnPoints[0] = new SpawnPoint(p.Center, new Vector2(rect.Width, rect.Height), this);
		if (typeID == 2)
			spawnPoints[1] = new SpawnPoint(p.Center, new Vector2(rect.Width, rect.Height), this);

		if (typeID == 3)
			stashList[0] = new SYBase(0, 0, new Vector2(rect.X + rect.Width / 2, rect.Y), this);
		if (typeID == 4)
			stashList[1] = new SYBase(1, 1, new Vector2(rect.X + rect.Width / 2, rect.Y), this);
	}

	public override void Logic()
	{
		base.Logic();
		foreach (SYScrap s in scrapList) if (s != null) s.Logic();
		foreach (SYStash s in stashList) if (s != null) s.Logic();

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