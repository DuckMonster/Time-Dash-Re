using System;
using OpenTK;
using TKTools;

public class Physics
{
	public static readonly Physics defaultPhysics = new Physics();

	float gravity = 40f;
	float maxVelocity = 6f;
	float accelerationTime = 0.2f, accelerationTimeAir = 0.6f, decelerationTime = 0.3f, decelerationTimeAir = 1.8f;
	float jumpForce = 10f, jumpAddForce = 20f, jumpAddLimit = 2f;

	float acceleration, accelerationAir, accFriction, accFrictionAir, decFriction, decFrictionAir;
	float warpVelocity = 18f;

	float wallJumpVelocity = 14f, wallJumpAngle = 50f;

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
		set
		{
			decelerationTimeAir = value;
			CalculatePhysics();
		}
	}

	public float Acceleration { get { return acceleration; } }
	public float AccelerationAir { get { return accelerationAir; } }
	public float AccFriction { get { return accFriction; } }
	public float AccFrictionAir { get { return accFrictionAir; } }
	public float DecFriction { get { return decFriction; } }
	public float DecFrictionAir { get { return decFrictionAir; } }

	public float JumpForce { get { return jumpForce; } set { jumpForce = value; } }
	public float JumpAddForce { get { return jumpAddForce; } set { jumpAddForce = value; } }
	public float JumpAddLimit { get { return jumpAddLimit; } set { jumpAddLimit = value; } }

	public float WarpVelocity { get { return warpVelocity; } set { warpVelocity = value; } }

	public float WallJumpVelocity { get { return wallJumpVelocity; } set { wallJumpVelocity = value; } }
	public float WallJumpAngle { get { return wallJumpAngle; } set { wallJumpAngle = value; } }
	public Vector2 WallJumpVector
	{
		get
		{
			return TKMath.GetAngleVector(WallJumpAngle) * WallJumpVelocity;
		}
	}

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
		jumpAddLimit = jal;

		CalculatePhysics();
	}
}