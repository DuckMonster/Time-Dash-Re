using OpenTK;
using OpenTK.Input;
using System;

using EZUDP;
using EZUDP.Server;

public partial class Player : Actor
{
	class PlayerInput
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

	PlayerInput inputData = new PlayerInput();
	PlayerInput input, oldInput;

	public int playerID;
	public Client client;

	public PlayerShadow shadow;
	float warpCooldown = 1f;

	public bool CanWarp
	{
		get
		{
			return warpCooldown <= 0;
		}
	}

	public Player(int id, Client c, Vector2 position, Map m)
		: base(position, m)
	{
		playerID = id;
		client = c;

		shadow = new PlayerShadow(this);
	}

	public override void Hit()
	{
		base.Hit();
		position = new Vector2(2, 9);
		velocity = Vector2.Zero;
		SendPositionToPlayer(map.playerList);
	}

	public override void Logic()
	{
		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		Input();

		base.Logic();

		if (warpCooldown > 0f) warpCooldown -= 1 / shadow.bufferLength * Game.delta;
		shadow.Logic();
	}

	public void Input()
	{
		if (input[PlayerKey.Right]) currentAcceleration += Acceleration;
		if (input[PlayerKey.Left]) currentAcceleration -= Acceleration;
		if (IsOnGround && input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump]) Jump();
		if (input[PlayerKey.Jump]) JumpHold();
		if (input[PlayerKey.Warp] && !oldInput[PlayerKey.Warp]) Warp();
	}

	public void Warp()
	{
		if (!CanWarp) return;

		float distance = (shadow.CurrentPosition - position).Length;

		float velo = (float)(1 - Math.Exp(-distance / 2f)) * physics.WarpVelocity;

		int accuracy = (int)distance;
		float step = distance / accuracy;
		Vector2 checkpos = position, dirVector = (shadow.CurrentPosition - position).Normalized();

		for (int i = 0; i < accuracy; i++)
		{
			Player p = map.GetPlayerAtPos(checkpos, size, this);
			if (p != null) p.Hit();

			checkpos += dirVector * step;
		}

		velocity = (shadow.CurrentPosition - position).Normalized() * velo;
		position = shadow.CurrentPosition;

		warpCooldown = 1.5f;
	}
}