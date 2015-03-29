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

	Timer creepHitTimer = new Timer(0.4f, true);

	public int Scrap
	{
		get { return scrap; }
	}

	public bool VulnerableToCreep
	{
		get { return creepHitTimer.IsDone; }
	}

	public SYPlayer(int id, string name, Client client, Vector2 position, Map m)
		: base(id, name, client, position, m)
	{
		scrap = 1;
	}

	public void HitByCreep()
	{
		creepHitTimer.Reset();
	}

	public override void Die()
	{
		base.Die();

		for (int i = 0; i < scrap; i++)
			Map.CreateScrap(Position);

		scrap = 0;
	}

	public void ReturnScrap(SYStash stash)
	{
		int scrapReturned = stash.AddScrap(scrap, id);
		scrap -= scrapReturned;
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
				if (scrap.CollidesWith(Position, Size))
					CollectScrap(scrap);
	}
}