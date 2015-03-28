using EZUDP;
using OpenTK;
using System;
using System.Collections.Generic;

public class SYFlyer : SYCreep
{
	Stats chasingStats;

	Player target;
	Timer updateTimer = new Timer(0.1f, false);

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

	Vector2? TargetPosition
	{
		get
		{
			if (target == null) return idleTimer.IsDone ? (idleTarget as Vector2?): null;
			else return target.Position;
		}
	}
	
	public SYFlyer(int id, Vector2 position, SYCreepCamp camp, Map map)
		: base(id, position, camp, map)
	{
		stats.MaxVelocity = 2f;
		stats.AccelerationTime = 4f;
		stats.DecelerationTime = 4f;

		chasingStats = new Stats();

		chasingStats.MaxVelocity = 4f;
		chasingStats.AccelerationTime = 2f;
		chasingStats.DecelerationTime = 2f;
	}

	void SetTarget(Player p)
	{
		target = p;
		SendTargetToPlayer(Map.playerList);
	}

	public override void Logic()
	{
		base.Logic();

		if (target == null)
		{
			foreach(Player p in Map.playerList)
				if (p != null && p.CollidesWith(creepCamp.Position, creepCamp.Size))
				{
					target = p;
					break;
				}
		}
		else
			if ((target.Position - Position).Length > 11f)
				target = null;


		if (TargetPosition != null)
		{
			Vector2 dir = (TargetPosition.Value - Position).Normalized();
			velocity += dir * Acceleration * Game.delta;
		}

		velocity -= velocity * Friction * Game.delta;

		if (velocity.Length > 0.001f)
		{
			if (Map.GetCollision(this, velocity * new Vector2(Game.delta, 0)) || Map.GetActorAtPos<SYFlyer>(Position + velocity * new Vector2(Game.delta, 0), Size, this) != null)
			{
				velocity.X *= -0.7f;

				if (target == null)
					ReachIdleTarget();
			}
			if (Map.GetCollision(this, velocity * new Vector2(0, Game.delta)) || Map.GetActorAtPos<SYFlyer>(Position + velocity * new Vector2(0, Game.delta), Size, this) != null)
			{
				velocity.Y *= -0.7f;

				if (target == null)
					ReachIdleTarget();
			}

			Position += velocity * Game.delta;

			updateTimer.Logic();
			if (updateTimer.IsDone)
			{
				SendPositionToPlayer(Map.playerList);
				updateTimer.Reset();
			}
		}

		if (TargetPosition == idleTarget && (idleTarget - Position).Length <= 0.1f) ReachIdleTarget();
		if (target != null && !creepCamp.CollidesWith(Position, Size))
			SetTarget(null);
	}

	void SendMessageToPlayer(MessageBuffer msg, params Player[] players)
	{
		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}

	void SendTargetToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetTargetMessage(), players);
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
}