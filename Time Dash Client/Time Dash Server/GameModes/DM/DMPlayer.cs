using EZUDP.Server;
using OpenTK;

public class DMPlayer : Player
{
	public DMPlayer(int id, string name, Client c, Vector2 pos, Map m)
		: base(id, name, c, pos, m)
	{
	}

	public override void OnKill(Player p)
	{
		base.OnKill(p);
		((DMMap)map).scoreboard.ChangeScore(id, 1);
	}
}