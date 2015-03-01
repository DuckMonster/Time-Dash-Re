using OpenTK;
using System.Collections.Generic;
using System.Drawing;
public class SYMap : Map
{
	public SYScrap[] scrapList = new SYScrap[500];
	int scrapIndex = 0;

	SpawnPoint[] spawnPoints = new SpawnPoint[2];
	Timer tempTimer = new Timer(1f, false);

	SYStash[] stashList = new SYStash[10];

	public SYMap(string filename, Player[] players)
		: base(filename, GameMode.ScrapYard, players)
	{
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
				CreateScrap(playerList[0].position);
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