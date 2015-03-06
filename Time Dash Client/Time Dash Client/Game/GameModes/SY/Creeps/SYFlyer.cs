using OpenTK;
using System;
using System.Collections.Generic;

public class SYFlyer : SYCreep
{
	Player target;

	public SYFlyer(int id, Vector2 position, Vector2 velocity, Map map)
		:base(id, position, velocity, map)
	{
	}

	public override void Logic()
	{
		base.Logic();

		if (target != null)
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

	public void ReceiveTarget(int playerID)
	{
		if (playerID == -1)
			target = null;
		else
		{
			target = Map.playerList[playerID];
		}
	}
}