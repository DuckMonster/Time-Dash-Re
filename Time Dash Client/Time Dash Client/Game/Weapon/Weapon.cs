using OpenTK;
using System.Collections.Generic;

public enum WeaponList
{
	Pistol,
	Rifle,
	GrenadeLauncher
}

public abstract class Weapon
{
	protected Map map;
	protected Player owner;
	int ammo, maxAmmo;
	WeaponStats.FireType fireType;

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

	public Weapon(WeaponStats.Stats stats, Player p, Map map)
	{
		owner = p;
		this.map = map;

		maxAmmo = stats.ammo;
		fireType = stats.fireType;
		FireRate = stats.fireRate;
		ReloadTime = stats.reloadTime;

		ammo = maxAmmo;
	}

	public abstract Projectile CreateProjectile(Vector2 target, int index);

	public void OnShoot()
	{
		if (!CanShoot) return;

		ammo--;
		owner.hud.UseAmmo(ammo);

		rearmTimer.Reset();

		if (ammo <= 0) Reload();
	}

	public void Reload()
	{
		if (!reloadTimer.IsDone) return;
		reloadTimer.Reset();
	}

	public void Logic()
	{
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