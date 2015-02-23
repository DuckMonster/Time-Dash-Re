using OpenTK;

public enum WeaponFireType
{
	Single,
	SingleTimed,
	Auto
}

public enum WeaponList
{
	Pistol,
	Rifle
}

public abstract class Weapon
{
	protected Map map;
	protected Player owner;
	int ammo, maxAmmo;
	WeaponFireType fireType;

	protected float damage;

	public int Ammo
	{
		get { return ammo; }
	}

	public int MaxAmmo
	{
		get { return maxAmmo; }
	}

	public WeaponFireType FireType
	{
		get { return fireType; }
	}

	public bool CanShoot
	{
		get
		{
			return ((fireType == WeaponFireType.Single || shootTimer.IsDone) && ammo > 0);
		}
	}

	Timer reloadTimer = new Timer(1f, false);
	Timer shootTimer = new Timer(0.1f, true);

	public float FireRate
	{
		get { return 1f / shootTimer.TimerLength; }
		set { shootTimer.Reset(1f / value); }
	}

	public float ReloadRate
	{
		get { return 1f / reloadTimer.TimerLength; }
		set { reloadTimer.Reset(1f / value); }
	}

	public Weapon(WeaponFireType fireType, float damage, float fireRate, float reloadRate, int maxAmmo, Player p, Map map)
	{
		owner = p;
		this.map = map;

		this.damage = damage;

		this.maxAmmo = maxAmmo;
		ammo = maxAmmo;

		this.fireType = fireType;
		FireRate = fireRate;
		ReloadRate = reloadRate;
	}

	public abstract Bullet CreateBullet(Vector2 target, int index);

	public void OnShoot()
	{
		ammo--;
		shootTimer.Reset();
	}

	public void Logic()
	{
		if (ammo < maxAmmo)
		{
			reloadTimer.Logic();

			if (reloadTimer.IsDone)
			{
				ammo++;
				reloadTimer.Reset();
			}
		}

		shootTimer.Logic();
	}
}