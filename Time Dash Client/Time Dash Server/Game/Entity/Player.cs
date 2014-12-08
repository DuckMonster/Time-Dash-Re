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

	public void ToggleInput(byte k) { ToggleInput((PlayerKey)k); }
	public void ToggleInput(PlayerKey k)
	{
		inputData[k] = !inputData[k];
		SendInputToggleToPlayer(k, map.playerList);
		SendPositionToPlayer(map.playerList);
	}

	public override void ReceivePosition(float x, float y)
	{
		base.ReceivePosition(x, y);
		SendPositionToPlayer(map.playerList);
	}

	public override void Logic()
	{
		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		Input();

		base.Logic();
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
	}

	public void SendLeaveToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetLeaveMessage(), players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), players);
	}

	public void SendInputToggleToPlayer(PlayerKey k, params Player[] players)
	{
		SendMessageToPlayer(GetInputToggleMessage(k), players);
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

	MessageBuffer GetInputToggleMessage(PlayerKey k)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInputToggle);
		msg.WriteByte(playerID);
		msg.WriteByte((byte)k);

		return msg;
	}

	MessageBuffer GetPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
		msg.WriteByte(playerID);
		msg.WriteFloat(position.X);
		msg.WriteFloat(position.Y);

		return msg;
	}
}