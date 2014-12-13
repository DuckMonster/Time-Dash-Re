using OpenTK;

public class PlayerShadow
{
	Player player;
	public Vector2 position;

	public PlayerShadow(Vector2 pos, Player p)
	{
		position = pos;
		player = p;
	}
}