using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;

public class Bullet : Entity
{
	Player owner;
	public int id;

	Vector2 directionVector;
	float direction;

	bool active = true;

	public Bullet(Player p, int id, Vector2 target, Map m)
		: base(p.position, m)
	{
		owner = p;
		this.id = id;
		directionVector = (target - position).Normalized();
		direction = TKMath.GetAngle(directionVector);

		size = new Vector2(0.1f, 0.1f);
	}

	public override void Logic()
	{
		if (!active) return;

		base.Logic();

		Vector2 stepVector = directionVector * Stats.defaultStats.BulletVelocity * Game.delta;

		if (map.GetCollision(this, stepVector))
		{
			Vector2 collidePos;
			map.RayTraceCollision(position, position + stepVector, new Vector2(0.1f, 0.1f), out collidePos);
			position = collidePos;

			Hit(null);
		} else position += directionVector * Stats.defaultStats.BulletVelocity * Game.delta;


		List<Player> playersHit = map.GetPlayerRadius(position, 0.2f, owner);

		if (playersHit.Count > 0)
			Hit(playersHit[0]);
	}

	public void Hit(Player player)
	{
		if (player == null)
		{
			map.AddEffect(new EffectSmokeCone(position, direction - 180, map));
			map.AddEffect(new EffectRing(position, 5f, 0.9f, Color.White, map));

			Random rng = new Random();

			float dir = ((float)rng.NextDouble() - 0.5f) * 120f;
			map.AddEffect(new EffectRockSmoke(position, direction - 180 + dir, map));
		}
		else
		{
			map.AddEffect(new EffectRing(player.position, 5f, 0.9f, player.Color, map));
		}

		active = false;
	}

	public override void Draw()
	{
		if (!active) return;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Rotate(direction);
		mesh.Scale(2f, 0.2f);

		mesh.Draw();
	}
}