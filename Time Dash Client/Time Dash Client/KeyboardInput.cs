using System;
using OpenTK.Input;

class KeyboardInput
{
	private static KeyboardState current, previous;

	public static KeyboardState Current
	{
		get { return current; }
	}
	public static KeyboardState Previous
	{
		get { return previous; }
	}

	public static void Update()
	{
		previous = current;
		current = OpenTK.Input.Keyboard.GetState();
	}

	public static bool KeyPressed(Key k)
	{
		return Current[k] && !Previous[k];
	}

	public static bool KeyReleased(Key k)
	{
		return !Current[k] && Previous[k];
	}
}