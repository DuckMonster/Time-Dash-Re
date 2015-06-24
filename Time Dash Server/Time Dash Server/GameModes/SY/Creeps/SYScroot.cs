using EZUDP;
using OpenTK;
using System;
using TKTools;
using TKTools.Mathematics;

public class SYScroot : SYCreep
{
	static Random rng = new Random();

	Stats idleStats = Stats.defaultStats;

	Vector2 idleTarget;
	Timer idleTimer = new Timer(2f, false);

	Player target;

	Timer reloadTimer;
	Timer chargeTimer;

	Timer updateTimer = new Timer(0.1f, false);

	Vector2? TargetPosition
	{
		get
		{
			if (!chargeTimer.IsDone) return null;

			if (target == null)
				return idleTimer.IsDone ? idleTarget as Vector2? : null;
			else
				return target.Position;
		}
	}

	public override float Acceleration
	{
		get { return target == null ? idleStats.Acceleration : stats.Acceleration; }
	}

	public override float Friction
	{
		get { return target == null ? idleStats.AccFriction : stats.AccFriction; }
	}

	float MinReloadTime
	{
		get { return GetStat<float>("reloadMin"); }
	}

	float MaxReloadTime
	{
		get { return GetStat<float>("reloadMax"); }
	}

	float ChargeTime
	{
		get { return GetStat<float>("chargeTime"); }
	}

	float ProjectileDamage
	{
		get { return GetStat<float>("projectileDamage"); }
	}

	float ReloadTime
	{
		get
		{
			return MinReloadTime + (float)rng.NextDouble() * (MaxReloadTime - MinReloadTime);
		}
	}

	float Recoil { get { return GetStat<float>("recoil"); } }

	public SYScroot(Vector2 position, SYCreepCamp camp, Map map)
		: base(position, camp, CreepStats.Scroot, map)
	{
		stats.MaxVelocity = GetStat<float>("chaseSpeed");
		stats.AccelerationTime = GetStat<float>("accelerationTime");
		stats.DecelerationTime = GetStat<float>("accelerationTime");

		idleStats.MaxVelocity = GetStat<float>("idleSpeed");
		idleStats.AccelerationTime = GetStat<float>("accelerationTime");
		idleStats.DecelerationTime = GetStat<float>("accelerationTime");

		reloadTimer = new Timer(ReloadTime, false);
		chargeTimer = new Timer(ChargeTime, true);
	}

	void SetIdleTarget(Vector2 target)
	{
		idleTarget = target;
		SendIdleTargetToPlayer(Map.playerList);
		SendPositionToPlayer(Map.playerList);
	}

	void IdleTargetReached()
	{
		idleTimer.Reset(2f + (float)rng.NextDouble() * 4f);
		SendIdleReachedToPlayer(Map.playerList);
		SendPositionToPlayer(Map.playerList);
	}

	void SetTarget(Player target)
	{
		if (target == null)
		{
			chargeTimer.IsDone = true;
		}
		this.target = target;
		SendTargetToPlayer(Map.playerList);
	}

	void Shoot()
	{
		float dir = TKMath.GetAngle(Position, target.Position) + (float)(rng.NextDouble() - 0.5f) * 20f;
		Vector2 targetPos = Position + TKMath.GetAngleVector(dir);

		Projectile p = new SlowBullet(this, Position, targetPos, ProjectileDamage, Map);

		velocity = (targetPos - Position) * -1 * Recoil;
		SendPositionToPlayer(Map.playerList);
		SendShootToPlayer(targetPos, p, Map.playerList);
	}

