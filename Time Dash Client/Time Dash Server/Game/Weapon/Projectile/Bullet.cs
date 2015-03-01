using OpenTK;
using System.Collections.Generic;
using TKTools;

public class Bullet : Projectile
{
	Vector2 directionVector;

	public float Direction
	{
		get
		{
			return TKMath.GetAngle(directionVector);
		}
	}

	public Bullet(Player p, int id, float damage, Vector2 target, Map m)
		: base(p, id, damage, m)
	{
		directionVector = (target - p.position).Normalized();
		size = new Vector2(0.1f, 0.1f);
	}

	public override void Logic()
	{
		if (!Active) return;
		
		base.Logic();

		Vector2 stepVector = directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		if (Map.GetCollision(this, stepVector))
			Hit(position + stepVector);
		else position += directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		List<Player> playersHit = Map.GetPlayerRadius(position, 0.2f, owner);

		if (playersHit.Count > 0)
			Hit(playersHit[0]);
	}
}