using OpenTK;

public class DMMap : Map
{
	public Scoreboard scoreboard;

	public DMMap(string filename)
		: base(filename, GameMode.DeathMatch)
	{
	}

	public DMMap(string filename, Player[] players)
		: base(filename, GameMode.DeathMatch, players)
	{
	}

	public override void MapObjectLoad(uint color, Environment.Tile t)
	{
		base.MapObjectLoad(color, t);

		switch (color)
		{
			case 0xFFFF0000:
				scoreboard = new Scoreboard(15, t.World + new Vector2(Environment.TILE_SIZE / 2, 0), this);
				break;
		}
	}

	public override Player CreatePlayer(int id, string name, EZUDP.Server.Client c)
	{
		return new DMPlayer(id, name, c, Vector2.Zero, this);
	}

	public override Player PlayerJoin(EZUDP.Server.Client c, string name)
	{
		Player p = base.PlayerJoin(c, name);
		scoreboard.SendScoreboardToPlayer(p);

		return p;
	}

	public override Vector2 GetFreeSpawnPosition(Player e)
	{
		Vector2 pos;
		bool alone = true;

		do
		{
			double x = rng.NextDouble(), y = rng.NextDouble();
			pos = new Vector2((float)x * environment.Width, (float)y * environment.Height);

			alone = true;

			foreach (Player p in playerList)
			{
				if (p == null || p == e) continue;

				if ((p.position - pos).Length <= 10f)
				{
					alone = false;
					break;
				}
			}
		} while (GetCollision(pos, e.size) || !alone);

		return pos;
	}
}