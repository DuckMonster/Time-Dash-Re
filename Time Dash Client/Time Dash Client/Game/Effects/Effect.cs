using System;

public class Effect : IDisposable
{
	Map map;

	public Effect(Map m)
	{
		map = m;
	}

	public void Remove()
	{
		map.RemoveEffect(this);
	}

	public virtual void Dispose()
	{
	}

	public virtual void Logic()
	{
	}

	public virtual void Draw()
	{
	}
}