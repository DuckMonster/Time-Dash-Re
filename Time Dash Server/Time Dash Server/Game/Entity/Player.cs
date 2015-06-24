using OpenTK;
using System;

using EZUDP.Server;
using System.Collections.Generic;

public partial class Player : Actor
{
	PlayerInput inputData = new PlayerInput();
	PlayerInput input, oldInput;
	protected Vector2 inputDirection, oldInputDirection, lastDirection;

	public int id;
	public string playerName;
	public Client client;

	protected PlayerShadow shadow;

	float wallStick = -1;
	bool canDoublejump = true;

	DashTarget dashTarget = null;
	DodgeTarget dodgeTarget = null;

	Timer dashCooldown;
	Timer dodgeCooldown;

	float dodgeIntervalTimer = 0;

	float dodgeGravityIgnoreTime = 0.1f;
	float gravityIgnore = 0f;

	Timer updatePositionTimer = new Timer(0.05f, true);
	Timer respawnTimer = new Timer(2f, false);

	int weaponIndex = 0;
	Weapon[] inventory = new Weapon[2];

	protected List<WeaponList> ownedWeapons = new List<WeaponList>();

	public Weapon Weapon
	{
		get { return inventory[weaponIndex]; }
	}

	public int WallTouch
	{
		get
		{
			if (Map.GetCollision(this, new Vector2(-0.1f, 0f)) && velocity.X < 0.1f) return -1;
			if (Map.GetCollision(this, new Vector2(0.1f, 0)) && velocity.X > -0.1f) return 1;
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
			return true;
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

	public int Ammo
	{
		get { return Weapon.Ammo; }
	}
	public int MaxAmmo
	{
		get { return Weapon.MaxAmmo; }
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

		ownedWeapons.Add(WeaponList.Pistol);

		size = size * 1.4f;
	}

	public virtual bool AlliedWith(Player p)
	{
		if (p.Team == null || Team == null) return false;
		else return p.Team == Team;
	}

	public virtual void BuyWeapon(WeaponList weapon)
	{
		if (ownedWeapons.Contains(weapon)) return;

		ownedWeapons.Add(weapon);
		SendBuyWeaponToPlayer(weapon, Map.playerList);
		for (int i = 0; i < inventory.Length; i++)
			if (inventory[i] == null)
			{
				EquipWeapon(i, weapon);
				break;
			}
	}

	public void EquipWeapon(int inventoryID, WeaponList weapon)
	{
		if (!ownedWeapons.Contains(weapon)) return;

		switch (weapon)
		{
			case WeaponList.Pistol: EquipWeapon(inventoryID, new Pistol(this, Map)); break;
			case WeaponList.Rifle: EquipWeapon(inventoryID, new Rifle(this, Map)); break;
			case WeaponList.GrenadeLauncher: EquipWeapon(inventoryID, new GrenadeLauncher(this, Map)); break;
			case WeaponList.Bow: EquipWeapon(inventoryID, new Bow(this, Map)); break;
			default: return;
		}

		SendInventoryToPlayer(inventoryID, weapon, Map.playerList);
	}

	public void EquipWeapon(int inventoryID, Weapon w)
	{
		if (inventory[inventoryID] != null)
			inventory[inventoryID] = null;

		inventory[inventoryID] = w;
	}

	public override void Hit(float dmg, float dir, Actor a, float force = 0)
	{
		base.Hit(dmg, dir, a, force);
		SendHitToPlayer(dmg, dir, Map.playerList);

		if (!IsAlive && a is Player)
			(a as Player).OnKill(this);

		if (force != 0)
			SendPositionToPlayerForce(Map.playerList);
	}

	public override void Hit(float dmg, float dir, Projectile proj, float force = 0)
	{
		base.Hit(dmg, dir, proj, force);
		SendHitToPlayer(dmg, dir, proj, Map.playerList);

		if (!IsAlive && proj.Owner is Player)
			(proj.Owner as Player).OnKill(this);

		if (force != 0)
			SendPositionToPlayerForce(Map.playerList);
	}

	public override void Hit(float dmg)
	{
		base.Hit(dmg);

		dashTarget = null;
		dodgeTarget = null;
	}

	public override void Die()
	{
		base.Die();

		SendDieToPlayer(Map.playerList);
	}

	public virtual void OnKill(Player p)
	{
	}

	public override void Respawn()
	{
		base.Respawn();
		Position = Map.GetFreeSpawnPosition(this);
		SendRespawnToPlayer(Position, Map.playerList);
	}

	public override void Logic()
	{
		if (Weapon != null)
			Weapon.Logic();

		Log.Debug(Position);

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

			SendServerPositionToPlayer(Map.playerList);

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