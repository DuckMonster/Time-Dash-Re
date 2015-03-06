public enum WeaponList
{
	Pistol,
	Rifle,
	GrenadeLauncher,
	Bow
}

public static class WeaponStats
{
	public enum FireType
	{
		Single,
		SingleTimed,
		Auto,
		Charge
	}

	public struct Stats
	{
		public int id;
		public float damage;
		public FireType fireType;
		public float fireRate;
		public float reloadTime;
		public int ammo;

		public Stats(int id, float damage, FireType fireType, float fireRate, float reloadTime, int ammo)
		{
			this.id = id;
			this.damage = damage;
			this.fireType = fireType;
			this.fireRate = fireRate;
			this.reloadTime = reloadTime;
			this.ammo = ammo;
		}
	}

	public static readonly Stats
		//Stats(Damage, Fire type, Fire rate, Reload time, Ammo
		Pistol = new Stats((int)WeaponList.Pistol, 1f, FireType.Single, 0f, 2.5f, 6),
		Rifle = new Stats((int)WeaponList.Rifle, 0.4f, FireType.Auto, 12, 3f, 30),
		GrenadeLauncher = new Stats((int)WeaponList.GrenadeLauncher, 2.4f, FireType.SingleTimed, 1.3f, 3.5f, 3),
		Bow = new Stats((int)WeaponList.Bow, 2f, FireType.Charge, 0.8f, 0f, 1);
}