using OpenTK;

public class DMMap : Map
{
	public Scoreboard scoreboard;

	public DMMap(string filename)
		: base(filename, GameMode.DeathMatch)
	{
		scoreboard = new Scoreboard(20, new Vector2(5, 5), this);
	}

	public DMMap(string filename, Player[] players)
		: base(filename, GameMode.DeathMatch, players)
	{
		scoreboard = new Scoreboard(20, new Vector2(5, 5), this);
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
		return new Vector2(0, 10);

		Vector2 pos;
		bool alone = true;

		do
		{
			double x = rng.NextDouble(), y = rng.NextDouble();
			pos = new Vector2((float)x * scene.Width, (float)y * scene.Height);

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