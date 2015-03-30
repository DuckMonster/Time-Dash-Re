using EZUDP;
using OpenTK;
using System;
using TKTools;

public class SYFlyer : SYCreep
{
	static Random rng = new Random();

	Stats chasingStats;

	Player target;
	Timer updateTimer = new Timer(0.8f, false);
	Timer shootTimer = new Timer(1.5f, false);
	Timer chargeTimer = new Timer(CreepStats.FlyerChargeTime, true);

	public override float Acceleration
	{
		get
		{
			return target == null ? stats.Acceleration : chasingStats.Acceleration;
		}
	}

	public override float Friction
	{
		get
		{
			return target == null ? stats.AccFriction : chasingStats.AccFriction;
		}
	}

	public float BulletDamage
	{
		get { return CreepStats.FlyerBulletDamage; }
	}

	public float ImpactDamage
	{
		get { return CreepStats.FlyerImpactDamage; }
	}

	Vector2? TargetPosition
	{
		get
		{
			if (target == null) return idleTimer.IsDone ? (idleTarget as Vector2?): null;
			else return target.Position;
		}
	}
	
	public SYFlyer(Vector2 position, SYCreepCamp camp, Map map)
		: base(position, camp, map)
	{
		stats.MaxVelocity = CreepStats.FlyerIdleSpeed;
		stats.AccelerationTime = 4f;
		stats.DecelerationTime = 4f;

		chasingStats = new Stats();

		chasingStats.MaxVelocity = CreepStats.FlyerChaseSpeed;
		chasingStats.AccelerationTime = 2f;
		chasingStats.DecelerationTime = 2f;
	}

	void SetTarget(Player p)
	{
		target = p;
		SendTargetToPlayer(Map.playerList);

		if (target == null) chargeTimer.IsDone = true;
	}

	void Shoot()
	{
		if (target == null) return;

		float aimDir = TKMath.GetAngle(target.Position - position);
		aimDir += ((float)rng.NextDouble() - 0.5f) * 20f;

		Vector2 targetVector = Position + TKMath.GetAngleVector(aimDir);

		Projectile p = new SlowBullet(this, Position, targetVector, BulletDamage, Map);
		SendShootToPlayer(targetVector, p, Map.playerList);

		shootTimer.Reset(1f + (float)rng.NextDouble() * 0.8f);
	}

	public override void Logic()
	{
		base.Logic();

		if (TargetPosition == idleTarget && (idleTarget - Position).Length <= 0.1f) ReachIdleTarget();
		if (target != null && (!creepCamp.CollidesWith(Position, Size) || !target.IsAlive || (target.Position - Position).Length > 11f))
			SetTarget(null);

		if (target == null)
		{
			foreach (Player p in Map.playerList)
				if (p != null && (p.Position - Position).Length < 10f && p.CollidesWith(creepCamp.Position, creepCamp.Size))
				{
					SetTarget(p);
					break;
				}
		}
		else
		{
			if (!shootTimer.IsDone)
			{
				shootTimer.Logic();
				if (shootTimer.IsDone)
				{
					chargeTimer.Reset();
					SendChargeToPlayer(Map.playerList);
				}
			}

			if (!chargeTimer.IsDone)
			{
				chargeTimer.Logic();
				if (chargeTimer.IsDone)
					Shoot();
			}
		}

		if (TargetPosition != null && chargeTimer.IsDone)
		{
			Vector2 dir = (TargetPosition.Value - Position).Normalized();
			velocity += dir * Acceleration * Game.delta;
		}

		velocity -= velocity * Friction * Game.delta;

		if (velocity.Length > 0.001f)
		{
			if (Map.GetCollision(this, velocity * new Vector2(Game.delta, 0)))
			{
				velocity.X *= -0.7f;

				if (target == null)
					ReachIdleTarget();
			}
			if (Map.GetCollision(this, velocity * new Vector2(0, Game.delta)))
			{
				velocity.Y *= -0.7f;

				if (target == null)
					ReachIdleTarget();
			}

			SYFlyer collFlyer = Map.GetActorAtPos<SYFlyer>(Position + velocity * new Vector2(Game.delta, 0), Size, this);
            if (collFlyer != null)
			{
				velocity.X *= -0.7f;

				if (target == null)
					ReachIdleTarget();

				SendPositionToPlayer(Map.playerList);
				collFlyer.SendPositionToPlayer(Map.playerList);
			}

			collFlyer = Map.GetActorAtPos<SYFlyer>(Position + velocity * new Vector2(0, Game.delta), Size, this);
            if (collFlyer != null)
			{
				velocity.Y *= -0.7f;

				if (target == null)
					ReachIdleTarget();

				SendPositionToPlayer(Map.playerList);
				collFlyer.SendPositionToPlayer(Map.playerList);
			}

			Position += velocity * Game.delta;

			updateTimer.Logic();
			if (updateTimer.IsDone)
			{
				SendPositionToPlayer(Map.playerList);
				updateTimer.Reset();
			}
		}
	}

	void SendMessageToPlayer(MessageBuffer msg, params Player[] players)
	{
		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}

	void SendTargetToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetTargetMessage(), players);
	}

	void SendShootToPlayer(Vector2 target, Projectile p, params Player[] players)
	{
		SendMessageToPlayer(GetShootMessage(target, p), players);
	}

	MessageBuffer GetTargetMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.EnemyTarget);

		msg.WriteByte(id);

		if (target == null)
			msg.WriteByte(-1);
		else
			msg.WriteByte(target.id);

		return msg;
	}

	MessageBuffer GetShootMessage(Vector2 target, Projectile p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.EnemyShoot);

		msg.WriteByte(id);
		msg.WriteVector(target);
		msg.WriteByte(p.id);

		return msg;
	}
}