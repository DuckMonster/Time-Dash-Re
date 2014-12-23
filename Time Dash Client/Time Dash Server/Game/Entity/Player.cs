using OpenTK;
using OpenTK.Input;
using System;

using EZUDP;
using EZUDP.Server;
using TKTools;
using System.Collections.Generic;

public partial class Player : Actor
{
	public class WarpTarget
	{
		public float timeTraveled = 0;
		public float lastStep = 0;

		public float angle;

		public Vector2 startPosition;
		public Vector2 endPosition;

		public Vector2 Direction
		{
			get
			{
				return (endPosition - startPosition).Normalized();
			}
		}

		public WarpTarget(Vector2 a, Vector2 b)
		{
			startPosition = a;
			endPosition = b;
			angle = TKMath.GetAngle(a, b);
		}
	}
	public class DashTarget
	{
		public float timeTraveled = 0;
		public float lastStep = 0;

		public float angle;

		public Vector2 startPosition;
		public Vector2 endPosition;

		public Vector2 Direction
		{
			get
			{
				return (endPosition - startPosition).Normalized();
			}
		}

		public DashTarget(Vector2 a, Vector2 b)
		{
			startPosition = a;
			endPosition = b;
			angle = TKMath.GetAngle(a, b);
		}
	}

	PlayerInput inputData = new PlayerInput();
	PlayerInput input, oldInput;
	protected Vector2 inputDirection, oldInputDirection, lastDirection;

	public int playerID;
	public Client client;

	protected PlayerShadow shadow;

	float wallStick = -1;
	bool canDoublejump = true;

	WarpTarget warpTarget = null;
	DashTarget dashTarget = null;

	Timer warpCooldown;
	Timer dashCooldown;
	Timer disableTimer;

	float dashInterval = 0.2f;
	float dashIntervalTimer = 0;

	float dashGravityIgnoreTime = 0.2f;
	float gravityIgnore = 0f;

	public int WallTouch
	{
		get
		{
			if (map.GetCollision(this, new Vector2(-0.2f, 0f))) return -1;
			if (map.GetCollision(this, new Vector2(0.2f, 0))) return 1;
			return 0;
		}
	}

	public bool WallStickable
	{
		get
		{
			return wallStick == -1;
		}
		set
		{
			if (value) wallStick = -1;
		}
	}

	public bool IsWarping
	{
		get
		{
			return warpTarget != null;
		}
	}

	public bool IsDashing
	{
		get
		{
			return dashTarget != null;
		}
	}

	public bool CanDash
	{
		get
		{
			if (IsOnGround)
				return !Disabled;
			else
				return !Disabled && airDashNmbr > 0;
		}
	}

	public bool CanWarp
	{
		get
		{
			return shadow != null;
		}
	}

	public bool IgnoresGravity
	{
		get
		{
			return gravityIgnore > 0;
		}
	}

	public bool Disabled
	{
		get
		{
			return !disableTimer.IsDone;
		}
		set
		{
			if (value)
			{
				disableTimer.Reset();
				SendDisableToPlayer(map.playerList);
			}
			else disableTimer.IsDone = true;
		}
	}

	public Player(int id, Client c, Vector2 position, Map m)
		: base(position, m)
	{
		playerID = id;
		client = c;

		shadow = new PlayerShadow(this);

		warpCooldown = new Timer(stats.WarpCooldown, false);
		dashCooldown = new Timer(stats.DashCooldown, true);
		disableTimer = new Timer(stats.DisableTime, true);
	}

	public override void Hit()
	{
		base.Hit();

		SendDieToPlayer(map.playerList);

		position = new Vector2(4, 30);
		velocity = Vector2.Zero;

		warpTarget = null;
		dashTarget = null;

		SendPositionToPlayerForce(map.playerList);
	}

	public override void Logic()
	{
		warpCooldown.Logic();
		dashCooldown.Logic();
		disableTimer.Logic();

		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		if (gravityIgnore > 0 && !input.Equals(oldInput)) gravityIgnore = 0;
		gravityIgnore -= Game.delta;

		dashIntervalTimer -= Game.delta;

		if (warpTarget == null && dashTarget == null)
		{
			Input();

			if (!WallStickable && WallTouch == 0) WallStickable = true;
			if (wallStick > 0)
			{
				if (WallTouch == 0 || IsOnGround)
				{
					wallStick = 0;
					WallStickable = true;
				}
				wallStick -= Game.delta;
			}

			if (!canDoublejump && IsOnGround) canDoublejump = true;

			base.Logic();

			if (shadow == null && (IsOnGround || WallTouch != 0)) CreateShadow();
			if (shadow != null) shadow.Logic();
		}
		else if (warpTarget != null)
		{
			WarpStep();
		}
		else if (dashTarget != null)
		{
			DashStep();
			if (shadow != null) shadow.Logic();
		}

		if (SEND_SERVER_POSITION) SendServerPositionToPlayer(map.playerList);
	}

	public override void DoPhysics()
	{
		bool aboveMaxSpeed = Math.Abs(velocity.X) > stats.MaxVelocity &&
			((velocity.X > 0 && input[PlayerKey.Right]) ||
			(velocity.X < 0 && input[PlayerKey.Left]));

		if (aboveMaxSpeed)
		{
			if (!IsOnGround)
			{
				Log.Debug("GOTTA GO FAST!");
			}
			else
			{
				velocity.X += (currentAcceleration * Game.delta - velocity.X * Friction * Game.delta) * 0.2f;
			}
		}
		else
		{
			velocity.X += currentAcceleration * Game.delta - velocity.X * Friction * Game.delta;
		}

		currentAcceleration = 0;
		if (!IgnoresGravity) velocity.Y -= stats.Gravity * Game.delta;
	}

	public void Land()
	{
		airDashNmbr = stats.AirDashMax;
	}

	public void Input()
	{
		inputDirection = Vector2.Zero;
		if (input[PlayerKey.Right]) inputDirection.X++;
		if (input[PlayerKey.Left]) inputDirection.X--;

		if (input[PlayerKey.Up]) inputDirection.Y++;
		if (input[PlayerKey.Down]) inputDirection.Y--;

		if (inputDirection.X != 0)
		{
			if (WallStickable && WallTouch == -inputDirection.X)
			{
				wallStick = 0.3f;
			}
			if (wallStick <= 0) currentAcceleration += Acceleration * inputDirection.X;

			lastDirection.X = inputDirection.X;
		}

		oldInputDirection = inputDirection;

		if (input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump])
		{
			if (IsOnGround) Jump();
			else if (WallTouch != 0) WallJump();
			else if (canDoublejump)
			{
				Jump();
				canDoublejump = false;
			}
		}
		if (input[PlayerKey.Jump]) JumpHold();

		if (warpTargetBuffer != null)
		{
			Warp(warpTargetBuffer);
			warpTargetBuffer = null;
		}

		if (dashTargetBuffer != null)
		{
			Dash(dashTargetBuffer);
			dashTargetBuffer = null;
		}
	}
}