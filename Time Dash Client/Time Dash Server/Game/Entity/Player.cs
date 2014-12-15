using OpenTK;
using OpenTK.Input;
using System;

using EZUDP;
using EZUDP.Server;
using TKTools;
using System.Collections.Generic;

public partial class Player : Actor
{
	class WarpTarget
	{
		public float distance = 0;

		public Vector2 startPosition;
		public Vector2 endPosition;

		public WarpTarget(Vector2 a, Vector2 b)
		{
			startPosition = a;
			endPosition = b;
		}
	}
	class DashTarget
	{
		public float distance = 0;
		public float lastStep = 0;

		public float angle;

		public Vector2 startPosition;
		public Vector2 endPosition;

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

	Timer warpCooldown = new Timer(1.2f, false);
	Timer dashCooldown = new Timer(0.4f, true);

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

	public bool IgnoresGravity
	{
		get
		{
			return gravityIgnore > 0;
		}
	}

	public Player(int id, Client c, Vector2 position, Map m)
		: base(position, m)
	{
		playerID = id;
		client = c;

		shadow = new PlayerShadow(this);
	}

	public override void Hit()
	{
		base.Hit();

		SendDieToPlayer(map.playerList);

		position = new Vector2(4, 30);
		velocity = Vector2.Zero;

		SendPositionToPlayer(map.playerList);
	}

	public override void Logic()
	{
		warpCooldown.Logic();
		dashCooldown.Logic();

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

			shadow.Logic();
		}
		else if (warpTarget != null)
		{
			WarpStep();
		}
		else if (dashTarget != null)
		{
			DashStep();
		}

		SendPositionToPlayer(this);
	}

	public override void DoPhysics()
	{
		bool aboveMaxSpeed = Math.Abs(velocity.X) > physics.MaxVelocity &&
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
		if (!IgnoresGravity) velocity.Y -= physics.Gravity * Game.delta;
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
			if (inputDirection.X != lastDirection.X) dashIntervalTimer = 0;

			if (inputDirection.X != oldInputDirection.X)
			{
				if (dashIntervalTimer > 0 && dashCooldown.IsDone)
				{
					Dash((int)inputDirection.X);
				}
				else
				{
					dashIntervalTimer = dashInterval;
				}
			}
			if (WallStickable && WallTouch == -inputDirection.X)
			{
				wallStick = 0.3f;
			}
			if (wallStick <= 0) currentAcceleration += Acceleration * inputDirection.X;

			lastDirection.X = inputDirection.X;
		}

		if (inputDirection.Y != 0)
		{
			if (inputDirection.Y != lastDirection.Y) dashIntervalTimer = 0;

			if (inputDirection.Y != oldInputDirection.Y)
			{
				if (dashIntervalTimer > 0 && dashCooldown.IsDone)
				{
					DashVertical((int)inputDirection.Y);
				}
				else
				{
					dashIntervalTimer = dashInterval;
				}
			}

			lastDirection.Y = inputDirection.Y;
		}

		oldInputDirection = inputDirection;

		if (input[PlayerKey.Warp] && !oldInput[PlayerKey.Warp] && warpCooldown.IsDone)
		{
			Warp(shadow.CurrentPosition);
		}

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
	}

	public override void Jump()
	{
		base.Jump();
	}

	public void WallJump()
	{
		Vector2 velo = physics.WallJumpVector;

		velocity = new Vector2(velo.X * -WallTouch, velo.Y);

		wallStick = 0f;

		canDoublejump = true;
	}

	public void Dash(int dir)
	{
		dashTarget = new DashTarget(position, position + new Vector2(physics.DashLength * dir, 0));
		dashCooldown.Reset();

		if (!IsOnGround) gravityIgnore = dashGravityIgnoreTime;
	}

	public void DashVertical(int dir)
	{
		dashTarget = new DashTarget(position, position + new Vector2(0, physics.DashLength * dir));
		dashCooldown.Reset();
	}

	public void DashStep()
	{
		Vector2 diffVector = dashTarget.endPosition - position;
		Vector2 directionVector = diffVector.Normalized();

		float factor = TKMath.Exp(Math.Max(0, 0.4f - dashTarget.distance), 1.5f, 30);

		Vector2 diffStepVector = directionVector * physics.DashVelocity * factor;

		if (diffStepVector.Length * Game.delta < diffVector.Length)
		{
			Vector2 collisionVec;
			bool collision = map.RayTraceCollision(position, position + diffStepVector * Game.delta, size, out collisionVec);

			List<Player> playerList = map.RayTrace(position, position + diffStepVector * Game.delta, size, this);
			foreach (Player p in playerList) p.Hit();

			position += diffStepVector * Game.delta;

			dashTarget.distance += Game.delta;
			dashTarget.lastStep = diffStepVector.Length;

			if (collision)
				DashEnd();
		}
		else
		{
			DashEnd();
		}
	}

	public void DashEnd()
	{
		position = dashTarget.endPosition;

		velocity = (dashTarget.endPosition - dashTarget.startPosition).Normalized() * physics.DashEndVelocity;

		dashTarget = null;
	}

	public void Warp(Vector2 target)
	{
		warpTarget = new WarpTarget(position, target);
		warpCooldown.Reset();
	}

	public void WarpStep()
	{
		Vector2 diffVector = warpTarget.endPosition - position;
		Vector2 directionVector = diffVector.Normalized();

		float factor = TKMath.Exp(Math.Max(0, 0.4f - warpTarget.distance), 2f, 20);

		Vector2 diffStepVector = directionVector * physics.WarpVelocity * factor;

		if (diffStepVector.Length * Game.delta < diffVector.Length)
		{
			List<Player> playerList = map.RayTrace(position, position + diffStepVector * Game.delta, size, this);
			foreach (Player p in playerList)
			{
				if (p.IsWarping)
				{
					WarpEnd(p);
					p.WarpEnd(this);
				}
				else
				{
					p.Hit();
				}
			}

			if (warpTarget == null) return;

			position += diffStepVector * Game.delta;
			warpTarget.distance += Game.delta;
		}
		else
		{
			WarpEnd();
		}
	}

	public void WarpEnd()
	{
		WarpEnd(warpTarget.endPosition, (warpTarget.endPosition - warpTarget.startPosition).Normalized() * physics.WarpEndVelocity);
	}

	public void WarpEnd(Player p)
	{
		WarpEnd(p.position, (position - p.position).Normalized() * physics.WarpEndVelocity);
	}

	public void WarpEnd(Vector2 pos, Vector2 velo)
	{
		position = pos;
		velocity = velo;
		warpTarget = null;
	}
}