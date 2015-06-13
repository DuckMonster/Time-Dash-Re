using OpenTK;
using OpenTK.Input;
using TKTools.Context;
using TKTools.Context.Input;

public class CameraControl
{
	Map map;
	Camera camera;
	public Vector3 position = new Vector3(0, 0, 5);
	public Entity focusObject;
	public Entity secondaryObject;

	MouseWatch mouse;
	float maxZ = 40f;

	public Entity FocusObject
	{
		get
		{
			if (focusObject == null)
				return map.LocalPlayer;
			else
				return focusObject;
		}
		set
		{
			focusObject = value;
		}
	}

	public CameraControl(Map m, Camera camera)
	{
		map = m;
		this.camera = camera;
		mouse = new MouseWatch();
	}

	public void Logic()
	{
		Entity f = FocusObject;

		if (f != null)
		{
			Vector2 mouseDelta = map.PauseInput ? Vector2.Zero : (mouse.ScreenPosition) * 4f;
			if (mouseDelta.Length > 2f)
				mouseDelta = mouseDelta.Normalized() * 2f;

			Vector2 target = f.Position;

			if (f is Actor)
				target += (f as Actor).velocity * 0.1f + mouseDelta;

			if (secondaryObject != null)
				target = (target + secondaryObject.Position) / 2;

            Vector2 difference = target - position.Xy;
			position.Xy += difference * 5f * Game.delta;

			float targetZ = 12f + difference.Length * 6f + mouseDelta.Length * 1.6f,
				ZDifference = targetZ - position.Z;
			position.Z += ZDifference * 0.8f * Game.delta;

			if (position.Z > maxZ) position.Z = maxZ;

			camera.Position = position;
			camera.Target = position * new Vector3(1, 1, 0);
		}
	}
}