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

	public void Logic()
	{
		if (KeyboardInput.Current[Key.Right]) position.X += 5f * Game.delta;
		if (KeyboardInput.Current[Key.Left]) position.X -= 5f * Game.delta;
		if (KeyboardInput.Current[Key.Down]) position.Y -= 5f * Game.delta;
		if (KeyboardInput.Current[Key.Up]) position.Y += 5f * Game.delta;
	}

	public void Draw()
	{
		mesh.Color = map.GetCollision(this) ? Color.Green : Color.Blue;

		mesh.Reset();
		mesh.Scale(size);
		mesh.Translate(position);
		mesh.Draw();
	}
}