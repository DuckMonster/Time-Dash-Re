using OpenTK;
using System;
using TKTools.Mathematics;

public class SYBase : Actor
{
	CircleBar healthBar = new CircleBar(7f, 2f, 90f + 80f, -80f*2);

	public override float MaxHealth
	{
		get
		{
			return 2f;
		}
	}

	Timer dieTimer = new Timer(5f, false);
	Timer dieExplosionTimer = new Timer(0.4f, true);

	public SYBase(Vector2 position, int teamID, Map map)
		:base(position, map)
	{
		this.teamID = teamID;
		this.size = new Vector2(4f, 4f);

		sprite.Texture = Art.Load("Res/circlebig.png");

		sprite.Position = new Vector3(Position);
		sprite.Scale = size;

		sprite.Color = Player.colorList[teamID];
	}

	public void ReceiveHit(float dmg, float dir, int projID, Vector2 hitPos)
	{
		Hit(dmg);

		if (Map.projectileList[projID] != null)
			Map.projectileList[projID].OnHit(this, hitPos);

		Map.AddEffect(new EffectBaseHit(this, dmg, Map));
		Map.AddEffect(new EffectImpactQuad(hitPos, dmg, Map));
	}

	public override void Die(Vector2 diePos)
	{
		base.Die(diePos);
	}

	void CreateDieExplosion()
	{
		Random rng = new Random();
		Vector2 position = TKMath.GetAngleVector((float)rng.NextDouble() * 360f) * ((float)rng.NextDouble() * size.X * 0.5f);

		EffectExplosion.CreateExplosion(Position + position, 0.8f, Map);
	}

	public override void Logic()
	{
		if (!IsAlive)
		{
			if (!dieTimer.IsDone)
			{
				dieTimer.Logic();
				dieExplosionTimer.Logic();

				if (dieExplosionTimer.IsDone)
				{
					CreateDieExplosion();
					dieExplosionTimer.Reset();
				}

				if (dieTimer.IsDone)
				{
					EffectExplosion.CreateExplosion(Position, 5f, Map);
				}
			}

			return;
		}

		healthBar.Progress = health / MaxHealth;
		healthBar.Logic();
	}

	public override void Draw()
	{
		if (!IsAlive && dieTimer.IsDone) return;

		//base.Draw();
		sprite.Draw();
		healthBar.Draw(position, Player.colorList[teamID]);
	}
}