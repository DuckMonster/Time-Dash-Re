﻿using OpenTK;
using OpenTK.Input;
using System;
using TKTools;

public class Actor : Entity
{
	protected Stats stats = Stats.defaultStats;

	public Vector2 velocity = Vector2.Zero;
	protected float currentAcceleration = 0;
	public int dir = 1;
	bool previousOnGround = false;

	public int health = 1;

	public Actor(Vector2 position, Map m)
		: base(position, m)
	{
		stats = new Stats();
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
		set
		{
			if (!value) health = 0;
		}
	}

	public virtual void Hit()
	{
		health--;
		velocity = Vector2.Zero;
	}

	public virtual void Respawn(Vector2 pos)
	{
		position = pos;
		health = 1;
	}

	public override void Logic()
	{
		if (!IsAlive) return;

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
		if (!IsAlive) return;

		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(position);

		mesh.Draw();
	}
}