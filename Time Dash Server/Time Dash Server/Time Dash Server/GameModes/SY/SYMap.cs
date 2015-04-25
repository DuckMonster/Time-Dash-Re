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

	SYBase[] baseList = new SYBase[2];
	public SYTower[] towerList = new SYTower[8];
	public SYTowerWall[] wallList = new SYTowerWall[8];
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

				tries--;
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

			foreach (SYTower t in towerList)
				if (t != null) yield return t;

			foreach (SYTowerWall t in wallList)
				if (t != null) yield return t;

			foreach (SYBase b in baseList)
				if (b != null) yield return b;

			foreach (Actor a in base.Actors)
				yield return a;
		}
	}

	public SYMap(string filename, Player[] players)
		: base(filename, GameMode.ScrapYard, players)
	{
		teamList[0] = new Team(0, this);
		teamList[1] = new Team(1, this);
	}

	public SYTower SpawnTower(SYTowerPoint point, int team) { return SpawnTower(point, team, NextTowerID); }
	public SYTower SpawnTower(SYTowerPoint point, int team, int id)
	{
		SYTower tower = new SYTower(id, team, point, point.Position, this);
		towerList[id] = tower;

		return tower;
	}

	public SYTower SpawnTower(Vector2 point, int team) { return SpawnTower(point, team, NextTowerID); }
    public SYTower SpawnTower(Vector2 point, int team, int id)
	{
		SYTower tower = new SYTower(id, team, point, this);
		towerList[id] = tower;

		return tower;
	}

	public int AddCreep(SYCreep c)
	{
		int id = NextEnemyIndex;
		creepList[id] = c;

		return id;
	}

	public void RemoveCreep(SYCreep e) { if (e != null) RemoveCreep(e.id); }
	public void RemoveCreep(int id)
	{
		creepList[id] = null;
	}
	
	public void TowerDestroyed(SYTower t)
	{
		towerList[t.id] = null;
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

		p.Position = GetFreeSpawnPosition(p);
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

	public override bool GetCollision(Vector2 position, Vector2 size, Entity e)
	{
		bool c = base.GetCollision(position, size, e);
		if (!c)
		{
			foreach (SYTowerWall w in wallList)
			{
				if (w == null || e == null) continue;
				if (e is Player && (e as Player).Team == w.Team) continue;

				if (w.CollidesWith(e.Position, e.Size))
					return true;
			}
		}

		return c;
	}

	public override void SceneEvent(Scene.EnvEvent e, int[] args, TKTools.Polygon p)
	{
		base.SceneEvent(e, args, p);

		RectangleF rect = p.Bounds;

		if (e.ID == 0)
			spawnPoints[args[0]] = new SpawnPoint(p.Center, new Vector2(rect.Width, rect.Height), this);

		if (e.ID == 1)
			baseList[args[0]] = new SYBase(p.Center, args[0], this);

		if (e.ID == 10)
			campList.Add(new SYCreepCamp(rect, args[0], this));

		if (e.ID == 5)
			SpawnTower(new Vector2(rect.X + rect.Width / 2, rect.Y), args[0], args[1]);

		if (e.ID == 6)
			wallList[args[0]] = new SYTowerWall(p, args[0], this);
	}

	public override void Logic()
	{
		base.Logic();
		foreach (SYScrap s in scrapList) if (s != null) s.Logic();
		foreach (SYStash s in stashList) if (s != null) s.Logic();
		foreach (SYCreepCamp c in campList) c.Logic();
	}

	public override Vector2 GetFreeSpawnPosition(Player p)
	{
		if (p.Team == null) return Vector2.Zero;

		Vector2 pos;

		do
		{
			pos = spawnPoints[p.Team.id].GetSpawnPosition();
		} while (GetCollision(pos, p.Size));

		return pos;
	}
}