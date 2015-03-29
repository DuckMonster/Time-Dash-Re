using OpenTK;

public class Projectile : Entity
{
	public int id;
	protected Actor owner;
	protected Vector2 velocity;

	bool active = true;

	public bool Active
	{
		get { return active; }
	}

	public Projectile(Actor owner, int id, Vector2 position, Map map)
		: base(position, map)
	{
		this.owner = owner;
		this.id = id;

		Map.AddProjectile(this);

		Logic();
	}

	public override void Logic()
	{
		if (!Active) return;

		position += velocity * Game.delta;
	}

	public virtual void OnHit(Actor a, Vector2 hitpos)
	{
		position = hitpos;
		Hit();
	}

	public virtual void Hit()
	{
		active = false;
		Map.RemoveProjectile(this);
	}

	public override void Draw()
	{
		if (!Active) return;

		mesh.Reset();

		mesh.Translate(Position);
		mesh.Scale(Size);

		mesh.Draw();
	}
}