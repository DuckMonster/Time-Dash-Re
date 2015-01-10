using OpenTK;
using OpenTK.Input;

namespace MapEditor
{
	public class MouseInput
	{
		public struct MouseStatus
		{
			MouseState state;
			float x, y;

			public MouseStatus(float x, float y, MouseState ms)
			{
				this.x = x;
				this.y = y;
				state = ms;
			}

			public Vector2 Position
			{
				get
				{
					return GetPositionAtZ(0);
				}
			}
			public Vector2 PositionOrtho
			{
				get
				{
					return new Vector2(X, Y);
				}
			}

			public Vector2 GetPositionAtZ(float z)
			{
				return new Vector2(X, Y) * (5 - z);
			}

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

			public float Wheel
			{
				get
				{
					return state.WheelPrecise;
				}
			}

			public bool this[MouseButton mb]
			{
				get
				{
					return state.IsButtonDown(mb);
				}
			}

			public static implicit operator Vector2(MouseStatus mi)
			{
				return mi.Position;
			}
		}

		public static MouseStatus Current, Previous;
		public static int CurrentX, CurrentY;

		public static void Update(GameWindow window)
		{
			MouseState state = Mouse.GetState();

			float x = ((float)CurrentX / window.ClientSize.Width - 0.5f) * Editor.screenWidth;
			float y = ((float)CurrentY / window.ClientSize.Height - 0.5f) * Editor.screenHeight * -1;

			Previous = Current;
			Current = new MouseStatus(x, y, state);
		}
	}
}