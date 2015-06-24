using System;
using OpenTK;
using EZUDP.Server;

public class KothPlayer : Player
{
	protected new KothMap Map
	{
		get
		{
			return (KothMap)base.Map;
		}
	}

	public KothPlayer(int id, string name, Client c, Vector2 pos, Map m)
		: base(id, name, c, pos, m)
	{
	}

	public void Kill(Player p)
	{
		Map.point.ChangeScore(id, 2);
	}
}