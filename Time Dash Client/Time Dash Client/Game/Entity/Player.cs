using EZUDP;
using OpenTK;
using OpenTK.Input;
using System;
using TKTools;

public partial class Player : Actor
{
	protected class PlayerInput
	{
		bool[] inputData = new bool[4];
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

	protected PlayerShadow shadow;
	float warpCooldown = 1f;

	Timer forceWarpTimer;

	public bool CanWarp
	{
		get
		{
			return warpCooldown <= 0;
		}
	}

	public Player(int id, Vector2 position, Map m)
		: base(position, m)
	{
		playerID = id;

		textureList[0] = new Texture("Res/guy.png");
		textureList[1] = new Texture("Res/guyHead1.png");
		textureList[2] = new Texture("Res/guyHead2.png");
		textureList[3] = new Texture("Res/guyHead3.png");

		float w = (size.X / size.Y) / 2;

		mesh.UV = new Vector2[] {
			new Vector2(0.5f-w, 0f),
			new Vector2(0.5f+w, 0f),
			new Vector2(0.5f-w, 1f),
			new Vector2(0.5f+w, 1f)
		};

		shadow = new PlayerShadow(this, mesh);
		forceWarpTimer = new Timer(1f, true);
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

		if (warpCooldown > 0f) warpCooldown -= 1 / shadow.bufferLength * Game.delta;
		shadow.Logic();

		if (shadow.warpHold)
		{
			forceWarpTimer.Logic();
			if (forceWarpTimer.IsDone)
			{
				Warp(shadow.CurrentPosition);
				shadow.warpHold = false;
			}
		}
	}

	public void Input()
	{
		if (input[PlayerKey.Right]) currentAcceleration += Acceleration;
		if (input[PlayerKey.Left]) currentAcceleration -= Acceleration;
		if (IsOnGround && input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump]) Jump();
		if (input[PlayerKey.Jump]) JumpHold();

		if (input[PlayerKey.Warp] && !oldInput[PlayerKey.Warp] && CanWarp)
		{
			shadow.warpHold = true;
			forceWarpTimer.Reset();
		}

		if (!input[PlayerKey.Warp] && shadow.warpHold)
		{
			Warp(shadow.CurrentPosition);
			shadow.warpHold = false;
			forceWarpTimer.IsDone = true;
		}
	}

	public void LocalInput()
	{
		PlayerInput oldInput = new PlayerInput(inputData);

		inputData[PlayerKey.Right] = KeyboardInput.Current[Key.Right];
		inputData[PlayerKey.Left] = KeyboardInput.Current[Key.Left];
		inputData[PlayerKey.Jump] = KeyboardInput.Current[Key.Z];
		inputData[PlayerKey.Warp] = KeyboardInput.Current[Key.X];

		for (int i = 0; i < inputData.Length; i++)
			if (inputData[i] != oldInput[i])
			{
				SendInput();
				break;
			}
	}

	public override void Jump()
	{
		base.Jump();
	}

	public void Warp(Vector2 target)
	{
		map.AddEffect(new EffectSpike(position, target, map));

		float velo = (float)(1 - Math.Exp(-(target - position).Length / 2f)) * physics.WarpVelocity;

		velocity = (target - position).Normalized() * velo;
		position = target;

		warpCooldown = 2f;

		map.AddEffect(new EffectRing(position, 0.2f + (velo / physics.WarpVelocity) * 5f, 0.9f, map));
	}

	public override void Draw()
	{
		mesh.Color = colorList[playerID];
		mesh.FillColor = false;

		mesh.Texture = textureList[tex];

		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));

		mesh.Draw();

		if (!forceWarpTimer.IsDone)
		{
			mesh.Color = new Color(1, 1, 1, forceWarpTimer.PercentageDone);
			mesh.FillColor = true;

			mesh.Draw();
		}

		/*
		mesh.Color = new Color(1, 1, 1, 0.5f);

		mesh.Reset();

		mesh.Translate(serverPosition);
		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));

		mesh.Draw();
		*/
 
		if (shadow.warpHold) shadow.Draw();
	}
}