	public override void Logic()
	{
		base.Logic();

		if (TargetPosition != null)
			velocity += (TargetPosition.Value - Position).Normalized() * Acceleration * Game.delta;

		velocity -= velocity * Friction * Game.delta;

		updateTimer.Logic();
		if (updateTimer.IsDone)
		{
			//SendPositionToPlayer(Map.playerList);
			updateTimer.Reset();
		}

		//Move
		DoMovement();

		//Search for player
		if (target == null)
		{
			foreach (SYPlayer p in Map.playerList)
				if (p != null && p.CollidesWith(creepCamp.Position, creepCamp.Size))
				{
					SetTarget(p);
					break;
				}

			if (target == null) //It's still null, do idle stuff
			{
				if (!idleTimer.IsDone)
				{
					idleTimer.Logic();

					if (idleTimer.IsDone)
						SetIdleTarget(creepCamp.RandomPosition);
				} else
				{
					if ((Position - idleTarget).Length <= 0.1f)
						IdleTargetReached();
				}
			}
		} else
		{
			if (!target.IsAlive || !target.CollidesWith(creepCamp.Position, creepCamp.Size))
			{
				SetTarget(null);
			}
			else
			{
				if (chargeTimer.IsDone)
				{
					reloadTimer.Logic();

					if (reloadTimer.IsDone)
					{
						SendChargeToPlayer(Map.playerList);
						chargeTimer.Reset();
						reloadTimer.Reset(ReloadTime);
					}
				}
				else
				{
					chargeTimer.Logic();
					if (chargeTimer.IsDone)
						Shoot();
				}
			}
		}
	}

	void DoMovement()
	{
		Vector2 colPos = Position + velocity * new Vector2(Game.delta, 0);

		SYCreep coll = Map.GetActorAtPos<SYCreep>(Position + velocity * new Vector2(Game.delta, 0), Size, this);
		if (coll != null || Map.GetCollision(this, velocity * new Vector2(Game.delta, 0)))
		{
			if (target == null)
				IdleTargetReached();

			velocity.X *= -0.7f;

			SendPositionToPlayer(Map.playerList);
			if (coll != null)
				coll.SendPositionToPlayer(Map.playerList);
		}

		colPos = Position + velocity * new Vector2(0, Game.delta);

		coll = Map.GetActorAtPos<SYCreep>(Position + velocity * new Vector2(0, Game.delta), Size, this);
		if (coll != null || Map.GetCollision(this, velocity * new Vector2(0, Game.delta)))
		{
			if (target == null)
				IdleTargetReached();

			velocity.Y *= -0.7f;

			SendPositionToPlayer(Map.playerList);
			if (coll != null)
				coll.SendPositionToPlayer(Map.playerList);
		}

		position += velocity * Game.delta;
	}

	public void SendIdleTargetToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetIdlePositionMessage(), players);
	}

	public void SendIdleReachedToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetIdleReachedMessage(), players);
	}

	public void SendTargetToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetTargetMessage(), players);
	}

	public void SendChargeToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetChargeMessage(), players);
	}

	public void SendShootToPlayer(Vector2 target, Projectile p, params Player[] players)
	{
		SendMessageToPlayer(GetShootMessage(target, p), players);
	}

	MessageBuffer GetIdlePositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepCustom);

		msg.WriteByte(id);

		msg.WriteByte((byte)CreepProtocol.Scroot.IdlePosition);
		msg.WriteVector(idleTarget);

		return msg;
	}

	MessageBuffer GetIdleReachedMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepCustom);

		msg.WriteByte(id);

		msg.WriteByte((byte)CreepProtocol.Scroot.IdleReached);

		return msg;
	}

	MessageBuffer GetTargetMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepCustom);

		msg.WriteByte(id);

		msg.WriteByte((byte)CreepProtocol.Scroot.Target);

		msg.WriteByte(target == null ? 0 : 1);
		if (target != null)
			msg.WriteByte(target.id);

		return msg;
	}

	MessageBuffer GetChargeMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepCustom);

		msg.WriteByte(id);

		msg.WriteByte((byte)CreepProtocol.Scroot.Charge);

		return msg;
	}

	MessageBuffer GetShootMessage(Vector2 target, Projectile p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.CreepCustom);

		msg.WriteByte(id);

		msg.WriteByte((byte)CreepProtocol.Scroot.Shoot);

		msg.WriteVector(target);
		msg.WriteByte(p.id);

		return msg;
	}
}