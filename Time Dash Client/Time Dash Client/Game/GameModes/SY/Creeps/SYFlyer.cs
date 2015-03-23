using OpenTK;

public class SYFlyer : SYCreep
{
	Player target;
	Stats chasingStats;

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
			if (target == null) return !idleTargetReached ? (idleTarget as Vector2?) : null;
			else return target.position;
		}
	}

	public SYFlyer(int id, Vector2 position, Vector2 velocity, Map map)
		:base(id, position, velocity, map)
	{
		stats.MaxVelocity = 2f;
		stats.AccelerationTime = 4f;
		stats.DecelerationTime = 4f;

		chasingStats = new Stats();

		chasingStats.MaxVelocity = 4f;
		chasingStats.AccelerationTime = 2f;
		chasingStats.DecelerationTime = 2f;
	}

	public override void Logic()
	{
		base.Logic();

		if (target != null)
		{
			if ((target.position - position).Length > 11f)
			{
				target = null;
			}
		}

		if (TargetPosition != null)
		{
			Vector2 dir = (TargetPosition.Value - position).Normalized();
			velocity += dir * Acceleration * Game.delta;
		}

		velocity -= velocity * Friction * Game.delta;

		if (velocity.Length > 0.001f)
		{
			if (Map.GetCollision(this, velocity * new Vector2(Game.delta, 0)) || Map.GetActorAtPos<SYFlyer>(position + velocity * new Vector2(Game.delta, 0), size, this) != null)
				velocity.X *= -0.7f;
			if (Map.GetCollision(this, velocity * new Vector2(0, Game.delta)) || Map.GetActorAtPos<SYFlyer>(position + velocity * new Vector2(0, Game.delta), size, this) != null)
				velocity.Y *= -0.7f;

			position += velocity * Game.delta;
		}

		if (TargetPosition == idleTarget && (idleTarget - position).Length <= 0.1f) ReachIdleTarget();
	}

	public void ReceiveTarget(int playerID)
	{
		Log.Write("My target is " + playerID);

		if (playerID == 255)
			target = null;
		else
		{
			target = Map.playerList[playerID];
		}
	}
}