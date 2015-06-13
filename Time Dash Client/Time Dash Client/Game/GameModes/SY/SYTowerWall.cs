using OpenTK;
using System.Drawing;
using TKTools;

public class SYTowerWall : Actor
{
	protected new SYMap Map
	{
		get { return base.Map as SYMap; }
	}

	bool enabled = true;
	int towerID;

	public SYTower Tower
	{
		get
		{
			return Map.towerList[towerID];
		}
	}

	public override Team Team
	{
		get
		{
			if (Tower == null) return null;
			return Tower.Team;
		}

		set
		{
			base.Team = value;
		}
	}

	TKTools.Color Color
	{
		get {
			if (Team != null)
				return Team.Color * new TKTools.Color(1f, 1f, 1f, 0.2f);
			else
				return new TKTools.Color(0, 0, 0, 0);
		}
	}

	public SYTowerWall(Polygon p, int towerID, Map m)
		:base(Vector2.Zero, m)
	{
		this.towerID = towerID;

		RectangleF rect = p.Bounds;
		position = new Vector2(rect.X, rect.Y) + new Vector2(rect.Width / 2, rect.Height / 2);
		size = new Vector2(rect.Width, rect.Height);
	}

	public override bool CollidesWith(Vector2 pos, float radius)
	{
		if (!enabled) return false;
		else return base.CollidesWith(pos, radius);
	}

	public override bool CollidesWith(Vector2 pos, Vector2 s)
	{
		if (!enabled) return false;
		else return base.CollidesWith(pos, s);
	}

	public override void Logic()
	{
		if (enabled && (Tower == null || !Tower.IsAlive))
			enabled = false;
	}

	public override void Draw()
	{
		sprite.Color = Color;
		sprite.Draw(Position, Size, 0f);
	}
}