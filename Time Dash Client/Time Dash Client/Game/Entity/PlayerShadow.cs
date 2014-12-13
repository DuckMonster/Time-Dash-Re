using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

public class PlayerShadow
{
	Player player;
	Mesh mesh;
	public Vector2 position;

	public PlayerShadow(Vector2 pos, Player p, Mesh m)
	{
		position = pos;
		player = p;
		mesh = m;
	}

	public void Draw()
	{
		mesh.Color = new Color(0, 0, 0, 0.4f);

		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(player.size);
		//mesh.Scale(new Vector2(1, 1));

		mesh.Draw();
	}
}