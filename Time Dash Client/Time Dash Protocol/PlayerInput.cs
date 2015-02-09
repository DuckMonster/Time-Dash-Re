public class PlayerInput
{
	bool[] inputData = new bool[8];
	public int Length
	{
		get
		{
			return inputData.Length;
		}
	}

	public PlayerInput()
	{
		for (int i = 0; i < inputData.Length; i++)
			inputData[i] = false;
	}
	public PlayerInput(bool[] data)
	{
		for (int i = 0; i < inputData.Length; i++)
			inputData[i] = data[i];
	}
	public PlayerInput(PlayerInput pi)
	{
		for (int i = 0; i < inputData.Length; i++)
			inputData[i] = pi[i];
	}

	public byte GetFlag()
	{
		byte flag = 0;

		for (int i = 0; i < inputData.Length; i++)
			if (this[i]) flag = (byte)(1 << i | flag);

		return flag;
	}

	public void DecodeFlag(byte b)
	{
		for (int i = 0; i < inputData.Length; i++)
			this[i] = ((1 << i) & b) != 0;
	}

	public byte GetFlagKey(PlayerKey k)
	{
		byte flag = (byte)(this[k] ? 1 << 7 : 0);
		byte value = (byte)k;

		byte fullFlag = (byte)(flag | value);

		return fullFlag;
	}

	public void DecodeFlagKey(byte b)
	{
		bool flag = (b & 1 << 7) == 1;
		byte value = (byte)(b & 0xF);

		this[(PlayerKey)value] = flag;
	}

	public bool this[PlayerKey k]
	{
		get
		{
			return inputData[(int)k];
		}
		set
		{
			inputData[(int)k] = value;
		}
	}

	public bool this[int s]
	{
		get
		{
			return inputData[s];
		}
		set
		{
			inputData[s] = value;
		}
	}

	public bool Equals(PlayerInput pi)
	{
		for (int i = 0; i < inputData.Length; i++)
			if (inputData[i] != pi[i]) return false;

		return true;
	}
}

public enum PlayerKey : short
{
	Right,
	Left,
	Up,
	Down,
	Jump,
	Dash,
	Parry,
	Shoot
}