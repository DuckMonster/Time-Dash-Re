using OpenTK;
using OpenTK.Input;
using System;
using TKTools;

public class Entity : IDisposable
{
	protected Map map;
	public Vector2 position, size = new Vector2(0.6f, 0.6f);

	protected Mesh mesh;

	public Entity(Vector2 pos, Map m)
	{
		map = m;
		position = pos;

		mesh = Mesh.Box;
	}

	public virtual void Dispose()
	{
		mesh.Dispose();
	}

	public virtual bool CollidesWith(Vector2 pos, Vector2 s)
	{
		return (pos.X + s.X / 2 >= position.X - size.X / 2 &&
			pos.X - s.X / 2 < position.X + size.X / 2 &&
			pos.Y + s.Y / 2 >= position.Y - size.Y / 2 &&
			pos.Y - s.Y / 2 < position.Y + size.Y / 2);
	}

	public virtual bool CollidesWith(Vector2 pos, float radius)
	{
		Vector2 checkpos = new Vector2(
			MathHelper.Clamp(pos.X, position.X - size.X / 2, position.X + size.X / 2),
			MathHelper.Clamp(pos.Y, position.Y - size.Y / 2, position.Y + size.Y / 2));

		return (pos - checkpos).Length <= radius;
		/* 
		Vector2 sizex = new Vector2(size.X/2, 0),
			sizey = new Vector2(0, size.Y/2);

		return
			((position + sizex) - pos).Length <= radius ||
			((position - sizex) - pos).Length <= radius ||
			((position + sizey) - pos).Length <= radius ||
			((position - sizey) - pos).Length <= radius ||
			((position + sizex + sizey) - pos).Length <= radius ||
			((position - sizex + sizey) - pos).Length <= radius ||
			((position + sizex - sizey) - pos).Length <= radius ||
			((position - sizex - sizey) - pos).Length <= radius;*/
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