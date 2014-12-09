using OpenTK;
using OpenTK.Input;
using System;

using EZUDP;
using EZUDP.Server;

public class Player : Actor
{
	class PlayerInput
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

	PlayerInput inputData = new PlayerInput();
	PlayerInput input, oldInput;

	public int playerID;
	public Client client;

	public Player(int id, Client c, Vector2 position, Map m)
		: base(position, m)
	{
		playerID = id;
		client = c;
	}

	public void ReceiveInput(Vector2 position, Vector2 velocity, byte k)
	{
		this.position = position;
		this.velocity = velocity;
		inputData.DecodeFlag(k);
		SendInputToPlayer(map.playerList);
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
		SendPositionToPlayer(map.playerList);
	}

	public override void Logic()
	{
		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		Input();

		base.Logic();

		Log.Debug(Convert.ToString(inputData.GetFlag(), 2));
	}

	public void Input()
	{
		if (input[PlayerKey.Right]) currentAcceleration += Acceleration;
		if (input[PlayerKey.Left]) currentAcceleration -= Acceleration;
		if (IsOnGround && input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump]) Jump();
		if (input[PlayerKey.Jump]) JumpHold();
	}

	public void SendExistanceToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetExistanceMessage(), players);
		SendPositionToPlayer(players);
	}

	public void SendLeaveToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetLeaveMessage(), players);
	}

	public void SendInputToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetInputMessage(), players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), players);
	}

	void SendMessageToPlayer(MessageBuffer msg, params Player[] players)
	{
		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}

	MessageBuffer GetExistanceMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerJoin);
		msg.WriteByte(playerID);

		return msg;
	}

	MessageBuffer GetLeaveMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerLeave);
		msg.WriteByte(playerID);

		return msg;
	}

	MessageBuffer GetInputMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInput);
		msg.WriteByte(playerID);
		msg.WriteVector(position);
		msg.WriteVector(velocity);
		msg.WriteByte(inputData.GetFlag());

		return msg;
	}

	MessageBuffer GetPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
		msg.WriteByte(playerID);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		return msg;
	}
}