using EZUDP;
using OpenTK;
using System;
using System.Collections.Generic;

public class SYFlyer : SYCreep
{
	Player target;

	public SYFlyer(int id, Vector2 position, Map map)
		: base(id, position, map)
	{
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
				target = playerList[new Random().Next(playerList.Count)];
		}
		else
		{
			if ((target.position - position).Length > 11f)
			{
				target = null;
				return;
			}

			Vector2 dir = (target.position - position).Normalized();
			Vector2 velocity = dir * 10f;

			Vector2 collisionVector = new Vector2(
				Map.GetCollision(this, velocity * new Vector2(Game.delta, 0)) ? 0 : 1,
				Map.GetCollision(this, velocity * new Vector2(0, Game.delta)) ? 0 : 1
				);

			position += velocity * collisionVector * Game.delta;
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