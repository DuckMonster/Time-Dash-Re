using EZUDP;
using OpenTK;
using OpenTK.Input;
using System;
using TKTools;

public class Player : Actor
{
	protected class PlayerInput
	{
		bool[] inputData = new bool[3];
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
	}
	public enum PlayerKey : short
	{
		Right,
		Left,
		Jump
	}

	protected PlayerInput inputData = new PlayerInput();
	protected PlayerInput input, oldInput;

	public Color[] colorList = new Color[] {
		Color.Blue,
		Color.Orange,
		Color.Green,
		Color.Teal,
		Color.Violet,
		Color.Yellow
	};	

	protected Texture[] textureList = new Texture[4];
	protected int tex = 0;

	public Player(Vector2 position, Map m)
		: base(position, m)
	{
		textureList[0] = new Texture("Res/guy.png");
		textureList[1] = new Texture("Res/guyHead1.png");
		textureList[2] = new Texture("Res/guyHead2.png");
		textureList[3] = new Texture("Res/guyHead3.png");
		//mesh.Texture = new TKTools.Texture("Res/guyHead1.png");

		float w = (size.X / size.Y) / 2;

		mesh.UV = new Vector2[] {
			new Vector2(0.5f-w, 0f),
			new Vector2(0.5f+w, 0f),
			new Vector2(0.5f-w, 1f),
			new Vector2(0.5f+w, 1f)
		};
	}

	public override void Logic()
	{
		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		Input();

		base.Logic();

		if (KeyboardInput.Current[Key.Number1]) tex = 0;
		if (KeyboardInput.Current[Key.Number2]) tex = 1;
		if (KeyboardInput.Current[Key.Number3]) tex = 2;
		if (KeyboardInput.Current[Key.Number4]) tex = 3;
	}

	public void Input()
	{
		if (input[PlayerKey.Right]) currentAcceleration += Acceleration;
		if (input[PlayerKey.Left]) currentAcceleration -= Acceleration;
		if (IsOnGround && input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump]) Jump();
		if (input[PlayerKey.Jump]) JumpHold();
	}

	public virtual void LocalInput()
	{
		inputData[PlayerKey.Right] = KeyboardInput.Current[Key.Right];
		inputData[PlayerKey.Left] = KeyboardInput.Current[Key.Left];
		inputData[PlayerKey.Jump] = KeyboardInput.Current[Key.Z];
	}

	public override void Jump()
	{
		base.Jump();
	}

	public override void Draw()
	{
		mesh.Texture = textureList[tex];
		mesh.Color = Color.White;

		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(position);

		mesh.Draw();
	}
}