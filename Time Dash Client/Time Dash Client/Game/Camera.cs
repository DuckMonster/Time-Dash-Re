using OpenTK;
using OpenTK.Input;

class Camera
{
	Map map;
	Vector3 position = new Vector3(0, 0, 5);

	public Matrix4 ViewMatrix
	{
		get
		{
			return Matrix4.LookAt(position, new Vector3(position.X, position.Y, 0), Vector3.UnitY);
		}
	}

	public Camera(Map m)
	{
		map = m;
	}

	public void Logic()
	{
		Player p = map.LocalPlayer;

		if (p != null)
		{
			Vector2 difference = p.position - position.Xy;
			position.Xy += difference * 4f * Game.delta;

			float targetZ = 5f + difference.Length + 0.6f,
				ZDifference = targetZ - position.Z;
			position.Z += ZDifference * 0.8f * Game.delta;
		}
	}
}