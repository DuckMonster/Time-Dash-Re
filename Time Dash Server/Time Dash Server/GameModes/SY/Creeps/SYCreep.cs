using EZUDP;
using OpenTK;
using TKTools;

public class SYCreep : Actor
{
	protected new SYMap Map
	{
		get { return (SYMap)base.Map; }
	}

	public int id;

	public override int MaxHealth
	{
		get
		{
			return 2;
		}
	}

	public SYCreep(int id, Vector2 position, Map map)
		: base(position, map)
	{
		this.id = id;
		SendExistanceToPlayer(Map.playerList);
	}

	public override void Hit(float dmg, float dir, Actor a, float force = 0)
	{
		base.Hit(dmg, dir, a);
		SendHitToPlayer(dmg, dir, a, Map.playerList);

		if (force != 0)
			SendPositionToPlayer(Map.playerList);
	}

	public override void Hit(float dmg, float dir, Projectile proj, float force = 0)
	{
		base.Hit(dmg, dir, proj);
		SendHitToPlayer(dmg, dir, proj, Map.playerList);

		if (force != 0)
			SendPositionToPlayer(Map.playerList);
	}

	public override void Hit(float dmg)
	{
		base.Hit(dmg);
	}

	public override void Die()
	{
		base.Die();
		SendDieToPlayer(Map.playerList);
		Map.RemoveEnemy(id);
		Map.CreateScrap(position);
	}

	public override void Logic()
	{
		//base.Logic();
		Player p = Map.GetActorAtPos<Player>(position, size);
		if (p != null) p.Hit(1f, TKMath.GetAngle(position, p.position), this, 40f);
	}

	public void SendExistanceToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetExistanceMessage(), players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), players);
	}

	public void SendHitToPlayer(float damage, float dir, Actor attacker, params Player[] players)
	{
		SendMessageToPlayer(GetHitMessage(damage, dir, attacker), players);
	}

	public void SendHitToPlayer(float damage, float dir, Projectile p, params Player[] players)
	{
		SendMessageToPlayer(GetHitMessage(damage, dir, p), players);
	}

	public void SendDieToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetDieMessage(), players);
	}

	void SendMessageToPlayer(MessageBuffer msg, params Player[] players)
	{
		foreach(Player p in players)
			if (p != null) p.client.Send(msg);
	}

	MessageBuffer GetExistanceMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.EnemyExistance);

		msg.WriteByte(id);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		return msg;
	}

	MessageBuffer GetPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.EnemyPosition);

		msg.WriteByte(id);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.EnemyHit);

		msg.WriteByte(id);
		msg.WriteFloat(dmg);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg, float dir, Actor attacker)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.EnemyHit);

		msg.WriteByte(id);
		msg.WriteFloat(dmg);
		msg.WriteFloat(dir);
		msg.WriteByte((attacker as Player).id);

		msg.WriteByte((byte)HitType.Dash);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg, float dir, Projectile proj)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.EnemyHit);

		msg.WriteByte(id);
		msg.WriteFloat(dmg);
		msg.WriteFloat(dir);
		msg.WriteByte(proj.Owner.id);

		msg.WriteByte((byte)HitType.Bullet);
		msg.WriteByte(proj.id);

		return msg;
	}

	MessageBuffer GetDieMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.EnemyDie);

		msg.WriteByte(id);
		msg.WriteVector(position);

		return msg;
	}
}