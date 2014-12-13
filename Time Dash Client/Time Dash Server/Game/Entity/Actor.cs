using OpenTK;
using OpenTK.Input;
using System;

public class Actor : Entity
{
	protected Physics physics = Physics.defaultPhysics;

	protected Vector2 velocity = Vector2.Zero;
	protected float currentAcceleration = 0;
	protected int dir = 1;

	public float Acceleration
	{
		get
		{
			return IsOnGround ? physics.Acceleration : physics.AccelerationAir;
		}
	}

	public float Friction
	{
		get
		{
			return currentAcceleration == 0 ?
				(IsOnGround ? physics.DecFriction : physics.DecFrictionAir) :
				(IsOnGround ? physics.AccFriction : physics.AccFrictionAir);
		}
	}

	public bool IsOnGround
	{
		get
		{
			return map.GetCollision(this, new Vector2(0, -0.1f));
		}
	}

	public Actor(Vector2 position, Map m)
		: base(position, m)
	{
		physics = new Physics();
	}

	public virtual void Hit()
	{
		Log.Write("OUCH!");
	}

	public override void Logic()
	{
		velocity.X += currentAcceleration * Game.delta - velocity.X * Friction * Game.delta;
		currentAcceleration = 0;

		velocity.Y -= physics.Gravity * Game.delta;

		if (map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
			velocity.Y = 0;
		if (map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
			velocity.X = 0;
		if (map.GetCollision(this, velocity * Game.delta))
			velocity = Vector2.Zero;

		position += velocity * Game.delta;

		if (velocity.X > 0) dir = 1;
		if (velocity.X < 0) dir = -1;
	}

	public void Jump()
	{
		velocity.Y = physics.JumpForce;
	}

	public void JumpHold()
	{
		if (velocity.Y >= physics.JumpAddLimit) velocity.Y += physics.JumpAddForce * Game.delta;
	}
}