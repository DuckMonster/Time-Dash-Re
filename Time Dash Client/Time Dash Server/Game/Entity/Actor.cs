using OpenTK;
using OpenTK.Input;
using System;

public class Actor : Entity
{
	protected Stats stats = Stats.defaultStats;

	protected Vector2 velocity = Vector2.Zero;
	protected float currentAcceleration = 0;
	protected int dir = 1;

	public int health;

	public float Acceleration
	{
		get
		{
			return IsOnGround ? stats.Acceleration : stats.AccelerationAir;
		}
	}

	public float Friction
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
			return map.GetCollision(this, new Vector2(0, -0.1f));
		}
	}

	public bool IsAlive
	{
		get
		{
			return health > 0;
		}
	}

	public Actor(Vector2 position, Map m)
		: base(position, m)
	{
		stats = new Stats();
		health = stats.PlayerHealth;
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

	public virtual void Hit()
	{
		health--;

		if (!IsAlive)
			Die();
	}

	public virtual void Die()
	{
		health = 0;
	}

	public virtual void Respawn()
	{
		health = stats.PlayerHealth;
	}

	public override void Logic()
	{
		if (!IsAlive) return;

		DoPhysics();

		if (map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
			velocity.Y = 0;

		if (map.GetCollision(this, new Vector2(velocity.X, Math.Min(0, velocity.Y)) * Game.delta))
		{
			float stepFactor = stats.StepSize * velocity.Length * Game.delta;

			//Stepping
			if (!map.GetCollision(this, new Vector2(velocity.X * Game.delta, stepFactor)))
			{
				int accuracy = 32;
				float testStep = stepFactor / accuracy;
				float currentStep = 0;

				for (int i = 0; i < accuracy; i++)
				{
					if (map.GetCollision(this, new Vector2(velocity.X * Game.delta, currentStep)))
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

		if (map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
			velocity.X = 0;
		if (map.GetCollision(this, velocity * Game.delta))
			velocity = Vector2.Zero;

		position += velocity * Game.delta;

		if (velocity.X > 0) dir = 1;
		if (velocity.X < 0) dir = -1;
	}

	public virtual void DoPhysics()
	{
		velocity.X += currentAcceleration * Game.delta - velocity.X * Friction * Game.delta;
		currentAcceleration = 0;

		velocity.Y -= stats.Gravity * Game.delta;
	}

	public virtual void Jump()
	{
		velocity.Y = stats.JumpForce;
	}

	public void JumpHold()
	{
		if (velocity.Y >= stats.JumpAddLimit) velocity.Y += stats.JumpAddForce * Game.delta;
	}
}