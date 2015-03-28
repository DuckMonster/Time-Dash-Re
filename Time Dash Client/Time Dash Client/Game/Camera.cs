using OpenTK;
using OpenTK.Input;

public class Camera
{
	Map map;
	public Vector3 position = new Vector3(0, 0, 5);

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
			Vector2 mouseDelta = (MouseInput.Current.Position - position.Xy) * 0.4f;
			if (mouseDelta.Length > 2f)
				mouseDelta = mouseDelta.Normalized() * 2f;

			Vector2 target = p.Position + p.velocity * 0.1f + mouseDelta;
			Vector2 difference = target - position.Xy;
			position.Xy += difference * 5f * Game.delta;

			float targetZ = 7f + difference.Length * 2.2f + mouseDelta.Length,
				ZDifference = targetZ - position.Z;
			position.Z += ZDifference * 0.8f * Game.delta;
		}
	}
}