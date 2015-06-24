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

	/*
	public override void SceneEvent(Env, TKTools.Polygon pos)
	{
		base.SceneEvent(typeID, pos);

		if (typeID == 1)
		{
			scoreboard = new Scoreboard(20, new Vector2(pos.Bounds.X + pos.Bounds.Width / 2, pos.Bounds.Y), this);
		}
	}*/

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
			double x = rng.NextDouble() - 0.5, y = rng.NextDouble() - 0.5;
			pos = new Vector2((float)x * scene.Width, (float)y * scene.Height) + scene.originOffset;

			alone = true;

			foreach (Player p in playerList)
			{
				if (p == null || p == e) continue;

				if ((p.Position - pos).Length <= 10f)
				{
					alone = false;
					break;
				}
			}
		} while (GetCollision(pos, e.Size) || !alone);

		return pos;
	}
}