using OpenTK;
using OpenTK.Input;
public class MouseInput
{
	public class MouseStatus
	{
		MouseState status;
		float x, y;

		public float X
		{
			get
			{
				return x * camera.position.Z + camera.position.X;
			}
		}

		public float Y
		{
			get
			{
				return y * camera.position.Z + camera.position.Y;
			}
		}

		public Vector2 Position
		{
			get
			{
				return new Vector2(X, Y);
			}
		}

		public Vector2 PositionOrtho
		{
			get
			{
				return new Vector2(x, y) * 10f;
			}
		}

		public MouseStatus(float x, float y, MouseState status)
		{
			this.x = x;
			this.y = y;
			this.status = status;
		}

		public bool this[MouseButton btn]
		{
			get
			{
				return status.IsButtonDown(btn);
			}
		}
	}

	private static Camera camera;
	public static void SetCamera(Camera c)
	{
		camera = c;
	}

	private static MouseStatus current, previous;

	public static MouseStatus Current
	{
		get { return current; }
	}
	public static MouseStatus Previous
	{
		get { return previous; }
	}

	public static void Update(int x, int y, GameWindow window)
	{
		float xx = ((float)x / window.ClientSize.Width - 0.5f) * 2;
		float yy = ((float)y / window.ClientSize.Height - 0.5f) * 2 * -Game.windowRatio;

		previous = current;
		current = new MouseStatus(xx, yy, Mouse.GetState());
	}

	public static bool GetPressed(MouseButton btn)
	{
		if (Previous == null || Current == null) return false;
		return (Current[btn] && !Previous[btn]);
	}

	public static bool GetReleased(MouseButton btn)
	{
		if (Previous == null || Current == null) return false;
		return (!Current[btn] && Previous[btn]);
	}
}