using System;
using System.Reflection;

using OpenTK;
using TKTools;

public class Stats
{
	public static readonly Stats defaultStats = new Stats();

	public float this[string name]
	{
		set
		{
			this.GetType().GetProperty(name).SetValue(this, value);
		}
		get
		{
			return (float)this.GetType().GetProperty(name).GetValue(this);
		}
	}

	public PropertyInfo[] GetAllStats()
	{
		return this.GetType().GetProperties();
	}

	#region Physics

	float gravity = 40f;
	float maxVelocity = 6f;
	float accelerationTime = 0.2f, accelerationTimeAir = 0.6f, decelerationTime = 0.3f, decelerationTimeAir = 1.8f;
	float jumpForce = 12f, jumpAddForce = 20f, jumpAddLimit = 2f;

	float stepSize = 0.6f;

	float acceleration, accelerationAir, accFriction, accFrictionAir, decFriction, decFrictionAir;
	float dashVelocity = 480f, dashEndVelocity = 18f;

	float wallJumpVelocity = 15f, wallJumpAngle = 50f;
	float dodgeVelocity = 40f, dodgeEndVelocity = 13f, dodgeLength = 3.5f;

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

	public float StepSize { get { return stepSize; } }

	public float DashVelocity { get { return dashVelocity; } set { dashVelocity = value; } }
	public float DashEndVelocity { get { return dashEndVelocity; } set { dashEndVelocity = value; } }

	public float WallJumpVelocity { get { return wallJumpVelocity; } set { wallJumpVelocity = value; } }
	public float WallJumpAngle { get { return wallJumpAngle; } set { wallJumpAngle = value; } }

	public Vector2 WallJumpVector
	{
		get
		{
			return TKMath.GetAngleVector(WallJumpAngle) * WallJumpVelocity;
		}
	}

	public float DodgeVelocity { get { return dodgeVelocity; } set { dodgeVelocity = value; } }
	public float DodgeEndVelocity { get { return dodgeEndVelocity; } set { dodgeEndVelocity = value; } }
	public float DodgeLength { get { return dodgeLength; } set { dodgeLength = value; } }

	#endregion

	#region Cooldowns

	float dashCooldown = 0.8f, dodgeCooldown = 1f, disabledTime = 0.8f;
	int airDodgeMax = 1;

	public float DisableTime { get { return disabledTime; } set { disabledTime = value; } }
	public float DodgeCooldown { get { return dodgeCooldown; } set { dodgeCooldown = value; } }
	public float DashCooldown { get { return dashCooldown; } set { dashCooldown = value; } }
	public int AirDodgeMax { get { return airDodgeMax; } set { airDodgeMax = value; } }

	#endregion

	public Stats()
	{
		CalculatePhysics();
	}
}