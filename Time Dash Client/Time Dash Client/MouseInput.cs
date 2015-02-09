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
				return x;
			}
		}

		public float Y
		{
			get
			{
				return y;
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
}