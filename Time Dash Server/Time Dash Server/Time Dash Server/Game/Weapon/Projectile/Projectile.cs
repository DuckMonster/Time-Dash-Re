using EZUDP;
using OpenTK;
using TKTools;
public abstract class Projectile : Entity
{
	public int id = -1;
	protected Actor owner;
	public Actor Owner
	{
		get { return owner; }
	}

	protected Vector2 velocity;

	bool active = true;
	public bool Active
	{
		get { return active; }
	}

	float damage;
	public float Damage
	{
		get { return damage; }
	}

	public Projectile(Actor owner, Vector2 position, float damage, Map map)
		: base(position, map)
	{
		this.owner = owner;
		this.damage = damage;
		id = map.AddProjectile(this);
	}

	public override void Logic()
	{
		Position += velocity * Game.delta;
	}

	public virtual void Hit(Vector2 position)
	{
		active = false;
	}

	public virtual void Hit(Actor a)
	{
		Hit(a.Position);
		a.Hit(damage, TKMath.GetAngle(velocity), this);
	}
}