using OpenTK;
using System.Collections.Generic;
using TKTools;

public class KothPoint : Scoreboard
{
	public Player owner = null;
	public float radius = 7f;

	Texture circle = new Texture("Res/circleBig.png");

	Timer scoreTimer = new Timer(0.7f, false);

	public KothPoint(Vector2 position, Map map)
		: base(40, 6f, 6f, position, map)
	{
		mesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);

		mesh.Vertices = new Vector2[] {
			new Vector2(-0.5f, 0.5f),
			new Vector2(0.5f, 0.5f),
			new Vector2(-0.5f, 0f),
			new Vector2(0.5f, 0f)
		};

		mesh.UV = new Vector2[] {
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(0, 0.5f),
			new Vector2(1, 0.5f)
		};

		mesh.Texture = circle;

		mesh.Translate(position);
		mesh.Scale(radius * 2);
	}

	public override bool IsLeader(Player p)
	{
		return owner == p;
	}

	public override void Dispose()
	{
		base.Dispose();
		circle.Dispose();
	}

	void LookForPlayer()
	{
		List<Player> playerList = map.GetPlayerRadius(position, radius);

		Player contender = null;
		foreach (Player p in playerList)
		{
			if (p.position.Y < position.Y) continue;

			if (contender == null) contender = p;
			else
			{
				owner = null;
				return;
			}
		}

		if (contender != null) owner = contender;
		else owner = null;
	}

	public override void Logic()
	{
		base.Logic();
		LookForPlayer();

		if (owner != null)
		{
			scoreTimer.Logic();

			if (scoreTimer.IsDone)
				scoreTimer.Reset();
		}
	}

	public override void Draw()
	{
		mesh.Color = (owner == null ? Color.Gray : Player.colorList[owner.playerID]) * new Color(1f, 1f, 1f, 0.4f);
		mesh.Draw();

		base.Draw();
	}
}