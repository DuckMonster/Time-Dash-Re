using System;
using System.Collections.Generic;

public class Physics
{
	public static readonly Physics StandardPhysics = new Physics(40, 6, 0.2f, 0.6f, 0.3f, 1.8f, 10f, 20f, 2f);

	public Dictionary<string, float> stats = new Dictionary<string, float>();
	void AddStat(string name, float s)
	{
		stats.Add(name, s);
	}

	float gravity = 40f;
	float maxVelocity = 6f;
	float accelerationTime = 0.2f, accelerationTimeAir = 0.6f, decelerationTime = 0.3f, decelerationTimeAir = 1.8f;
	float jumpForce = 10f, jumpAddForce = 20f, jumpAddLim = 2f;

	float acceleration, accelerationAir, accFriction, accFrictionAir, decFriction, decFrictionAir;

	float GetFriction(float speed)
	{
		return -(float)(Math.Log(0.02, Math.E) / speed);
	}
	void CalculatePhysics()
	{
		stats["acc_friction"] = GetFriction(stats["acceleration_time"]);
		stats["acc_friction_air"] = GetFriction(stats["acceleration_time_air"]);
		stats["dec_friction"] = GetFriction(stats["deceleration_time"]);
		stats["dec_friction_air"] = GetFriction(stats["deceleration_time_air"]);

		stats["acceleration"] = stats["max_velocity"] * stats["acc_friction"];
		stats["acceleration_air"] = stats["max_velocity"] * stats["acc_friction_air"];
	}

	public float Gravity { get { return stats["gravity"]; } set { stats["gravity"] = value; } }
	public float MaxVelocity
	{
		get
		{
			return stats["max_velocity"];
		}
		set
		{
			stats["max_velocity"] = value;
			CalculatePhysics();
		}
	}
	public float AccelerationTime
	{
		get
		{
			return stats["acceleration_time"];
		}
		set
		{
			stats["acceleration_time"] = value;
			CalculatePhysics();
		}
	}
	public float AccelerationTimeAir
	{
		get
		{
			return stats["acceleration_time_air"];
		}
		set
		{
			stats["acceleration_time_air"] = value;
			CalculatePhysics();
		}
	}
	public float DecelerationTime
	{
		get
		{
			return stats["deceleration_time"];
		}
		set
		{
			stats["deceleration_time"] = value;
			CalculatePhysics();
		}
	}
	public float DecelerationTimeAir
	{
		get
		{
			return stats["deceleration_time_air"];
		}
		set
		{
			stats["deceleration_time_air"] = value;
			CalculatePhysics();
		}
	}

	public float Acceleration { get { return stats["acceleration"]; } }
	public float AccelerationAir { get { return stats["acceleration_air"]; } }
	public float AccFriction { get { return stats["acc_friction"]; } }
	public float AccFrictionAir { get { return stats["acc_friction_air"]; } }
	public float DecFriction { get { return stats["dec_friction"]; } }
	public float DecFrictionAir { get { return stats["dec_friction_air"]; } }

	public float JumpForce { get { return stats["jump_force"]; } set { stats["jump_force"] = value; } }
	public float JumpAddForce { get { return stats["jump_add_force"]; } set { stats["jump_add_force"] = value; } }
	public float JumpAddLimit { get { return stats["jump_add_limit"]; } set { stats["jump_add_limit"] = value; } }

	public Physics()
	{
		CalculatePhysics();
	}
	public Physics(float g, float mv, float at, float ata, float dt, float dta, float jf, float jaf, float jal)
	{
		AddStat("gravity", g);

		AddStat("max_velocity", mv);
		AddStat("acceleration_time", at);
		AddStat("acceleration_time_air", ata);
		AddStat("deceleration_time", dt);
		AddStat("deceleration_time_air", dta);

		AddStat("jump_force", jf);
		AddStat("jump_add_force", jaf);
		AddStat("jump_add_limit", jal);

		AddStat("acc_friction", 0);
		AddStat("dec_friction", 0);
		AddStat("acc_friction_air", 0);
		AddStat("dec_friction_air", 0);
		AddStat("acceleration", 0);
		AddStat("acceleration_air", 0);

		CalculatePhysics();
	}

	public float this[string name]
	{
		get
		{
			return stats[name];
		}
		set
		{
			stats[name] = value;
		}
	}
}