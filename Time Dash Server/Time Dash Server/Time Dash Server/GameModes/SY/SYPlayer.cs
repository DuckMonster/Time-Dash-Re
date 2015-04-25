using EZUDP;
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
	}

	public override void BuyWeapon(WeaponList weapon)
	{
		if (ownedWeapons.Contains(weapon) || scrap < WeaponStats.GetStats(weapon).scrapCost) return;
		base.BuyWeapon(weapon);

		SetScrap(scrap - WeaponStats.GetStats(weapon).scrapCost);
	}

	public override void KilledBy(Actor a)
	{
		if (a is SYPlayer) (a as SYPlayer).GainScrap(20);
		base.KilledBy(a);
	}

	void SetScrap(int n)
	{
		scrap = n;
		SendScrapToPlayer(Map.playerList);
	}

	public void HitByCreep()
	{
		creepHitTimer.Reset();
	}

	public override void Die()
	{
		base.Die();

		GainScrap(-scrap / 2, Position);
	}

	public void ReturnScrap(SYStash stash)
	{
		int scrapReturned = stash.AddScrap(scrap, id);
		SetScrap(scrap - scrapReturned);
	}

	public void GainScrap(int n)
	{
		SetScrap(scrap + n);
	}

	public void GainScrap(int n, Vector2 pos)
	{
		SetScrap(scrap + n);
		SendScrapGainEffectToPlayer(pos, n, this);
	}

	public void CollectScrap(SYScrap s)
	{
		s.CollectedBy(this);
		GainScrap(1, s.Position);
	}

	public override void Logic()
	{
		base.Logic();

		if (!IsAlive) return;

		creepHitTimer.Logic();

		foreach (SYScrap scrap in Map.scrapList)
			if (scrap != null && scrap.Grabbable)
				if (scrap.CollidesWith(Position, Size))
					CollectScrap(scrap);
	}

	public void SendScrapToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetScrapMessage(), false, players);
	}

	public void SendScrapGainEffectToPlayer(Vector2 pos, int n, params Player[] players)
	{
		SendMessageToPlayer(GetScrapGainEffectMessage(pos, n), false, players);
	}

	MessageBuffer GetScrapMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.PlayerScrap);

		msg.WriteByte(id);
		msg.WriteByte(scrap);

		return msg;
	}

	MessageBuffer GetScrapGainEffectMessage(Vector2 pos, int n)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.ScrapGainEffect);

		msg.WriteVector(pos);
		msg.WriteShort(n);

		return msg;
	}
}