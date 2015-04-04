using EZUDP;
using OpenTK;
using System;
using TKTools;

public class SYCreep : Actor
{
	protected T GetStat<T>(string statName)
	{
		return creepStats.GetStat<T>(statName);
	}

	protected new SYMap Map
	{
		get { return (SYMap)base.Map; }
	}

	protected virtual CreepType Type
	{
		get { return GetStat<CreepType>("type"); }
	}

	public int id;
	protected SYCreepCamp creepCamp;
	CustomStats creepStats;

	public override float MaxHealth
	{
		get
		{
			if (creepStats != null)
				return GetStat<float>("health");
			else
				return base.MaxHealth;
		}
	}

	public float ImpactDamage
	{
		get { return GetStat<float>("impactDamage"); }
	}

	public float ImpactForce
	{
		get { return GetStat<float>("impactForce"); }
	}

	public SYCreep(Vector2 position, SYCreepCamp camp, CustomStats stats, Map map)
		: base(position, map)
	{
		id = Map.AddCreep(this);

		creepCamp = camp;
		creepStats = stats;

		health = MaxHealth; //Required because called virtual method in constructor :(

		SendExistanceToPlayer(Map.playerList);
	}

	public override void Hit(float dmg, float dir, Actor a, float force = 0)
	{
		SendHitToPlayer(dmg, dir, Map.playerList);
		base.Hit(dmg, dir, a);

		if (force != 0)
			SendPositionToPlayer(Map.playerList);
	}

	public override void Hit(float dmg, float dir, Projectile proj, float force = 0)
	{
		SendHitToPlayer(dmg, dir, proj, Map.playerList);
		base.Hit(dmg, dir, proj);

		if (force != 0)
			SendPositionToPlayer(Map.playerList);
	}

	public override void Die()
	{
		base.Die();
		SendDieToPlayer(Map.playerList);
		Map.RemoveCreep(id);
		Map.CreateScrap(Position);
	}

	public override void Logic()
	{
		//base.Logic();
		SYPlayer p = Map.GetActorAtPos<SYPlayer>(Position, Size);
		if (p != null && p.VulnerableToCreep)
		{
			p.Hit(ImpactDamage, TKMath.GetAngle(Position, p.Position), this, ImpactForce);
			p.HitByCreep();
		}
	}

	public void SendExistanceToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetExistanceMessage(), players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), players);
	}

	public void SendHitToPlayer(float damage, float dir, params Player[] players)
	{
		SendMessageToPlayer(GetHitMessage(damage, dir), players);
	}

	public void SendHitToPlayer(float damage, float dir, Projectile p, params Player[] players)
	{
		SendMessageToPlayer(GetHitMessage(damage, dir, p), players);
	}

	public void SendDieToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetDieMessage(), players);
	}


	protected void SendMessageToPlayer(MessageBuffer msg, params Player[] players)
	{
		foreach(Player p in players)
			if (p != null) p.client.Send(msg);
	}

	MessageBuffer GetExistanceMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepExistance);

		msg.WriteByte((byte)Type);
		msg.WriteByte(id);
		msg.WriteVector(Position);
		msg.WriteVector(velocity);

		return msg;
	}

	MessageBuffer GetPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepPosition);

		msg.WriteByte(id);
		msg.WriteVector(Position);
		msg.WriteVector(velocity);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepHit);

		msg.WriteByte(id);
		msg.WriteFloat(dmg);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg, float dir)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepHit);

		msg.WriteByte(id);

		msg.WriteFloat(dmg);
		msg.WriteFloat(dir);

		msg.WriteByte((byte)HitType.Dash);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg, float dir, Projectile proj)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepHit);

		msg.WriteByte(id);

		msg.WriteFloat(dmg);
		msg.WriteFloat(dir);

		msg.WriteByte((byte)HitType.Projectile);
		msg.WriteByte(proj.id);
		msg.WriteVector(proj.Position);

		return msg;
	}

	MessageBuffer GetDieMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepDie);

		msg.WriteByte(id);
		msg.WriteVector(Position);

		return msg;
	}
}