using OpenTK;
using TKTools;

public class KothPlayer : Player
{
	public KothPlayer(int id, string name, Vector2 position, Map m)
		: base(id, name, position, m)
	{

	}

	public override Color Color
	{
		get
		{
			if (((KothMap)map).point.owner == this) return Color.White;
			else return base.Color;
		}
	}
}