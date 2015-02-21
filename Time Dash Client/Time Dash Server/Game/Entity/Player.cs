using OpenTK;
using OpenTK.Input;
using System;

using EZUDP;
using EZUDP.Server;
using TKTools;
using System.Collections.Generic;

public partial class Player : Actor
{
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
	public class DodgeTarget
	{
		public float timeTraveled = 0;

		public float stepLength = 0;
		public float stepAngle = 0;

		public Vector2 startPosition;
		public Direction direction;

		public float frameDirection = 0f;

		public Vector2 DirectionVector
		{
			get
			{
				switch (direction)
				{
					case Direction.Down: return new Vector2(0, -1);
					case Direction.Up: return new Vector2(0, 1);
					case Direction.Right: return new Vector2(1, 0);
					case Direction.Left: return new Vector2(-1, 0);
				}

				return new Vector2(1, 0);
			}
		}

		public DodgeTarget(Vector2 start, Direction dir)
		{
			startPosition = start;
			direction = dir;
		}

		public bool TargetReached(Vector2 pos)
		{
			float posLen = 0;

			switch (direction)
			{
				case Direction.Right:
				case Direction.Left:
					posLen = Math.Abs(pos.X - startPosition.X);
					break;

				case Direction.Up:
				case Direction.Down:
					posLen = Math.Abs(pos.Y - startPosition.Y);
					break;
			}

			return (posLen >= Stats.defaultStats.DodgeLength);
		}
	}

	PlayerInput inputData = new PlayerInput();
	PlayerInput input, oldInput;
	protected Vector2 inputDirection, oldInputDirection, lastDirection;

	public int id;
	public string playerName;
	public Client client;

	protected PlayerShadow shadow;

	public Team team;

	float wallStick = -1;
	bool canDoublejump = true;

	DashTarget dashTarget = null;
	DodgeTarget dodgeTarget = null;

	Timer dashCooldown;
	Timer dodgeCooldown;
	Timer disableTimer;

	float dodgeIntervalTimer = 0;

	float dodgeGravityIgnoreTime = 0.1f;
	float gravityIgnore = 0f;

	Timer updatePositionTimer = new Timer(0.05f, true);

	Timer respawnTimer = new Timer(2f, false);

	public int WallTouch
	{
		get
		{
			if (map.GetCollision(this, new Vector2(-0.1f, 0f)) && velocity.X < 0.1f) return -1;
			if (map.GetCollision(this, new Vector2(0.1f, 0)) && velocity.X > -0.1f) return 1;
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

	public bool IsDashing
	{
		get
		{
			return dashTarget != null;
		}
	}

	public bool IsDodging
	{
		get
		{
			return dodgeTarget != null;
		}
	}

	public bool CanDodge
	{
		get
		{
			return !Disabled;
		}
	}

	public bool CanDash
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

	public Player(int id, string name, Client c, Vector2 position, Map m)
		: base(position, m)
	{
		this.id = id;
		playerName = name;
		client = c;

		shadow = new PlayerShadow(this);

		dashCooldown = new Timer(stats.DashCooldown, false);
		dodgeCooldown = new Timer(stats.DodgeCooldown, true);
		disableTimer = new Timer(stats.DisableTime, true);
	}

	public virtual bool AlliedWith(Player p)
	{
		if (p.team == null || team == null) return false;
		else return p.team == team;
	}

	public void Hit(Player p, float dir)
	{
		Hit();
		SendHitToPlayer(p, dir, map.playerList);
	}

	public void Hit(Player p, Bullet b)
	{
		Hit();
		SendHitToPlayer(p, TKMath.GetAngle(b.position, position), map.playerList);
	}

	public override void Hit()
	{
		base.Hit();

		dashTarget = null;
		dodgeTarget = null;
	}

	public override void Die()
	{
		base.Die();

		SendDieToPlayer(map.playerList);
	}

	public virtual void OnKill(Player p)
	{
	}

	public override void Respawn()
	{
		base.Respawn();
		position = map.GetFreeSpawnPosition(this);
		SendRespawnToPlayer(position, map.playerList);
	}

	public override void Logic()
	{
		foreach (Bullet b in bulletList)
			if (b != null) b.Logic();

		if (!IsAlive)
		{
			respawnTimer.Logic();
			if (respawnTimer.IsDone)
			{
				Respawn();
				respawnTimer.Reset();
			}

			return;
		}

		dashCooldown.Logic();
		dodgeCooldown.Logic();
		disableTimer.Logic();

		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		if (gravityIgnore > 0 && !input.Equals(oldInput)) gravityIgnore = 0;
		gravityIgnore -= Game.delta;

		dodgeIntervalTimer -= Game.delta;

		if (dashTarget == null && dodgeTarget == null)
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
		else if (dashTarget != null)
		{
			DashStep();
		}
		else if (dodgeTarget != null)
		{
			DodgeStep();
			if (shadow != null) shadow.Logic();
		}

		if (SEND_SERVER_POSITION)
		{
			updatePositionTimer.Logic();

			SendServerPositionToPlayer(map.playerList);

			if (updatePositionTimer.IsDone)
			{
				SendServerPositionToPlayer(this);
				updatePositionTimer.Reset();
			}
		}
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

		/*
		if (input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump])
		{
			if (IsOnGround) Jump();
			else if (WallTouch != 0) WallJump();
			else if (canDoublejump)
			{
				Jump();
				canDoublejump = false;
			}
		}*/
		if (input[PlayerKey.Jump]) JumpHold();

		if (dashTargetBuffer != null)
		{
			Dash(dashTargetBuffer);
			dashTargetBuffer = null;
		}

		if (dodgeTargetBuffer != null)
		{
			Dodge(dodgeTargetBuffer);
			dodgeTargetBuffer = null;
		}
	}
}