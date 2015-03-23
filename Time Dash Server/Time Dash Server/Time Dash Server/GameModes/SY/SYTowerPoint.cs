using System;
using OpenTK;

public class SYTowerPoint : SYStash
{
	SYTower tower;

	public SYTowerPoint(int id, Vector2 position, Map m)
		:base(id, 2f, 1, position, m)
	{

	}

	public void Reset()
	{
		scrap = 0;
	}

	public override void Finish()
	{
		tower = Map.SpawnTower(this);
	}
}