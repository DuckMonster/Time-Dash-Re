using OpenTK;

public class Projectile : Entity
{
	public int id;
	protected Player owner;
	protected Vector2 velocity;

	bool active = true;

	public bool Active
	{
		get { return active; }
	}

	public Projectile(Player owner, int id, Map map)
		: base(owner.position, map)
	{
		this.owner = owner;
		this.id = id;
	}

	public override void Logic()
	{
		if (!Active) return;

		position += velocity * Game.delta;
	}

	public virtual void Hit()
	{
		active = false;
	}

	public override void Draw()
	{
		if (!Active) return;

		mesh.Draw();
	}
}