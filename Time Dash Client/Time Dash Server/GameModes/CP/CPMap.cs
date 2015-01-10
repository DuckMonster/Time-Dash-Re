using EZUDP.Server;
using OpenTK;
using System.Collections.Generic;
public class CPMap : Map
{
	List<CPPoint> pointList = new List<CPPoint>();
	List<Vector2> spawnPoint = new List<Vector2>();

	public CPMap(string filename)
		: base(filename, GameMode.ControlPoints)
	{
	}

	public CPMap(string filename, Player[] players)
		: base(filename, GameMode.ControlPoints, players)
	{
	}

	public override Player PlayerJoin(Client c, string name)
	{
		Player p = base.PlayerJoin(c, name);

		PlayerJoinTeam(p, p.id % 2);
		foreach (CPPoint point in pointList)
		{
			point.SendOwnerToPlayer(p);
			point.SendProgressToPlayer(p);
		}

		p.position = GetFreeSpawnPosition(p);
		p.SendPositionToPlayerForce(playerList);

		return p;
	}

	public override void MapObjectLoad(uint color, Environment.Tile t)
	{
		base.MapObjectLoad(color, t);

		switch (color)
		{
			case 0xFFFF0000:
				pointList.Add(new CPPoint(pointList.Count, t.World + new Vector2(Environment.TILE_SIZE / 2, 0), this));
				break;

			case 0xFF00FF00:
				spawnPoint.Add(t.World + new Vector2(0, 1));
				break;
		}
	}

	public override Vector2 GetFreeSpawnPosition(Player p)
	{
		if (p.team == null) return base.GetFreeSpawnPosition(p);

		Vector2 pos;

		double x = (rng.NextDouble() * 2) - 1, y = rng.NextDouble();
		pos = spawnPoint[p.team.id] + new Vector2(2f * (float)x, 0);

		return pos;
	}

	public override void Logic()
	{
		base.Logic();
		foreach (CPPoint p in pointList) p.Logic();
	}
}