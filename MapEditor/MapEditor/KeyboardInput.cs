using OpenTK.Input;

namespace MapEditor
{
	public class KeyboardInput
	{
		public class KeyboardStatus
		{
			KeyboardState state;

			public KeyboardStatus(KeyboardState ks)
			{
				state = ks;
			}

			public bool this[Key k]
			{
				get
				{
					return state.IsKeyDown(k);
				}
			}
		}

		static KeyboardStatus current, previous;

		public static KeyboardStatus Current
		{
			get
			{
				return current;
			}
		}

		public static KeyboardStatus Previous
		{
			get
			{
				return previous != null ? previous : current;
			}
		}

		public static void Update()
		{
			KeyboardState state = Keyboard.GetState();
			previous = current;
			current = new KeyboardStatus(state);
		}
	}
}