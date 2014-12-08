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

	public virtual void ReceivePosition(float x, float y)
	{
		position = new Vector2(x, y);
	}

	public virtual void Logic()
	{
	}
}