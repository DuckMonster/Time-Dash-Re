using EZUDP.Server;
using OpenTK;

public class DMPlayer : Player
{
	public DMPlayer(int id, string name, Client c, Vector2 pos, Map m)
		: base(id, name, c, pos, m)
	{
	}

	public override void Kill(Player p)
	{
		base.Kill(p);
		((DMMap)map).scoreboard.ChangeScore(id, 1);
	}
}