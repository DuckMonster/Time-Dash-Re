using OpenTK;
using TKTools;

public class DMPlayer : Player
{
	public override Color Color
	{
		get
		{
			//if (((DMMap)map).scoreboard.IsLeader(this)) return Color.White;
			return base.Color;
		}
	}

	public DMPlayer(int id, string name, Vector2 position, Map m)
		: base(id, name, position, m)
	{
	}
}