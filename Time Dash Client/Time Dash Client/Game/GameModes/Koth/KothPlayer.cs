using OpenTK;
using TKTools;

public class KothPlayer : Player
{
	protected new KothMap Map
	{
		get
		{
			return (KothMap)base.Map;
		}
	}

	public KothPlayer(int id, string name, Vector2 position, Map m)
		: base(id, name, position, m)
	{

	}

	public override Color Color
	{
		get
		{
			if (Map.point.owner == this) return Color.White;
			else return base.Color;
		}
	}
}