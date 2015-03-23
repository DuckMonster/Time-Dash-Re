using EZUDP.Server;
using OpenTK;
public class SYPlayer : Player
{
	protected new SYMap Map
	{
		get
		{
			return (SYMap)base.Map;
		}
	}

	int scrap = 0;

	public int Scrap
	{
		get { return scrap; }
	}

	public SYPlayer(int id, string name, Client client, Vector2 position, Map m)
		: base(id, name, client, position, m)
	{
	}

	public override void Die()
	{
		base.Die();

		for (int i = 0; i < scrap; i++)
			Map.CreateScrap(position);

		scrap = 0;
	}

	public void ReturnScrap(SYStash stash)
	{
		stash.AddScrap(scrap, id);
		scrap = 0;
	}

	public void CollectScrap(SYScrap s)
	{
		s.CollectedBy(this);
		scrap++;
	}

	public override void Logic()
	{
		base.Logic();

		if (!IsAlive) return;

		foreach (SYScrap scrap in Map.scrapList)
			if (scrap != null && scrap.Grabbable)
				if (scrap.CollidesWith(position, size))
					CollectScrap(scrap);
	}
}