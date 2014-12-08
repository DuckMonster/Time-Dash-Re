using EZUDP;
using OpenTK;
using OpenTK.Input;
using System;
using TKTools;

public class Player : Actor
{
	Vector2 wallJumpForce = new Vector2(12, 10);

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
	public enum WallStickType
	{
		Left = 1,
		Right = -1,
		None = 0
	}

	PlayerInput inputData = new PlayerInput();
	PlayerInput input, oldInput;

	public Color[] colorList = new Color[] {
		Color.Blue,
		Color.Orange,
		Color.Green,
		Color.Teal,
		Color.Violet,
		Color.Yellow
	};

	public int playerID;
	public bool IsLocalPlayer
	{
		get
		{
			return map.myID == playerID;
		}
	}

	Texture[] textureList = new Texture[4];
	int tex = 0;

	Vector2 serverOffset = Vector2.Zero;
	Vector2 serverPosition = Vector2.Zero;

	public WallStickType WallTouch
	{
		get
		{
			if (IsOnGround) return WallStickType.None;

			if (map.GetCollision(this, new Vector2(0.1f, 0))) return WallStickType.Right;
			if (map.GetCollision(this, new Vector2(-0.1f, 0))) return WallStickType.Left;

			return WallStickType.None;
		}
	}
	public WallStickType WallStick
	{
		get
		{
			if (IsOnGround || wallStickFactor <= 0) return WallStickType.None;

			if (map.GetCollision(this, new Vector2(0.1f, 0))) return WallStickType.Right;
			if (map.GetCollision(this, new Vector2(-0.1f, 0))) return WallStickType.Left;

			return WallStickType.None;
		}
	}
	public bool IsWallSticking
	{
		get
		{
			return WallStick != WallStickType.None;
		}
	}

	WallStickType prevWallTouch = WallStickType.None;
	float wallStickFactor = 0f;

	public Player(int id, Vector2 position, Map m)
		: base(position, m)
	{
		playerID = id;

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

	public void ToggleKey(byte k) { ToggleKey((PlayerKey)k); }
	public void ToggleKey(PlayerKey k)
	{
		if (!IsLocalPlayer) inputData[k] = !inputData[k];
	}

	public override void ReceivePosition(float x, float y)
	{
		//if (!IsLocalPlayer) base.ReceivePosition(x, y);
		serverPosition = new Vector2(x, y);
		if (!IsLocalPlayer) serverOffset = serverPosition - position;
	}

	public void SendPosition()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
		msg.WriteFloat(position.X);
		msg.WriteFloat(position.Y);

		Game.client.Send(msg);
	}

	public override void Logic()
	{
		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		Input();

		base.Logic();

		if (serverOffset.Length > 0.01f)
		{
			Vector2 movement = serverOffset * 6f * Game.delta;
			serverOffset -= serverOffset * 6f * Game.delta;
			position += movement;
		}

		if (KeyboardInput.Current[Key.Number1]) tex = 0;
		if (KeyboardInput.Current[Key.Number2]) tex = 1;
		if (KeyboardInput.Current[Key.Number3]) tex = 2;
		if (KeyboardInput.Current[Key.Number4]) tex = 3;
	}

	public override void DoPhysics()
	{
		base.DoPhysics();
	}

	public void Input()
	{
		if (input[PlayerKey.Right])
		{
			if (WallTouch == WallStickType.Left && wallStickFactor > 0) wallStickFactor -= Game.delta * 10;
			else if (WallTouch == WallStickType.Right) wallStickFactor = 1f;
			else currentAcceleration += Acceleration;
		}
		if (input[PlayerKey.Left])
		{
			if (WallTouch == WallStickType.Right && wallStickFactor > 0) wallStickFactor -= Game.delta * 10;
			else if (WallTouch == WallStickType.Left) wallStickFactor = 1f;
			else currentAcceleration -= Acceleration;
		}
		if (input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump] && (IsOnGround || WallStick != WallStickType.None)) Jump();
		if (input[PlayerKey.Jump]) JumpHold();
	}

	public void LocalInput()
	{
		PlayerInput newInput = new PlayerInput();

		newInput[PlayerKey.Right] = KeyboardInput.Current[Key.Right];
		newInput[PlayerKey.Left] = KeyboardInput.Current[Key.Left];
		newInput[PlayerKey.Jump] = KeyboardInput.Current[Key.Z];

		for (int i = 0; i < inputData.Length; i++)
			if (inputData[i] != newInput[i])
			{
				SendInputToggle(i);
				inputData[i] = newInput[i];
			}
	}

	public void SendInputToggle(PlayerKey k) { SendInputToggle((int)k); }
	public void SendInputToggle(int k)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInputToggle);
		msg.WriteByte(k);

		Game.client.Send(msg);

		SendPosition();
	}

	public override void Jump()
	{
		base.Jump();

		WallStickType wallStick = WallStick;

		if (wallStick != WallStickType.None)
		{
			int ws = (int)wallStick;

			velocity = new Vector2(wallJumpForce.X * ws, wallJumpForce.Y);
		}
	}

	public override void Draw()
	{
		mesh.Color = colorList[playerID];
		mesh.Texture = textureList[tex];

		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(position);

		mesh.Draw();

		mesh.Color = new Color(1f, 1f, 1f, 0.4f);

		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(serverPosition);

		mesh.Draw();
	}
}