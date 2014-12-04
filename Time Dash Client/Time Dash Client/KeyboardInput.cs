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
}