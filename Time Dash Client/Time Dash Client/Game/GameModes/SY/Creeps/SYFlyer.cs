using OpenTK;
using TKTools;

public class SYFlyer : SYCreep
{
	static Texture texture;

	Player target;
	Stats chasingStats;
	Timer chargeTimer = new Timer(0.6f, true);

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

	public override float MaxHealth
	{
		get
		{
			return 1.5f;
		}
	}

	Vector2? TargetPosition
	{
		get
		{
			if (target == null) return !idleTargetReached ? (idleTarget as Vector2?) : null;
			else return target.Position;
		}
	}

	public SYFlyer(int id, Vector2 position, Vector2 velocity, Map map)
		:base(id, position, velocity, map)
	{
		if (texture == null)
			texture = new Texture("Res/creep.png");

		stats.MaxVelocity = 2f;
		stats.AccelerationTime = 4f;
		stats.DecelerationTime = 4f;

		chasingStats = new Stats();

		chasingStats.MaxVelocity = 4f;
		chasingStats.AccelerationTime = 2f;
		chasingStats.DecelerationTime = 2f;

		mesh.Texture = texture;
	}

	public override void Logic()
	{
		base.Logic();

		if (TargetPosition != null && chargeTimer.IsDone)
		{
			Vector2 dir = (TargetPosition.Value - position).Normalized();
			velocity += dir * Acceleration * Game.delta;
		}

		chargeTimer.Logic();

		velocity -= velocity * Friction * Game.delta;

		if (velocity.Length > 0.001f)
		{
			if (Map.GetCollision(this, velocity * new Vector2(Game.delta, 0)) || Map.GetActorAtPos<SYFlyer>(position + velocity * new Vector2(Game.delta, 0), size, this) != null)
			{
				velocity.X *= -0.7f;
			}
			if (Map.GetCollision(this, velocity * new Vector2(0, Game.delta)) || Map.GetActorAtPos<SYFlyer>(position + velocity * new Vector2(0, Game.delta), size, this) != null)
			{
				velocity.Y *= -0.7f;
			}

			position += velocity * Game.delta;
		}
	}

	public void ReceiveTarget(int playerID)
	{
		if (playerID == 255)
		{
			target = null;
			chargeTimer.IsDone = true;
		}
		else
		{
			target = Map.playerList[playerID];
		}
	}

	public override void ReceiveShoot(Vector2 target, int projID)
	{
		new SlowBullet(this, projID, Position, target, Map);
	}

	public override void ReceiveCharge()
	{
		chargeTimer.Reset();
	}

	public override void Draw()
	{
		mesh.Reset();

		mesh.Translate(Position);
		mesh.Scale(Size);

		if (TargetPosition != null && TargetPosition.Value.X < Position.X)
			mesh.Scale(-1, 1);

		mesh.Rotate(velocity.Y * 6f);

		mesh.Draw();
	}
}