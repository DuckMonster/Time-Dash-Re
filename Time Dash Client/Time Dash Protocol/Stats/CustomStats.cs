using System.Collections.Generic;

public class CustomStats
{
	Dictionary<string, object> stats = new Dictionary<string, object>();

	public CustomStats(CustomStats par, params object[] objects)
	{
		foreach (KeyValuePair<string, object> p in par.stats)
			stats.Add(p.Key, p.Value);

		for(int i=0; i<objects.Length; i+=2)
		{
			string key = objects[i] as string;

			if (stats.ContainsKey(key))
				stats[key] = objects[i + 1];
			else
				stats.Add(key, objects[i + 1]);
		}
	}
	public CustomStats(params object[] objects)
	{
		for (int i = 0; i < objects.Length; i += 2)
		{
			stats.Add(objects[i] as string, objects[i+1]);
		}
	}

	public T GetStat<T>(string key)
	{
		return (T)stats[key];
	}
}

public enum CreepType
{
	Scroot
}

public static class CreepStats
{
	public static readonly CustomStats Creep = new CustomStats(
		"name", "*CREEP*",
		"health", 1.5f,
		"impactDamage", 1.8f,
		"impactForce", 2f
		);

	public static readonly CustomStats Scroot = new CustomStats(
		Creep,
		"name", "Scroot",
		"type", CreepType.Scroot,
		"idleSpeed", 1.2f,
		"chaseSpeed", 7f,
		"accelerationTime", 4f,
		"chargeTime", 0.8f,
		"projectileDamage", 1.4f,
		"recoil", 3f,

		"reloadMin", 1.5f,
		"reloadMax", 2.5f
		);
}