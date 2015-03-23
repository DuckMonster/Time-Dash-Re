using EZUDP;
using OpenTK;
using System.Collections.Generic;

public abstract class SYStash : Entity
{
	protected new SYMap Map
	{
		get { return (SYMap)base.Map; }
	}

	public int id;

	float areaSize = 5f;

	float[] progress = new float[20];
	float progressTime = 2f;

	protected int scrap = 0;
	protected int targetScrap = 50;

	public SYStash(int id, float size, int target, Vector2 position, Map map)
		: base(position, map)
	{
		this.id = id;

		this.areaSize = size;
		this.targetScrap = target;
	}

	public abstract void Finish();

	public void AddScrap(int nmbr, int id)
	{
		scrap += nmbr;
		progress[id] = 0;

		SendReturnScrap(id);

		if (scrap >= targetScrap)
		{
			scrap = targetScrap;
			Finish();
		}
	}

	void Hold(int id)
	{
		progress[id] += (1f / progressTime) * Game.delta;
		if (progress[id] >= 1f)
			((SYPlayer)Map.playerList[id]).ReturnScrap(this);
	}

	public override void Logic()
	{
		List<Player> playerList = Map.GetActorRadius<Player>(position, areaSize/2);

		foreach (Player p in playerList)
			if (p != null)
			{
				if (p.position.Y >= position.Y && ((SYPlayer)p).Scrap > 0 && (p.position - position).Length <= areaSize / 2)
					Hold(p.id);
				else
					progress[p.id] = 0;
			}
	}

	public void SendReturnScrap(int playerID)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.ScrapReturn);

		msg.WriteByte(playerID);
		msg.WriteByte(id);

		Map.SendToAllPlayers(msg);
	}

	public void SendScrapAmountToPlayer(params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.StashScrapAmount);

		msg.WriteByte(id);
		msg.WriteShort(scrap);

		foreach (Player p in players)
			if (p != null) p.client.Send(msg);
	}
}