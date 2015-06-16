using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;
using TKTools.Context.Input;

public abstract class Weapon
{
	static Texture[] weaponIcons;
	public static Texture GetIcon(WeaponList weapon)
	{
		if (weaponIcons == null)
		{
			weaponIcons = new Texture[Enum.GetNames(typeof(WeaponList)).Length];
			weaponIcons[(int)WeaponList.Pistol] = Art.Load("Res/weapons/pistol.png");
			weaponIcons[(int)WeaponList.Rifle] = Art.Load("Res/weapons/rifle.png");
			weaponIcons[(int)WeaponList.GrenadeLauncher] = Art.Load("Res/weapons/grenadeLauncher.png");
			weaponIcons[(int)WeaponList.Bow] = Art.Load("Res/weapons/bow.png");
		}

		return weaponIcons[(int)weapon];
	}

	public readonly WeaponList type;

	protected Map map;
	protected Player owner;
	int ammo, maxAmmo;
	WeaponStats.FireType fireType;
	float maxCharge;
	float charge = 0f;

	bool releasable = false;

	Timer overChargeTimer = new Timer(0.5f, false);
	MouseWatch mouse = new MouseWatch();

	public int Ammo
	{
		get { return ammo; }
	}

	public int MaxAmmo
	{
		get { return maxAmmo; }
	}

	public WeaponStats.FireType FireType
	{
		get { return fireType; }
	}

	public bool CanShoot
	{
		get
		{
			return ((fireType == WeaponStats.FireType.Single || rearmTimer.IsDone) && ammo > 0 && reloadTimer.IsDone);
		}
	}

	public float ReloadProgress
	{
		get
		{
			if (reloadTimer.IsDone) return -1;
			else return reloadTimer.PercentageDone;
		}
	}

	public float RearmProgress
	{
		get
		{
			if (rearmTimer.IsDone) return -1;
			else return rearmTimer.PercentageDone;
		}
	}

	public float Charge
	{
		get { return charge / maxCharge; }
		set { charge = value; }
	}

	Timer reloadTimer = new Timer(1f, true);
	Timer rearmTimer = new Timer(0.1f, true);

	public float FireRate
	{
		get { return 1f / rearmTimer.TimerLength; }
		set { rearmTimer.Reset(1f / value); }
	}

	public float ReloadTime
	{
		get { return reloadTimer.TimerLength; }
		set { reloadTimer.Reset(value); }
	}

	public Weapon(WeaponList weapon, Player p, Map map)
	{
		owner = p;
		this.map = map;

		WeaponStats.Stats stats = WeaponStats.GetStats(weapon);

		type = stats.type;

		maxAmmo = stats.ammo;
		fireType = stats.fireType;
		FireRate = stats.fireRate;
		maxCharge = stats.fireRate;
		ReloadTime = stats.reloadTime;

		ammo = maxAmmo;

		reloadTimer.IsDone = true;

		mouse.Perspective = Map.GameCamera;
	}

	public abstract Projectile CreateProjectile(Vector2 target, int id);

	void TryShoot()
	{
		if (!CanShoot) return;

		if (fireType == WeaponStats.FireType.Charge)
			owner.SendShoot(mouse.Position.Xy, charge);
		else
			owner.SendShoot(mouse.Position.Xy);

		OnShoot();
	}

	public void OnShoot()
	{
		ammo--;
		owner.hud.UseAmmo(ammo);
		rearmTimer.Reset();
		charge = 0;

		if (ammo <= 0) Reload();
	}

	public void Reload()
	{
		if (!reloadTimer.IsDone) return;
		reloadTimer.Reset();

		if (reloadTimer.IsDone)
			ammo = MaxAmmo;
	}

	public void Press()
	{
		if ((fireType == WeaponStats.FireType.Single || fireType == WeaponStats.FireType.SingleTimed)
			&& CanShoot)
			TryShoot();

		releasable = true;
	}

	public void Hold()
	{
		if (!releasable)
			return;

		if (fireType == WeaponStats.FireType.Auto && CanShoot)
			TryShoot();
		else if (fireType == WeaponStats.FireType.Charge)
		{
			charge += Game.delta;
			if (charge > maxCharge)
			{
				charge = maxCharge;
				overChargeTimer.Logic();
				if (overChargeTimer.IsDone)
					Release();
			}
		}
	}

	public void Release()
	{
		if (!releasable) return;

		if (fireType == WeaponStats.FireType.Charge)
			TryShoot();

		charge = 0;
		overChargeTimer.Reset();

		releasable = false;
	}

	public void Logic()
	{
		mouse.PlaneDistance = mouse.Perspective.Position.Z;

		if (!reloadTimer.IsDone)
		{
			reloadTimer.Logic();

			if (reloadTimer.IsDone)
			{
				ammo = MaxAmmo;
				owner.hud.OnReload();
			}
		}

		rearmTimer.Logic();
	}

	public void Draw()
	{

	}
}