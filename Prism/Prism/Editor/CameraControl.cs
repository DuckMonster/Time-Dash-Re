using OpenTK;
using OpenTK.Input;
using System;
using TKTools.Context.Input;

public class CameraControl
{
	static CameraControl activeControl;
	public static Vector3 Position
	{
		get { return activeControl.position; }
	}

	Editor editor;

	Vector3 position = new Vector3(0, 0, 5);
	Vector2 previousMouse;

	KeyboardWatch keyboard = Editor.keyboard;
	MouseWatch mouse = Editor.mouse;

	float zoomOrigin;
	float zoomOriginZ;

	public bool Active
	{
		get { return keyboard[Key.LAlt] || mouse[MouseButton.Middle]; }
	}

	public CameraControl(Editor e)
	{
		activeControl = this;
		editor = e;
	}

	void Move()
	{
		Vector2 currentMouse = mouse.ScreenPosition * 0.5f;
		currentMouse.X *= editor.form.AspectRatio;

		Vector2 delta = (currentMouse - previousMouse) * Position.Z;
		position.X -= delta.X;
		position.Y -= delta.Y;
	}

	public void Logic()
	{
		if (editor.form.Focused)
		{
			if (keyboard[Key.LAlt] && mouse[MouseButton.Left] || mouse[MouseButton.Middle]) Move();

			if (keyboard[Key.LAlt])
			{
				if (mouse[MouseButton.Right])
				{
					float currentMouse = (mouse.ScreenPosition.X + 1f) / 2f;

					if (mouse.ButtonPressed(MouseButton.Right))
					{
						zoomOrigin = currentMouse;
						zoomOriginZ = position.Z;
					}

					position.Z = zoomOriginZ * (currentMouse / zoomOrigin);
					if (position.Z < 1f) position.Z = 1f;
				}
			}

			//Zoom :)
			if (keyboard.HasPrefix(KeyPrefix.None))
				position.Z -= mouse.WheelDelta * 0.1f * position.Z;

			previousMouse = mouse.ScreenPosition * 0.5f;
			previousMouse.X *= editor.form.AspectRatio;
		}
	}
}