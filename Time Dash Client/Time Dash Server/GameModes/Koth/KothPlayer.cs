using System;
using OpenTK;
using EZUDP.Server;

public class KothPlayer : Player
{
	public KothPlayer(int id, string name, Client c, Vector2 pos, Map m)
		: base(id, name, c, pos, m)
	{
	}

	public override void Kill(Player p)
	{
		base.Kill(p);

		((KothMap)map).point.ChangeScore(id, 2);
	}
}