using System;

enum Key
{
	LeftArrow = 37,
	UpArrow = 38,
	RightArrow = 39,
	DownArrow = 40
}

class Keyboard
{
	private static bool[] currentKeyData = new bool[400];
	private static Keyboard current, previous;

	public static Keyboard Current
	{
		get { return current; }
	}
	public static Keyboard Previous
	{
		get { return previous; }
	}

	public static void SetKey(byte k, bool s)
	{
		if (s) Console.WriteLine(k + " is pressed!");
		currentKeyData[k] = s;
	}

	public static void Update()
	{
		previous = current;
		current = new Keyboard(currentKeyData);
	}

	private bool[] key;

	public Keyboard(bool[] data)
	{
		key = new bool[data.Length];
		for (int i = 0; i < key.Length; i++) key[i] = data[i];
	}

	public bool this[byte i]
	{
		get
		{
			return key[i];
		}
	}
	public bool this[char i]
	{
		get
		{
			return key[i];
		}
	}
	public bool this[Key k]
	{
		get
		{
			return key[(int)k];
		}
	}
}
