public static class WeaponStats
{
	public enum FireType {
		Single,
		SingleTimed,
		Auto
	}

	public struct Stats
	{
		public float damage;
		public FireType fireType;
		public float fireRate;
		public float reloadTime;
		public int ammo;

		public Stats(float damage, FireType fireType, float fireRate, float reloadTime, int ammo)
		{
			this.damage = damage;
			this.fireType = fireType;
			this.fireRate = fireRate;
			this.reloadTime = reloadTime;
			this.ammo = ammo;
		}
	}

	public static readonly Stats
		Pistol = new Stats(1f, FireType.Single, 0f, 2.5f, 6),
		Rifle = new Stats(0.2f, FireType.Auto, 20, 3.4f, 17),
		GrenadeLauncher = new Stats(1.2f, FireType.SingleTimed, 0.6f, 3f, 3);
}