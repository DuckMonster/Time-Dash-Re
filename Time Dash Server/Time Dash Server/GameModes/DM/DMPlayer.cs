using EZUDP.Server;
using OpenTK;

public class DMPlayer : Player
{
	protected new DMMap Map
	{
		get
		{
			return (DMMap)base.Map;
		}
	}

	public DMPlayer(int id, string name, Client c, Vector2 pos, Map m)
		: base(id, name, c, pos, m)
	{
	}

	public override void OnKill(Player p)
	{
		base.OnKill(p);
		Map.scoreboard.ChangeScore(id, 1);
	}
}