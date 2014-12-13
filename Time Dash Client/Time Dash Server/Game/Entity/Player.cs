﻿using OpenTK;
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

	protected PlayerShadow shadow;
	float warpCooldown = 1f;

	Timer forceWarpTimer = new Timer(1.4f, true);

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

		if (warpCooldown > 0f) warpCooldown -= Game.delta;
		if (shadow != null)
		{
			forceWarpTimer.Logic();
			if (forceWarpTimer.IsDone)
			{
				Warp(shadow.position);
				shadow = null;
			}
		}
	}

	public void Input()
	{
		if (input[PlayerKey.Right]) currentAcceleration += Acceleration;
		if (input[PlayerKey.Left]) currentAcceleration -= Acceleration;
		if (IsOnGround && input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump]) Jump();
		if (input[PlayerKey.Jump]) JumpHold();

		if (input[PlayerKey.Warp] && !oldInput[PlayerKey.Warp])
		{
			if (shadow != null)
			{
				Warp(shadow.position);
				shadow = null;
				forceWarpTimer.IsDone = true;
			}
			else if (CanWarp)
			{
				shadow = new PlayerShadow(position, this);
				forceWarpTimer.Reset();
			}
		}
	}

	public void Warp(Vector2 target)
	{
		float velo = (float)(1 - Math.Exp(-(target - position).Length / 2f)) * physics.WarpVelocity;

		float distance = (target - position).Length;

		int accuracy = (int)(distance * 6);
		float step = distance / accuracy;
		Vector2 checkpos = position, dirVector = (target - position).Normalized();

		Log.Write("Raytracing with accuracy " + accuracy);

		for (int i = 0; i < accuracy; i++)
		{
			Player p = map.GetPlayerAtPos(checkpos, size, this);
			if (p != null) p.Hit();

			checkpos += dirVector * step;
		}

		velocity = (target - position).Normalized() * velo;
		position = target;

		warpCooldown = 0f;
	}
}