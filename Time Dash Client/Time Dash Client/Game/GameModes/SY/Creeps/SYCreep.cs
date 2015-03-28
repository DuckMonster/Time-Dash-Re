using EZUDP;
using OpenTK;
using TKTools;

public class SYCreep : Actor
{
	protected new SYMap Map
	{
		get
		{
			return (SYMap)base.Map;
		}
	}

	public int id;
	protected Vector2 idleTarget;

	protected bool idleTargetReached = false;

	CircleBar healthBar = new CircleBar(2f, 0.5f, 90 + 30f, 30f * 2);

	public SYCreep(int id, Vector2 position, Vector2 velocity, Map map)
		:base(position, map)
	{
		this.id = id;
		this.velocity = velocity;
	}

	public override void Die(Vector2 diePos)
	{
		base.Die(diePos);
		EffectExplosion.CreateExplosion(diePos, 1f, Map);
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
	}

	public void ReceiveHit(float damage, float dir, MessageBuffer msg)
	{
		Map.AddEffect(new EffectEnemyHit(this, dir, damage, Map));

		HitType type = (HitType)msg.ReadByte();

		switch(type)
		{
			case HitType.Bullet:
				int id = msg.ReadByte();

				if (Map.projectileList[id] != null)
					Map.projectileList[id].OnHit(this);
				else
					Log.Write(System.ConsoleColor.Red, "Projectile " + id + " doesn't exist :S");	
				break;

			case HitType.NPC:
				break;
		}

		Hit(damage);
	}

	public void ReceiveIdleTarget(Vector2 target)
	{
		idleTarget = target;
		idleTargetReached = false;
	}

	public virtual void ReachIdleTarget()
	{
		idleTargetReached = true;
	}

	public override void Logic()
	{
		//base.Logic();
		healthBar.Progress = health / MaxHealth;
		healthBar.Logic();
	}

	public override void Draw()
	{
		if (!IsAlive) return;

		mesh.Color = Color.Red;
		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(size);

		mesh.Draw();

		healthBar.Draw(position, Color.Red);
	}
}