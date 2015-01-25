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

		public DodgeTarget(Vector2 a, Vector2 b)
		{
			startPosition = a;
			endPosition = b;
			angle = TKMath.GetAngle(a, b);
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

	float dodgeGravityIgnoreTime = 0.2f;
	float gravityIgnore = 0f;

	public Tileset playerTileset = new Tileset(200, 160, "Res/jackTileset.png"),
		occlusionTileset = new Tileset(200, 160, "Res/jackShadowTileset.png");

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
			return colorList[playerID];
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
		//float w = (size.X / size.Y);
		float w = (float)playerTileset.TileHeight / playerTileset.TileWidth / 2;

		w = w * (size.X / size.Y);

		mesh.UV = new Vector2[] {
			new Vector2(0.5f-w, 0f),
			new Vector2(0.5f+w, 0f),
			new Vector2(0.5f+w, 1f),
			new Vector2(0.5f-w, 1f)
		};

		shadow = new PlayerShadow(this, mesh);

		dashCooldown = new Timer(stats.DashCooldown, false);
		dodgeCooldown = new Timer(stats.DodgeCooldown, true);
		disableTimer = new Timer(stats.DisableTime, true);
	}

	public override void Dispose()
	{
		base.Dispose();
		playerTileset.Dispose();
	}

	public virtual void Hit(Vector2 diePos)
	{
		base.Hit();
		map.AddEffect(new EffectSkull(diePos, Color, map));

		dashTarget = null;
		dodgeTarget = null;

		dashCooldown.Reset();
	}

	public virtual void Kill(Player p)
	{
	}

	public override void Respawn(Vector2 pos)
	{
		base.Respawn(pos);
		map.AddEffect(new EffectRing(position, 4f, 1.5f, Color, map));
	}

	public override void Logic()
	{
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
		airDodgeNmbr = stats.AirDodgeMax;

		base.Land();

		//if (IsLocalPlayer) SendLand();
	}

	public void Input()
	{
		inputDirection = Vector2.Zero;
		if (input[PlayerKey.Right]) inputDirection.X++;
		if (input[PlayerKey.Left]) inputDirection.X--;

		if (input[PlayerKey.Up]) inputDirection.Y++;
		if (input[PlayerKey.Down]) inputDirection.Y--;

		if (IsLocalPlayer)
		{
			//Dodging hori
			if (inputDirection.X != 0)
			{
				if (inputDirection.X != lastDirection.X) dodgeIntervalTimer = 0;

				if (inputDirection.X != oldInputDirection.X)
				{
					if (dodgeIntervalTimer > 0 && dodgeCooldown.IsDone)
						Dodge((int)inputDirection.X);
					else
						dodgeIntervalTimer = dodgeInterval;

					lastDirection.X = inputDirection.X;
				}
			}

			//Dodging vert
			if (inputDirection.Y != 0)
			{
				if (inputDirection.Y != oldInputDirection.Y)
				{
					if (dodgeIntervalTimer > 0 && dodgeCooldown.IsDone)
						DodgeVertical((int)inputDirection.Y);
					else
						dodgeIntervalTimer = dodgeInterval;

					lastDirection.Y = inputDirection.Y;
				}
			}

			//Jumping
			if (input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump])
			{
				if (IsOnGround) Jump();
				else if (WallTouch != 0) WallJump();
				else if (canDoublejump)
				{
					Jump();
					canDoublejump = false;
				}

				SendJump();
			}

			//Dash
			if (input[PlayerKey.Dash] && !oldInput[PlayerKey.Dash] && CanDash)
			{
				Dash(shadow.CurrentPosition);
			}
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
	}

	public void LocalInput()
	{
		if (Program.client.Focused)
		{
			inputData[PlayerKey.Right] = KeyboardInput.Current[Key.Right];
			inputData[PlayerKey.Left] = KeyboardInput.Current[Key.Left];
		}
		else
		{
			inputData[PlayerKey.Right] = KeyboardInput.Current[Key.Left];
			inputData[PlayerKey.Left] = KeyboardInput.Current[Key.Right];
		}

		//inputData[PlayerKey.Up] = KeyboardInput.Current[Key.Up];
		inputData[PlayerKey.Down] = KeyboardInput.Current[Key.Down];
		inputData[PlayerKey.Jump] = KeyboardInput.Current[Key.Z];
		inputData[PlayerKey.Dash] = KeyboardInput.Current[Key.X];
	}

	public override void Draw()
	{
		if (!IsAlive) return;

		if (WallTouch != 0 && !IsOnGround)
		{
			playerTileset.X = 0;
		}
		else if (IsOnGround)
		{
			if (Math.Abs(velocity.X) > 1f)
			{
				playerTileset.X = 1;
			}
			else
			{
				playerTileset.X = 3;
			}
		}
		else
		{
			if (velocity.Y > 0) playerTileset.X = 4;
			else playerTileset.X = 2;
		}

		mesh.FillColor = true;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(size);

		if (dashTarget != null)
		{
			mesh.Rotate(dashTarget.angle);
			mesh.Scale(1 + dashTarget.lastStep * 8.5f, 1 - dashTarget.lastStep * 2.7f);
			mesh.Rotate(-dashTarget.angle);
		}

		if (dodgeTarget != null)
		{
			mesh.Rotate(dodgeTarget.angle);
			mesh.Scale(1 + dodgeTarget.lastStep * 1.5f, 1 - dodgeTarget.lastStep * 0.7f);
			mesh.Rotate(-dodgeTarget.angle);
		}

		mesh.Scale(new Vector2(-dir, 1));

		mesh.Color = Color.Black;
		mesh.Draw(occlusionTileset, playerTileset.X, playerTileset.Y);
		mesh.Color = Color;

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
	}
}