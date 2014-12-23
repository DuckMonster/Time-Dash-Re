using OpenTK;
using OpenTK.Input;
using TKTools;

public class Entity
{
	protected Map map;
	public Vector2 position, size = new Vector2(0.6f, 0.8f);

	protected Mesh mesh;

	public Entity(Vector2 pos, Map m)
	{
		map = m;
		position = pos;

		mesh = Mesh.Box;
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

	public virtual void Draw()
	{
		mesh.Color = map.GetCollision(this) ? Color.Green : Color.Blue;

		mesh.Reset();
		mesh.Scale(size);
		mesh.Translate(position);
		mesh.Draw();
	}
}