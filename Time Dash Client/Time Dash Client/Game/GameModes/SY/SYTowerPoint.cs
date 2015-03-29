using OpenTK;

public class SYTowerPoint : SYStash
{
	public SYTower tower;

	public SYTowerPoint(int id, Vector2 position, Map m)
		: base(id, 2f, 1, position, m)
	{

	}

	public void Reset()
	{
		scrap = 0;
		tower = null;
	}

	public override void Draw()
	{
		if (tower == null)
			base.Draw();
	}
}