using OpenTK;

public class SYBase : Entity
{
	Team team;

	public SYBase(Vector2 position, Team team, Map map)
		:base(position, map)
	{
		this.team = team;

		mesh.Texture = Art.Load("Res/circlebig.png");

		mesh.Translate(position);
		mesh.Scale(6f);
	}

	public override void Draw()
	{
		//base.Draw();
		mesh.Draw();
	}
}