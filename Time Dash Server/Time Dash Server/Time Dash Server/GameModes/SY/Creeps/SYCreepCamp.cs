using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;

public class SYCreepCamp : Entity
{
	protected new SYMap Map
	{
		get { return (SYMap)base.Map; }
	}

	Random rng = new Random();

	List<SYCreep> creepList = new List<SYCreep>();
	RectangleF rectangle;

	Timer spawnTimer = new Timer(5f, false);

	public Vector2 RandomPosition
	{
		get
		{
			Vector2 pos;

			do
			{
				pos = new Vector2(
					rectangle.X + (float)rng.NextDouble() * rectangle.Width,
					rectangle.Y + (float)rng.NextDouble() * rectangle.Height
					);
			} while (Map.GetCollision(pos, new Vector2(1, 1)) || Map.GetActorAtPos<Actor>(pos, new Vector2(1, 1)) != null);

			return pos;
		}
	}

	public SYCreepCamp(RectangleF rect, Map map)
		:base(Vector2.Zero, map)
	{
		rectangle = rect;

		Position = new Vector2(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2);
		Size = new Vector2(rectangle.Width, rectangle.Height);
	}

	public void SpawnCreeps()
	{
		creepList.Clear();

		for (int i = 0; i < 4; i++)
		{
			creepList.Add(Map.SpawnEnemy(RandomPosition, this));
		}
	}

	public override void Logic()
	{
		if (spawnTimer.IsDone)
		{
			if (creepList.TrueForAll(x => !x.IsAlive))
				spawnTimer.Reset();
		}
		else
		{
			spawnTimer.Logic();
			if (spawnTimer.IsDone)
				SpawnCreeps();
		}
	}
}