public enum WeaponList
{
	Pistol,
	Rifle,
	GrenadeLauncher,
	Bow
}

public static class WeaponStats
{
	public static Stats GetStats(WeaponList weapon)
	{
		switch(weapon)
		{
			case WeaponList.Pistol: return Pistol;
			case WeaponList.Rifle: return Rifle;
			case WeaponList.GrenadeLauncher: return GrenadeLauncher;
			case WeaponList.Bow: return Bow;
			default: return Pistol;
		}		
	}

	public enum FireType
	{
		Single,
		SingleTimed,
		Auto,
		Charge
	}

	public struct Stats
	{
		public WeaponList type;
		public float damage;
		public FireType fireType;
		public float fireRate;
		public float reloadTime;
		public int ammo;

		public int scrapCost;

		public Stats(WeaponList type, float damage, FireType fireType, float fireRate, float reloadTime, int ammo, int scrapCost)
		{
			this.type = type;
			this.damage = damage;
			this.fireType = fireType;
			this.fireRate = fireRate;
			this.reloadTime = reloadTime;
			this.ammo = ammo;
			this.scrapCost = scrapCost;
		}
	}

	public static readonly Stats
		//Stats(Damage, Fire type, Fire rate, Reload time, Ammo
		Pistol = new Stats(WeaponList.Pistol, 1f, FireType.Single, 0f, 2.5f, 6, 2),
		Rifle = new Stats(WeaponList.Rifle, 0.4f, FireType.Auto, 12, 3f, 30, 3),
		GrenadeLauncher = new Stats(WeaponList.GrenadeLauncher, 2.4f, FireType.SingleTimed, 1.3f, 3.5f, 3, 5),
		Bow = new Stats(WeaponList.Bow, 2f, FireType.Charge, 0.8f, 0f, 1, 4);
}