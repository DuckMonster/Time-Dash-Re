using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using TKTools;
using TKTools.Context;
using TKTools.Context.Input;

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

	protected bool updateInput;
	protected PlayerInput inputData = new PlayerInput();
	protected PlayerInput input, oldInput;
	protected Vector2 inputDirection, oldInputDirection, lastDirection;

	protected PlayerShadow shadow;

	float wallStick = -1;
	bool canDoublejump = true;

	DashTarget dashTarget = null;
	DodgeTarget dodgeTarget = null;

	Timer dashCooldown;
	Timer dodgeCooldown;

	public Timer DodgeCooldown
	{
		get { return dodgeCooldown; }
	}

	public Timer DashCooldown
	{
		get { return dashCooldown; }
	}

	float dodgeInterval = 0.2f;
	float dodgeIntervalTimer = 0;

	float dodgeGravityIgnoreTime = 0.1f;
	float gravityIgnore = 0f;

	public Tileset playerTileset = new Tileset(Art.Load("Res/jacktilesetnew.png"), 250, 200),
		occlusionTileset = new Tileset(Art.Load("Res/jackShadowTileset.png"), 250, 200);

	public Sound jumpSound = new Sound(@"Res\Snd\jump.wav"),
		dashSound = new Sound(@"Res\Snd\dash.wav");

	bool bufferFrame = false;

	public PlayerHud hud;

	public int weaponIndex;
	public Weapon[] inventory = new Weapon[2];
	public List<WeaponList> ownedWeapons = new List<WeaponList>();
	public bool OwnsWeapon(WeaponList w) { return ownedWeapons.Contains(w); }

	Mesh playerMesh;

	KeyboardWatch keyboard;
	MouseWatch mouse;

	public int WallTouch
	{
		get
		{
			if (Map.GetCollision(this, new Vector2(-0.1f, 0f)) && velocity.X < 0.1f) return -1;
			if (Map.GetCollision(this, new Vector2(0.1f, 0)) && velocity.X > -0.1f) return 1;
			return 0;
		}
	}

	public virtual Color Color
	{
		get
		{
			if (Team != null) return Team.Color;
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
			return true;
		}
	}
	public bool IgnoresGravity
	{
		get
		{
			return gravityIgnore > 0;
		}
	}

	public Weapon Weapon
	{
		get { return inventory[weaponIndex]; }
	}

	public int Ammo
	{
		get { return Weapon.Ammo; }
	}
	public int MaxAmmo
	{
		get { return Weapon.MaxAmmo; }
	}

	public Player(int id, string name, Vector2 position, Map m)
		: base(position, m)
	{ 
		playerID = id;
		playerName = name;

		keyboard = new KeyboardWatch();
		mouse = new MouseWatch();
		mouse.Perspective = Map.GameCamera;

		float charHeight = 165f;
		float h = (float)playerTileset.Height / charHeight;
		float w = ((float)playerTileset.Width / playerTileset.Height);

		playerMesh = sprite.Mesh;
		playerMesh.Tileset = playerTileset;

		playerMesh.Vertices2 = new Vector2[] {
			new Vector2(-0.5f * w * h, -0.5f + h),
			new Vector2(0.5f * w * h, -0.5f + h),
			new Vector2(0.5f * w * h, -0.5f),
			new Vector2(-0.5f * w * h, - 0.5f)
		};

		playerMesh.UV = new Vector2[] {
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 0)
		};

		shadow = new PlayerShadow(this, playerMesh);

		dashCooldown = new Timer(stats.DashCooldown, false);
		dodgeCooldown = new Timer(stats.DodgeCooldown, true);

		hud = new PlayerHud(this);

		ownedWeapons.Add(WeaponList.Pistol);
	}

	public override void Dispose()
	{
		base.Dispose();
		hud.Dispose();
	}

	public override void Hit(float dmg)
	{
		base.Hit(dmg);
		hud.Hit();
	}

	public override void Die(Vector2 diePos)
	{
		Map.AddEffect(new EffectSkull(diePos, Color, Map));

		dashTarget = null;
		dodgeTarget = null;

		dashCooldown.Reset();

		base.Die(diePos);
	}

	public override void Respawn(Vector2 pos)
	{
		base.Respawn(pos);
		Map.AddEffect(new EffectRing(position, 4f, 1.5f, Color, Map));
	}

	public override void Logic()
	{
		mouse.PlaneDistance = mouse.Perspective.Position.Z;

		hud.Logic();
		if (Weapon != null)
			Weapon.Logic();

		if (!IsAlive) return;

		if (shadow != null && !dashCooldown.IsDone)
		{
			if (!IsDashing) dashCooldown.Logic();

			if (dashCooldown.IsDone && IsLocalPlayer)
				Map.AddEffect(new EffectRing(shadow.CurrentPosition, 1.2f, 0.5f, Color, Map));
		}

		dodgeCooldown.Logic();

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
			if (dodgeCooldown.IsDone && inputDirection != Vector2.Zero)
			{
				if ((inputDirection == lastDirection && dodgeIntervalTimer > 0 && oldInputDirection != inputDirection) || keyboard.KeyPressed(Key.LShift))
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

				if (inputDirection != oldInputDirection)
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
			if (Weapon != null)
			{
				if (input[PlayerKey.Shoot] && !oldInput[PlayerKey.Shoot])
					Weapon.Press();
				if (input[PlayerKey.Shoot] && oldInput[PlayerKey.Shoot])
					Weapon.Hold();
				if (!input[PlayerKey.Shoot] && oldInput[PlayerKey.Shoot])
					Weapon.Release();
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

		bufferFrame = false;
	}

	public void LocalInput()
	{
		if (Map.PauseInput)
		{
			inputData.Reset();
			return;
		}

		if (Program.context.Focused)
		{
			inputData[PlayerKey.Right] = keyboard[Key.D];
			inputData[PlayerKey.Left] = keyboard[Key.A];
		}
		else
		{
			inputData[PlayerKey.Right] = keyboard[Key.Left];
			inputData[PlayerKey.Left] = keyboard[Key.Right];
		}

		inputData[PlayerKey.Up] = keyboard[Key.W];
		inputData[PlayerKey.Down] = keyboard[Key.S];
		inputData[PlayerKey.Jump] = keyboard[Key.Space];
		inputData[PlayerKey.Dash] = mouse[MouseButton.Right];
		inputData[PlayerKey.Shoot] = mouse[MouseButton.Left];

		if (keyboard.KeyPressed(Key.Q))
		{
			SendSwapWeapon();
		}
		if (keyboard.KeyPressed(Key.R))
		{
			Weapon.Reload();
			SendReload();
		}
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

		occlusionTileset.Position = playerTileset.Position;

		playerMesh.FillColor = false;

		playerMesh.Reset();

		playerMesh.Translate(position);
		playerMesh.Scale(size);

		if (dashTarget != null)
		{
			playerMesh.RotateZ(dashTarget.angle);
			playerMesh.Scale(1 + dashTarget.lastStep * 2.4f, 1 - dashTarget.lastStep * 0.5f);
			playerMesh.RotateZ(-dashTarget.angle);
		}

		if (dodgeTarget != null)
		{
			playerMesh.RotateZ(dodgeTarget.stepAngle);
			playerMesh.Scale(1 + dodgeTarget.stepLength * 1.5f, 1 - dodgeTarget.stepLength * 0.7f);
			playerMesh.RotateZ(-dodgeTarget.stepAngle);
		}

		playerMesh.Scale(new Vector2(dir, 1));

		playerMesh.Color = Color.Black;

		playerMesh.Tileset = occlusionTileset;
		playerMesh.Draw();

		playerMesh.Color = Color.White;

		playerMesh.Tileset = playerTileset;
		playerMesh.Draw();

		float glow = Math.Max(1 - dodgeCooldown.PercentageDone, 1 - dashCooldown.PercentageDone);
		if (glow > 0)
		{
			playerMesh.FillColor = true;
			playerMesh.Color = new Color(1, 1, 1, glow);

			playerMesh.Draw();
		}

		if (receivedServerPosition)
		{
			playerMesh.Color = new Color(0, 1, 0, 0.5f);
			playerMesh.FillColor = true;

			playerMesh.Reset();

			playerMesh.Translate(serverPosition);
			playerMesh.Scale(size);
			playerMesh.Scale(new Vector2(-dir, 1));

			playerMesh.Draw();
		}

		//if (shadow != null && IsLocalPlayer && (dashCooldown.IsDone || IsDashing) && !Disabled) shadow.Draw();
		if (IsLocalPlayer && (CanDash || IsDashing)) shadow.Draw();

		hud.Draw();
	}

	public virtual void DrawHUD()
	{
		if (!IsAlive) return;

		hud.DrawHUD();
	}
}