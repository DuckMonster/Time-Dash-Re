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

		p.Position = GetFreeSpawnPosition(p);
		p.SendPositionToPlayerForce(playerList);

		return p;
	}

	public override Vector2 GetFreeSpawnPosition(Player p)
	{
		if (p.Team == null) return base.GetFreeSpawnPosition(p);

		Vector2 pos;

		double x = (rng.NextDouble() * 2) - 1, y = rng.NextDouble();
		pos = spawnPoint[p.Team.id] + new Vector2(2f * (float)x, 0);

		return pos;
	}

	public override void Logic()
	{
		base.Logic();
		foreach (CPPoint p in pointList) p.Logic();
	}
}