﻿using EZUDP;
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

	public Tileset playerTileset = new Tileset(250, 200, "Res/jacktilesetnew.png"),
		occlusionTileset = new Tileset(250, 200, "Res/jackShadowTileset.png");

	public Sound jumpSound = new Sound(@"Res\Snd\jump.wav"),
		dashSound = new Sound(@"Res\Snd\dash.wav");

	bool bufferFrame = false;

	public PlayerHud hud;
	public Weapon weapon;

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

	public int Ammo
	{
		get { return weapon.Ammo; }
	}
	public int MaxAmmo
	{
		get { return weapon.MaxAmmo; }
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

		shadow = new PlayerShadow(this, mesh);

		dashCooldown = new Timer(stats.DashCooldown, false);
		dodgeCooldown = new Timer(stats.DodgeCooldown, true);

		hud = new PlayerHud(this);
		EquipWeapon(3);
	}

	public override void Dispose()
	{
		base.Dispose();
		playerTileset.Dispose();

		foreach (Projectile b in projectileList)
			if (b != null) b.Dispose();

		hud.Dispose();
	}

	public void EquipWeapon(int id)
	{
		switch ((WeaponList)id)
		{
			case WeaponList.Pistol: EquipWeapon(new Pistol(this, Map)); break;
			case WeaponList.Rifle: EquipWeapon(new Rifle(this, Map)); break;
			case WeaponList.GrenadeLauncher: EquipWeapon(new GrenadeLauncher(this, Map)); break;
			case WeaponList.Bow: EquipWeapon(new Bow(this, Map)); break;
		}
	}

	public void EquipWeapon(Weapon w)
	{
		if (weapon != null)
			weapon = null;

		weapon = w;
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
		hud.Logic();
		weapon.Logic();

		foreach (Projectile p in projectileList)
			if (p != null) p.Logic();

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
				if ((inputDirection == lastDirection && dodgeIntervalTimer > 0 && oldInputDirection != inputDirection) || KeyboardInput.KeyPressed(Key.LShift))
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
				weapon.Press();
			if (input[PlayerKey.Shoot] && oldInput[PlayerKey.Shoot])
				weapon.Hold();
			if (!input[PlayerKey.Shoot])
				weapon.Release();
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
		inputData[PlayerKey.Dash] = MouseInput.Current[MouseButton.Right];
		inputData[PlayerKey.Shoot] = MouseInput.Current[MouseButton.Left];

		if (KeyboardInput.KeyPressed(Key.Number1)) SendEquipWeapon(0);
		if (KeyboardInput.KeyPressed(Key.Number2)) SendEquipWeapon(1);
		if (KeyboardInput.KeyPressed(Key.Number3)) SendEquipWeapon(2);
		if (KeyboardInput.KeyPressed(Key.Number4)) SendEquipWeapon(3);
		if (KeyboardInput.KeyPressed(Key.R))
		{
			weapon.Reload();
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

		float glow = Math.Max(1 - dodgeCooldown.PercentageDone, 1 - dashCooldown.PercentageDone);
		if (glow > 0)
		{
			mesh.FillColor = true;
			mesh.Color = new Color(1, 1, 1, glow);

			mesh.Draw(playerTileset, playerTileset.X, playerTileset.Y);
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

		foreach (Projectile p in projectileList)
			if (p != null) p.Draw();
	}

	public virtual void DrawHUD()
	{
		if (!IsAlive) return;
		hud.Draw();
	}
}