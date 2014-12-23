using OpenTK;
using OpenTK.Input;
using System;
using TKTools;

public class Actor : Entity
{
	protected Stats stats = Stats.defaultStats;

	protected Vector2 velocity = Vector2.Zero;
	protected float currentAcceleration = 0;
	public int dir = 1;
	bool previousOnGround = false;

	public Actor(Vector2 position, Map m)
		: base(position, m)
	{
		stats = new Stats();
	}

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

	public virtual void Die()
	{

	}

	public override void Logic()
	{
		DoPhysics();

		if (map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
			velocity.Y = 0;
		if (map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
			velocity.X = 0;
		if (map.GetCollision(this, velocity * Game.delta))
			velocity = Vector2.Zero;

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
		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(position);

		mesh.Draw();
	}
}