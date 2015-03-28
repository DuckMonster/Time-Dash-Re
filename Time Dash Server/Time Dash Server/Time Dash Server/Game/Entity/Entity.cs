using OpenTK;
using OpenTK.Input;

public class Entity
{
	Map map;
	protected Map Map
	{
		get
		{
			return map;
		}
	}

	protected Vector2 position, size = new Vector2(0.6f, 0.6f);
	public virtual Vector2 Position
	{
		get { return position; }
		set { position = value; }
	}
	public virtual Vector2 Size
	{
		get { return size; }
		set { size = value; }
	}

	public Entity(Vector2 pos, Map m)
	{
		map = m;
		Position = pos;
	}

	public virtual bool CollidesWith(Vector2 pos, Vector2 s)
	{
		return (pos.X + s.X / 2 >= Position.X - Size.X / 2 &&
			pos.X - s.X / 2 < Position.X + Size.X / 2 &&
			pos.Y + s.Y / 2 >= Position.Y - Size.Y / 2 &&
			pos.Y - s.Y / 2 < Position.Y + Size.Y / 2);
	}

	public virtual bool CollidesWith(Vector2 pos, float radius)
	{
		Vector2 checkpos = new Vector2(
			MathHelper.Clamp(pos.X, Position.X - Size.X / 2, Position.X + Size.X / 2),
			MathHelper.Clamp(pos.Y, Position.Y - Size.Y / 2, Position.Y + Size.Y / 2));

		return (pos - checkpos).Length <= radius;
	}

	public virtual void Logic()
	{
	}
}