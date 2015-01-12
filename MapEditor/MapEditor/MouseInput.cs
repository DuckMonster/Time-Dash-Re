using OpenTK;
using OpenTK.Input;

namespace MapEditor
{
	public class MouseInput
	{
		public class MouseStatus
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
				return new Vector2(X, Y) * (Editor.camera.Position.Z - z);
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

		static MouseStatus current, previous;
		public static int CurrentX, CurrentY;

		public static MouseStatus Current
		{
			get
			{
				return current;
			}
		}

		public static MouseStatus Previous
		{
			get
			{
				return previous != null ? previous : current;
			}
		}

		public static Vector2 Delta
		{
			get
			{
				return current.Position - previous.Position;
			}
		}

		public static bool ButtonPressed(MouseButton mb)
		{
			return Current[mb] && !Previous[mb];
		}

		public static bool ButtonReleased(MouseButton mb)
		{
			return !Current[mb] && Previous[mb];
		}

		public static void Update(GameWindow window)
		{
			MouseState state = Mouse.GetState();

			float x = ((float)CurrentX / window.ClientSize.Width - 0.5f) * Editor.screenWidth;
			float y = ((float)CurrentY / window.ClientSize.Height - 0.5f) * Editor.screenHeight * -1;

			previous = current;
			current = new MouseStatus(x, y, state);
		}
	}
}