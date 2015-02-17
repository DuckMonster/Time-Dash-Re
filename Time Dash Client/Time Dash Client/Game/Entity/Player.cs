using EZUDP;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using TKTools;

public partial class Player : Actor
{
	public static Color[] colorList = new Color[] {
		Color.Blue,
		Color.Orange,
		Color.Green,
		Color.Violet,
		Color.Yellow,
		Color.Teal
	};

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

	protected bool updateInput;
	protected PlayerInput inputData = new PlayerInput();
	protected PlayerInput input, oldInput;
	protected Vector2 inputDirection, oldInputDirection, lastDirection;

	protected PlayerShadow shadow;

	public Team team;

	float wallStick = -1;
	bool canDoublejump = true;

	DashTarget dashTarget = null;
	DodgeTarget dodgeTarget = null;

	Timer dashCooldown;
	Timer dodgeCooldown;
	Timer disableTimer;

	CircleBar dashCooldownBar = new CircleBar(2.5f, 0.5f),
		dodgeCooldownBar = new CircleBar(2f, 0.3f);

	float dodgeInterval = 0.2f;
	float dodgeIntervalTimer = 0;

	float dodgeGravityIgnoreTime = 0.1f;
	float gravityIgnore = 0f;

	public Tileset playerTileset = new Tileset(250, 200, "Res/jacktilesetnew.png"),
		occlusionTileset = new Tileset(250, 200, "Res/jackShadowTileset.png");

	public Sound jumpSound = new Sound(@"Res\Snd\jump.wav"),
		dashSound = new Sound(@"Res\Snd\dash.wav");

	bool bufferFrame = false;

	public int WallTouch
	{
		get
		{
			if (map.GetCollision(this, new Vector2(-0.1f, 0f)) && velocity.X < 0.1f) return -1;
			if (map.GetCollision(this, new Vector2(0.1f, 0)) && velocity.X > -0.1f) return 1;
			return 0;
		}
	}

