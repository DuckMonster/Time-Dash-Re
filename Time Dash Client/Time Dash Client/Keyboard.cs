using System;

class Keyboard
{
	private static bool[] currentKeyData = new bool[256];
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
}
