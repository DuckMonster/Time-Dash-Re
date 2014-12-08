using OpenTK;
using OpenTK.Input;
using System;

public class Actor : Entity
{
	public class Physics
	{
		float gravity = 40f;
		float maxVelocity = 8f;
		float accelerationTime = 0.2f, accelerationTimeAir = 0.6f, decelerationTime = 0.3f, decelerationTimeAir = 1.8f;
		float jumpForce = 10f, jumpAddForce = 20f, jumpAddLim = 2f;

		float acceleration, accelerationAir, accFriction, accFrictionAir, decFriction, decFrictionAir;

		float GetFriction(float speed)
		{
			return -(float)(Math.Log(0.02, Math.E) / speed);
		}
		void CalculatePhysics()
		{
			accFriction = GetFriction(accelerationTime);
			accFrictionAir = GetFriction(accelerationTimeAir);
			decFriction = GetFriction(decelerationTime);
			decFrictionAir = GetFriction(decelerationTimeAir);

			acceleration = maxVelocity * accFriction;
			accelerationAir = maxVelocity * accFrictionAir;
		}

		public float Gravity { get { return gravity; } set { gravity = value; } }
		public float MaxVelocity
		{
			get
			{
				return maxVelocity;
			}
			set
			{
				maxVelocity = value;
				CalculatePhysics();
			}
		}
		public float AccelerationTime
		{
			get
			{
				return accelerationTime;
			}
			set
			{
				accelerationTime = value;
				CalculatePhysics();
			}
		}
		public float AccelerationTimeAir
		{
			get
			{
				return accelerationTimeAir;
			}
			set
			{
				accelerationTimeAir = value;
				CalculatePhysics();
			}
		}
		public float DecelerationTime
		{
			get
			{
				return decelerationTime;
			}
			set
			{
				decelerationTime = value;
				CalculatePhysics();
			}
		}
		public float DecelerationTimeAir
		{
			get
			{
				return decelerationTimeAir;
			}
			set {
				decelerationTimeAir = value;
				CalculatePhysics();
			}
		}

		public float Acceleration { get { return acceleration; } }
		public float AccelerationAir { get { return accelerationAir; } }
		public float AccFriction { get { return accFriction; } }
		public float AccFrictionAir { get { return accFrictionAir; } }
		public float DecFriction { get { return decFriction; } }
		public float DecFrictionAir { get { return decFrictionAir;} }

		public float JumpForce { get { return jumpForce; } set { jumpForce = value; } }
		public float JumpAddForce { get { return jumpAddForce; } set { jumpAddForce = value; } }
		public float JumpAddLim { get { return jumpAddLim; } set { jumpAddLim = value; } }

		public Physics()
		{
			CalculatePhysics();
		}
		public Physics(float g, float mv, float at, float dt, float dta, float jf, float jaf, float jal)
		{
			gravity = g;

			maxVelocity = mv;
			accelerationTime = at;
			decelerationTime = dt;
			decelerationTimeAir = dta;

			jumpForce = jf;
			jumpAddForce = jaf;
			jumpAddLim = jal;

			CalculatePhysics();
		}
	}

	protected Physics physics;

	protected Vector2 velocity = Vector2.Zero;
	protected float currentAcceleration = 0;
	protected int dir = 1;

	public Actor(Vector2 position, Map m)
		: base(position, m)
	{
		physics = new Physics();
	}

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
		if (velocity.Y >= physics.JumpAddLim) velocity.Y += physics.JumpAddForce * Game.delta;
	}
}