using OpenTK;
using TKTools;
public class Projectile : Entity
{
	public int id;
	protected Player owner;
	public Player Owner
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

	public Projectile(Player owner, int id, float damage, Map map)
		: base(owner.position, map)
	{
		this.owner = owner;
		this.id = id;
		this.damage = damage;
	}

	public override void Logic()
	{
		position += velocity * Game.delta;
	}

	public virtual void Hit(Vector2 position)
	{
		active = false;
	}

	public virtual void Hit(Actor a)
	{
		Hit(a.position);
		a.Hit(damage, TKMath.GetAngle(velocity), this);
	}
}