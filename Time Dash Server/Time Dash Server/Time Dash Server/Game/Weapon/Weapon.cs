using OpenTK;

public abstract class Weapon
{
	public readonly WeaponList type;

	protected Map map;
	protected Player owner;
	int ammo, maxAmmo;
	WeaponStats.FireType fireType;

	protected float damage;

	float maxCharge;
	float charge = 0f;

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
			return ((fireType == WeaponStats.FireType.Single || shootTimer.IsDone) && ammo > 0);
		}
	}

	public float Charge
	{ 
		get { return charge / maxCharge; }
		set { charge = value * maxCharge; }
	}

	Timer reloadTimer = new Timer(1f, false);
	Timer shootTimer = new Timer(0.1f, true);

	public float FireRate
	{
		get { return 1f / shootTimer.TimerLength; }
		set { shootTimer.Reset(1f / value); }
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

		damage = stats.damage;
		fireType = stats.fireType;
		FireRate = stats.fireRate;
		maxCharge = stats.fireRate;
		ReloadTime = stats.reloadTime;
		maxAmmo = stats.ammo;

		ammo = maxAmmo;
	}

	public abstract Projectile CreateProjectile(Vector2 target);

	public void OnShoot()
	{
		ammo--;
		shootTimer.Reset();

		if (ammo <= 0) Reload();
	}

	public void Logic()
	{
		if (!reloadTimer.IsDone)
		{
			reloadTimer.Logic();
			if (reloadTimer.IsDone) ammo = MaxAmmo;
		}

		shootTimer.Logic();
	}

	public void Reload()
	{
		reloadTimer.Reset();
	}
}