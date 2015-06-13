using EZUDP;
using OpenTK;
using TKTools;

public class SYScroot : SYCreep
{
	Texture texture;

	Stats idleStats = Stats.defaultStats;

	Vector2? idleTarget;
	Player target;

	Timer chargeTimer;

	Vector2? TargetPosition
	{
		get
		{
			if (!chargeTimer.IsDone) return null;

			if (target != null)
				return target.Position;
			else if (idleTarget != null)
				return idleTarget.Value;
			else
				return null;
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

	public SYScroot(int id, Vector2 position, Vector2 velocity, Map map)
		:base(id, position, velocity, CreepStats.Scroot, map)
	{
		stats.MaxVelocity = GetStat<float>("chaseSpeed");
		stats.AccelerationTime = GetStat<float>("accelerationTime");
		stats.DecelerationTime = GetStat<float>("accelerationTime");

		idleStats.MaxVelocity = GetStat<float>("idleSpeed");
		idleStats.AccelerationTime = GetStat<float>("accelerationTime");
		idleStats.DecelerationTime = GetStat<float>("accelerationTime");

		chargeTimer = new Timer(GetStat<float>("chargeTime"), true);
		
		texture = Art.Load("Res/creep.png");

		sprite.Texture = texture;
	}

	public override void ReceiveCustom(MessageBuffer msg)
	{
		switch((CreepProtocol.Scroot)msg.ReadByte())
		{
			case CreepProtocol.Scroot.IdlePosition:
				idleTarget = msg.ReadVector2();
				break;

			case CreepProtocol.Scroot.IdleReached:
				idleTarget = null;
				break;

			case CreepProtocol.Scroot.Target:
				if (msg.ReadByte() == 1)
					target = Map.playerList[msg.ReadByte()];
				else
				{
					target = null;
					chargeTimer.IsDone = true;
				}

				break;

			case CreepProtocol.Scroot.Charge:
				chargeTimer.Reset();
				break;

			case CreepProtocol.Scroot.Shoot:
				Shoot(msg.ReadVector2(), msg.ReadByte());
				break;
		}
	}

	void Shoot(Vector2 target, int projID)
	{
		new SlowBullet(this, projID, Position, target, Map);
		Map.AddEffect(new EffectRing(Position, 2f, 0.5f, Color.White, Map));
	}

	public override void Logic()
	{
		base.Logic();

		if (TargetPosition != null)
			velocity += (TargetPosition.Value - Position).Normalized() * Acceleration * Game.delta;

		velocity -= velocity * Friction * Game.delta;

		position += velocity * Game.delta;

		chargeTimer.Logic();
	}

	public override void Draw()
	{
		sprite.FillColor = false;
		sprite.Draw(Position, Size, velocity.Y * 6f);

		if (!chargeTimer.IsDone)
		{
			sprite.FillColor = true;
			sprite.Color = new Color(1, 1, 1, chargeTimer.PercentageDone);

			sprite.Draw();
		}
	}
}