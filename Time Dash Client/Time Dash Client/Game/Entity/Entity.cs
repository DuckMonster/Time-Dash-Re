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

	public virtual void ReceivePosition(float x, float y)
	{
		position = new Vector2(x, y);
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