	public virtual Color Color
	{
		get
		{
			if (team != null) return team.Color;
			return colorList[playerID % colorList.Length];
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
	public bool IsDodgeing
	{
		get
		{
			return dodgeTarget != null;
		}
	}
	public bool CanDash
	{
		get
		{
			return shadow != null && dashCooldown.IsDone;
		}
	}
	public bool CanDodge
	{
		get
		{
			if (IsOnGround)
				return !Disabled;
			else
				return !Disabled;
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
			if (!value) disableTimer.IsDone = true;
			else disableTimer.Reset();
		}
	}

	public Player(int id, string name, Vector2 position, Map m)
		: base(position, m)
	{
		playerID = id;
		playerName = name;

		float charHeight = 165f;
		float h = (float)playerTileset.TileHeight / charHeight;
		float w = ((float)playerTileset.TileWidth / playerTileset.TileHeight);

		mesh.Dispose();
		mesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);

		mesh.Vertices = new Vector2[] {
			new Vector2(-0.5f * w * h, -0.5f + h),
			new Vector2(0.5f * w * h, -0.5f + h),
			new Vector2(0.5f * w * h, -0.5f),
			new Vector2(-0.5f * w * h, - 0.5f)
		};

		mesh.UV = new Vector2[] {
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(1, 1),
			new Vector2(0, 1)
		};

		mesh.Texture = playerTileset.sourceTexture;

		shadow = new PlayerShadow(this, mesh);

		dashCooldown = new Timer(stats.DashCooldown, false);
		dodgeCooldown = new Timer(stats.DodgeCooldown, true);
		disableTimer = new Timer(stats.DisableTime, true);
	}

	public override void Dispose()
	{
		base.Dispose();
		playerTileset.Dispose();

		foreach (Bullet b in bulletList)
			if (b != null) b.Dispose();
	}

	public override void Hit()
	{
		base.Hit();
	}

	public override void Die(Vector2 diePos)
	{
		map.AddEffect(new EffectSkull(diePos, Color, map));

		dashTarget = null;
		dodgeTarget = null;

		dashCooldown.Reset();

		base.Die(diePos);
	}

	public override void Respawn(Vector2 pos)
	{
		base.Respawn(pos);
		map.AddEffect(new EffectRing(position, 4f, 1.5f, Color, map));
	}

	public override void Logic()
	{
		foreach (Bullet b in bulletList)
			if (b != null) b.Logic();

		if (!IsAlive) return;

		if (shadow != null && !dashCooldown.IsDone)
		{
			if (!IsDashing) dashCooldown.Logic();
			dashCooldownBar.Progress = 1 - dashCooldown.PercentageDone;
			dashCooldownBar.Logic();

			if (dashCooldown.IsDone && IsLocalPlayer)
			{
				map.AddEffect(new EffectRing(shadow.CurrentPosition, 1.2f, 0.5f, Color, map));
			}
		}

		if (!dodgeCooldown.IsDone)
		{
			dodgeCooldown.Logic();
			dodgeCooldownBar.Progress = 1 - dodgeCooldown.PercentageDone;
			dodgeCooldownBar.Logic();
		}

		disableTimer.Logic();

		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		if (!input.Equals(oldInput) && IsLocalPlayer)
		{
			if (IsDodgeing || IsDashing)
				SendInputPure();
			else
				SendInput();
		}

		if (gravityIgnore > 0 && !input.Equals(oldInput)) { gravityIgnore = 0; }
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
	}

	public override void DoPhysics()
	{
		bool aboveMaxSpeed = Math.Abs(velocity.X) > stats.MaxVelocity &&
			((velocity.X > 0 && input[PlayerKey.Right]) ||
			(velocity.X < 0 && input[PlayerKey.Left]));

		if (aboveMaxSpeed)
		{
			if (IsOnGround)
				velocity.X += (currentAcceleration * Game.delta - velocity.X * Friction * Game.delta) * 0.2f;
		}
		else
			velocity.X += currentAcceleration * Game.delta - velocity.X * Friction * Game.delta;

		currentAcceleration = 0;
		if (!IgnoresGravity) velocity.Y -= stats.Gravity * Game.delta;
	}

	public override void Land()
	{
		base.Land();

		//if (IsLocalPlayer) SendLand();
	}

	public void Input()
	{
		inputDirection = Vector2.Zero;
		if (input[PlayerKey.Right]) inputDirection.X++;
		if (input[PlayerKey.Left]) inputDirection.X--;

		if (input[PlayerKey.Up])
			inputDirection.Y++;
		if (input[PlayerKey.Down])
			inputDirection.Y--;

		if (IsLocalPlayer)
		{
			//Dodging
			if (dodgeCooldown.IsDone && inputDirection != Vector2.Zero && oldInputDirection != inputDirection)
			{
				if (inputDirection == lastDirection && dodgeIntervalTimer > 0)
				{
					if (inputDirection.X > 0)
						Dodge(Direction.Right);
					else if (inputDirection.X < 0)
						Dodge(Direction.Left);
					else if (inputDirection.Y > 0)
						Dodge(Direction.Up);
					else if (inputDirection.Y < 0)
						Dodge(Direction.Down);
				}

				dodgeIntervalTimer = dodgeInterval;
				lastDirection = inputDirection;
			}

			//Jumping
			if (input[PlayerKey.Jump] && (!oldInput[PlayerKey.Jump] || bufferFrame))
			{
				if (IsOnGround) Jump();
				else if (WallTouch != 0) WallJump();
				else if (canDoublejump)
				{
					Jump();
					canDoublejump = false;
				}

				SendJump();
				bufferFrame = false;
			}

			//Dash
			if (input[PlayerKey.Dash] && !oldInput[PlayerKey.Dash] && CanDash)
			{
				Dash(shadow.CurrentPosition);
			}

			//Shooting
			if (input[PlayerKey.Shoot] && !oldInput[PlayerKey.Shoot])
				Shoot(MouseInput.Current.Position);
		}
		else
		{
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

		if (inputDirection.X != 0)
		{
			if (WallStickable && WallTouch == -inputDirection.X)
				wallStick = 0.3f;

			if (wallStick <= 0) currentAcceleration += Acceleration * inputDirection.X;
		}

		oldInputDirection = inputDirection;

		if (input[PlayerKey.Jump]) JumpHold();

		bufferFrame = false;
	}

	public void LocalInput()
	{
		if (Program.client.Focused)
		{
			inputData[PlayerKey.Right] = KeyboardInput.Current[Key.D];
			inputData[PlayerKey.Left] = KeyboardInput.Current[Key.A];
		}
		else
		{
			inputData[PlayerKey.Right] = KeyboardInput.Current[Key.Left];
			inputData[PlayerKey.Left] = KeyboardInput.Current[Key.Right];
		}

		inputData[PlayerKey.Up] = KeyboardInput.Current[Key.W];
		inputData[PlayerKey.Down] = KeyboardInput.Current[Key.S];
		inputData[PlayerKey.Jump] = KeyboardInput.Current[Key.Space];
		inputData[PlayerKey.Dash] = KeyboardInput.Current[Key.E];
		inputData[PlayerKey.Shoot] = MouseInput.Current[MouseButton.Left];
	}

	public override void Draw()
	{
		if (!IsAlive) return;

		if (WallTouch != 0 && !IsOnGround)
		{
			playerTileset.X = 4;
		}
		else if (IsOnGround)
		{
			if (Math.Abs(velocity.X) > 1f)
			{
				playerTileset.X = 1;
			}
			else
			{
				playerTileset.X = 0;
			}
		}
		else
		{
			if (velocity.Y > 0) playerTileset.X = 2;
			else playerTileset.X = 3;
		}

		mesh.FillColor = false;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(size);

		if (dashTarget != null)
		{
			mesh.Rotate(dashTarget.angle);
			mesh.Scale(1 + dashTarget.lastStep * 2.4f, 1 - dashTarget.lastStep * 0.5f);
			mesh.Rotate(-dashTarget.angle);
		}

		if (dodgeTarget != null)
		{
			mesh.Rotate(dodgeTarget.stepAngle);
			mesh.Scale(1 + dodgeTarget.stepLength * 1.5f, 1 - dodgeTarget.stepLength * 0.7f);
			mesh.Rotate(-dodgeTarget.stepAngle);
		}

		mesh.Scale(new Vector2(dir, 1));

		mesh.Color = Color.Black;
		mesh.Draw(occlusionTileset, playerTileset.X, playerTileset.Y);
		mesh.Color = Color.White;

		mesh.Draw(playerTileset);

		if (IsLocalPlayer)
		{
			if (!dodgeCooldown.IsDone)
			{
				dodgeCooldownBar.Draw(position, Color);
			}
			if (!dashCooldown.IsDone)
			{
				dashCooldownBar.Draw(position, Color);
			}
		}

		float glow = Math.Max(1 - dodgeCooldown.PercentageDone, 1 - dashCooldown.PercentageDone);
		if (glow > 0)
		{
			mesh.FillColor = true;
			mesh.Color = new Color(1, 1, 1, glow);

			mesh.Draw(playerTileset, playerTileset.X, playerTileset.Y);
		}

		if (Disabled)
		{
			mesh.Color = new Color(0.3f, 0.3f, 1f, 1 - disableTimer.PercentageDone);
			mesh.FillColor = true;

			mesh.Draw(playerTileset);
		}

		if (receivedServerPosition)
		{
			mesh.Color = new Color(0, 1, 0, 0.5f);
			mesh.FillColor = true;

			mesh.Reset();

			mesh.Translate(serverPosition);
			mesh.Scale(size);
			mesh.Scale(new Vector2(-dir, 1));

			mesh.Draw(playerTileset);
		}

		//if (shadow != null && IsLocalPlayer && (dashCooldown.IsDone || IsDashing) && !Disabled) shadow.Draw();
		if (IsLocalPlayer && (CanDash || IsDashing)) shadow.Draw();

		foreach (Bullet b in bulletList)
			if (b != null) b.Draw();
	}
}