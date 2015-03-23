using OpenTK;
using OpenTK.Input;
using System;
using TKTools;

public class Actor : Entity
{
	protected Stats stats = Stats.defaultStats;
	public Stats Stats
	{
		get { return stats; }
	}

	public Vector2 velocity = Vector2.Zero;
	protected float currentAcceleration = 0;
	public int dir = 1;
	bool previousOnGround = false;

	public float health;

	public float MaxHealth
	{
		get
		{
			return stats.PlayerHealth;
		}
	}

	public Actor(Vector2 position, Map m)
		: base(position, m)
	{
		stats = new Stats();
		health = MaxHealth;
	}

	public override bool CollidesWith(Vector2 pos, Vector2 s)
	{
		if (!IsAlive) return false;
		return base.CollidesWith(pos, s);
	}
	public override bool CollidesWith(Vector2 pos, float radius)
	{
		if (!IsAlive) return false;
		return base.CollidesWith(pos, radius);
	}

	public virtual float Acceleration
	{
		get
		{
			return IsOnGround ? stats.Acceleration : stats.AccelerationAir;
		}
	}

	public virtual float Friction
	{
		get
		{
			return currentAcceleration == 0 ? 
				(IsOnGround ? stats.DecFriction : stats.DecFrictionAir) : 
				(IsOnGround ? stats.AccFriction : stats.AccFrictionAir);
		}
	}

	public bool IsOnGround
	{
		get
		{
			return Map.GetCollision(this, new Vector2(0, -0.1f));
		}
	}

	public bool IsAlive
	{
		get
		{
			return health > 0;
		}
		set
		{
			if (!value) health = 0;
		}
	}

	public virtual void Hit(float dmg)
	{
		health -= dmg;
	}

	public virtual void Die(Vector2 diePos)
	{
		health = 0;
	}

	public virtual void Respawn(Vector2 pos)
	{
		position = pos;
		health = MaxHealth;
	}

	public override void Logic()
	{
		if (!IsAlive) return;

		DoPhysics();

		if (Map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
		{
			Vector2 ground;
			Map.RayTraceCollision(position, position + new Vector2(0, velocity.Y * Game.delta), size, out ground);

			velocity.Y = 0;
			position = ground;
		}

		if (Map.GetCollision(this, new Vector2(velocity.X, Math.Min(0, velocity.Y)) * Game.delta))
		{
			float stepFactor = stats.StepSize * velocity.Length * Game.delta;

			//Stepping
			if (!Map.GetCollision(this, new Vector2(velocity.X * Game.delta, stepFactor)))
			{
				int accuracy = 32;
				float testStep = stepFactor / accuracy;
				float currentStep = 0;

				for (int i = 0; i < accuracy; i++)
				{
					if (Map.GetCollision(this, new Vector2(velocity.X * Game.delta, currentStep)))
						currentStep += testStep;
					else
					{
						break;
					}
				}

				position.Y += currentStep;

				if (velocity.Y < 0)
					velocity.Y = 0;
			}
		}

		if (Map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
		{
			velocity.X = 0;
		}

		if (Map.GetCollision(this, velocity * Game.delta))
		{
			velocity = Vector2.Zero;
		}

		position += velocity * Game.delta;
		if (IsOnGround && !previousOnGround) Land();

		if (velocity.X > 0) dir = 1;
		if (velocity.X < 0) dir = -1;

		previousOnGround = IsOnGround;
	}

	public virtual void DoPhysics()
	{
		velocity.X += currentAcceleration * Game.delta - velocity.X * Friction * Game.delta;
		currentAcceleration = 0;

		velocity.Y -= stats.Gravity * Game.delta;
	}

	public virtual void Land()
	{
	}

	public virtual void Jump()
	{
		velocity.Y = stats.JumpForce;
	}

	public virtual void JumpHold()
	{
		if (velocity.Y >= stats.JumpAddLimit) velocity.Y += stats.JumpAddForce * Game.delta;
	}

	public override void Draw()
	{
		if (!IsAlive) return;

		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(position);

		mesh.Draw();
	}
}