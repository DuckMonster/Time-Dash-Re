using OpenTK;
using System.Collections.Generic;

public abstract class Weapon
{
	public readonly int weaponID;

	protected Map map;
	protected Player owner;
	int ammo, maxAmmo;
	WeaponStats.FireType fireType;
	float maxCharge;
	float charge = 0f;

	bool releasable = false;

	Timer overChargeTimer = new Timer(0.5f, false);

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
		set { charge = value * maxCharge; }
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

		weaponID = stats.id;

		maxAmmo = stats.ammo;
		fireType = stats.fireType;
		FireRate = stats.fireRate;
		maxCharge = stats.fireRate;
		ReloadTime = stats.reloadTime;

		ammo = maxAmmo;

		reloadTimer.IsDone = true;
	}

	public abstract Projectile CreateProjectile(Vector2 target, int id);

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

		if (reloadTimer.IsDone)
			ammo = MaxAmmo;
	}

	public void Press()
	{
		if ((fireType == WeaponStats.FireType.Single || fireType == WeaponStats.FireType.SingleTimed)
			&& CanShoot)
			owner.SendShoot(MouseInput.Current.Position);

		releasable = true;
	}

	public void Hold()
	{
		if (!releasable)
			return;

		if (fireType == WeaponStats.FireType.Auto && CanShoot)
			owner.SendShoot(MouseInput.Current.Position);
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
			owner.SendShoot(MouseInput.Current.Position, charge);

		charge = 0;
		overChargeTimer.Reset();

		releasable = false;
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