﻿using OpenTK;
using OpenTK.Input;
using System;
using TKTools;
using TKTools.Context;

public class Entity : IDisposable
{
	Map map;
	protected virtual Map Map
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
	}
	public virtual Vector2 Size
	{
		get { return size; }
	}

	public Sprite sprite;

	public Entity(Vector2 pos, Map m)
	{
		map = m;
		position = pos;

		sprite = new Sprite();
	}

	public virtual void Dispose()
	{
		sprite.Dispose();
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
		sprite.Color = map.GetCollision(this) ? Color.Green : Color.Blue;

		sprite.Draw(position, size, 0f);
	}
}