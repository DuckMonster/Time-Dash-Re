using EZUDP;
using OpenTK;
public class SYBase : Actor
{
	protected new SYMap Map
	{
		get
		{
			return base.Map as SYMap;
		}
	}

	int teamID;
	Timer dieTimer = new Timer(5f, false);

	public Team Team
	{
		get
		{
			return Map.teamList[teamID];
		}
	}

	public override float MaxHealth
	{
		get
		{
			return 2f;
		}
	}

	public SYBase(Vector2 position, int teamID, Map map)
		:base(position, map)
	{
		this.teamID = teamID;
		Size = new Vector2(4, 4);
	}

	public override bool CollidesWith(Vector2 pos, float radius)
	{
		return (pos - Position).Length <= (radius + size.X / 2);
	}

	public override void Hit(float dmg, float dir, Projectile proj, float force = 0)
	{
		base.Hit(dmg, dir, proj, force);
		SendHitToPlayer(dmg, dir, proj, Map.playerList);
	}

	public override void Die()
	{
		//Map.TeamWin(Team);
		SendDieToPlayer(Map.playerList);
		base.Die();
	}

	public override void Logic()
	{
		if (!IsAlive)
		{
			dieTimer.Logic();
			if (dieTimer.IsDone)
				Map.TeamWin(Team);
		}
	}

	public void SendHitToPlayer(float dmg, float dir, Projectile p, params Player[] player)
	{
		SendMessageToPlayer(GetHitMessage(dmg, dir, p), player);
	}

	public void SendDieToPlayer(params Player[] player)
	{
		SendMessageToPlayer(GetDieMessage(), player);
	}

	void SendMessageToPlayer(MessageBuffer msg, params Player[] player)
	{
		foreach (Player p in player) if (p != null) p.client.Send(msg);
	}

	MessageBuffer GetHitMessage(float dmg, float dir, Projectile proj)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.BaseHit);

		msg.WriteByte(teamID);
		msg.WriteFloat(dmg);
		msg.WriteFloat(dir);
		msg.WriteByte(proj.id);
		msg.WriteVector(proj.Position);

		return msg;
	}

	MessageBuffer GetDieMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.BaseDie);

		msg.WriteByte(teamID);

		return msg;
	}
}