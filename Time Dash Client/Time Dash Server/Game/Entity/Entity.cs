using OpenTK;
using OpenTK.Input;

public class Entity
{
	protected Map map;
	public Vector2 position, size = new Vector2(0.6f, 0.8f);

	public Entity(Vector2 pos, Map m)
	{
		map = m;
		position = pos;
	}

	public bool CollidesWith(Vector2 pos, Vector2 s)
	{
		return (pos.X + s.X / 2 >= position.X - size.X / 2 &&
			pos.X - s.X / 2 < position.X + size.X / 2 &&
			pos.Y + s.Y / 2 >= position.Y - size.Y / 2 &&
			pos.Y - s.Y / 2 < position.Y + size.Y / 2);
	}

	public virtual void Logic()
	{
	}
}