using EZUDP;
using OpenTK;
using System;
using System.Collections.Generic;

public class SYFlyer : SYCreep
{
	Player target;
	Timer updateTimer = new Timer(1f, false);
	
	public SYFlyer(int id, Vector2 position, Map map)
		: base(id, position, map)
	{
		stats.MaxVelocity = 2f;
		stats.AccelerationTime = 4f;
		stats.DecelerationTime = 4f;
	}

	void SetTarget(Player p)
	{
		target = p;
		SendTargetToPlayer(Map.playerList);
	}

	public override void Logic()
	{
		base.Logic();

		Log.Debug(position);

		if (target == null)
		{
			List<Player> playerList = Map.GetActorRadius<Player>(position, 10f);

			if (playerList.Count > 0)
				SetTarget(playerList[new Random().Next(playerList.Count)]);
		}
		else
		{
			if ((target.position - position).Length > 11f)
			{
				target = null;
			}

			if (target != null)
			{
				Vector2 dir = (target.position - position).Normalized();
				velocity += dir * stats.Acceleration * Game.delta - velocity * stats.AccFriction * Game.delta;
			}
        }

		if (velocity.Length > 0.001f)
		{
			if (Map.GetCollision(this, velocity * new Vector2(Game.delta, 0)) || Map.GetActorAtPos<SYFlyer>(position + velocity * new Vector2(Game.delta, 0), size, this) != null)
				velocity.X *= -0.7f;
			if (Map.GetCollision(this, velocity * new Vector2(0, Game.delta)) || Map.GetActorAtPos<SYFlyer>(position + velocity * new Vector2(0, Game.delta), size, this) != null)
				velocity.Y *= -0.7f;

			position += velocity * Game.delta;

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