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
	Timer warpCooldown;
	float warpCooldownMax = 1.3f;

	float wallStick = 0f;
	bool wallStickable = true;

	bool canDoublejump = true;

	Timer dashCooldown;
	float dashCooldownMax = 0.3f;

	float dashInterval = 0.2f;
	float dashTimer = 0f;
	float ignoreGravityTimer = 0f;

	public bool CanWarp
	{
		get
		{
			return warpCooldown.IsDone;
		}
	}

	public int WallTouch
	{
		get
		{
			if (map.GetCollision(this, new Vector2(-0.2f, 0f))) return -1;
			if (map.GetCollision(this, new Vector2(0.2f, 0))) return 1;
			return 0;
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

		warpCooldown = new Timer(warpCooldownMax, false);
		dashCooldown = new Timer(dashCooldownMax, true);
	}

	public override void Die()
	{
		base.Die();
		warpCooldown.Reset();
		map.AddEffect(new EffectSkull(position, map));
	}

	public override void Logic()
	{
		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		Input();

		if (!wallStickable && WallTouch == 0) wallStickable = true;
		if (wallStick > 0)
		{
			if (WallTouch == 0 || IsOnGround) wallStick = 0;
			wallStick -= Game.delta;
		}

		if (!canDoublejump && IsOnGround) canDoublejump = true;

		ignoreGravityTimer -= Game.delta;
		dashTimer -= Game.delta;

		base.Logic();

		if (KeyboardInput.Current[Key.Number1]) tex = 0;
		if (KeyboardInput.Current[Key.Number2]) tex = 1;
		if (KeyboardInput.Current[Key.Number3]) tex = 2;
		if (KeyboardInput.Current[Key.Number4]) tex = 3;

		warpCooldown.Logic();
		dashCooldown.Logic();

		shadow.Logic();
	}

	public override void DoPhysics()
	{
		if (!IsOnGround &&
			Math.Abs(velocity.X) > physics.MaxVelocity &&
			((velocity.X > 0 && input[PlayerKey.Right]) ||
			(velocity.X < 0 && input[PlayerKey.Left])))
		{
			Log.Debug("GOTTA GO FAST!");
		}
		else if (ignoreGravityTimer <= 0)
			velocity.X += currentAcceleration * Game.delta - velocity.X * Friction * Game.delta;

		currentAcceleration = 0;
		if (ignoreGravityTimer <= 0f) velocity.Y -= physics.Gravity * Game.delta;
	}

	public void Input()
	{
		if (input[PlayerKey.Right])
		{
			if (!oldInput[PlayerKey.Right])
			{
				if (dashTimer > 0f && dashCooldown.IsDone)
					Dash(1);
				else
					dashTimer = dashInterval;
			}
			if (wallStickable && WallTouch == -1)
			{
				wallStick = 0.3f;
				wallStickable = false;
			}
			if (wallStick <= 0) currentAcceleration += Acceleration;
		}

		if (input[PlayerKey.Left])
		{
			if (!oldInput[PlayerKey.Left])
			{
				if (dashTimer > 0f && dashCooldown.IsDone)
					Dash(-1);
				else
					dashTimer = dashInterval;
			}
			if (wallStickable && WallTouch == 1)
			{
				wallStick = 0.3f;
				wallStickable = false;
			}
			if (wallStick <= 0) currentAcceleration -= Acceleration;
		}

		if (input[PlayerKey.Jump] && !oldInput[PlayerKey.Jump])
		{
			if (IsOnGround) Jump();
			else if (WallTouch != 0) WallJump();
			else if (canDoublejump)
			{
				Jump();
				canDoublejump = false;
			}
		}
		if (input[PlayerKey.Jump]) JumpHold();
		if (input[PlayerKey.Dash] && !oldInput[PlayerKey.Dash]) Warp();
	}

	public void LocalInput()
	{
		PlayerInput oldInput = new PlayerInput(inputData);

		inputData[PlayerKey.Right] = KeyboardInput.Current[Key.Right];
		inputData[PlayerKey.Left] = KeyboardInput.Current[Key.Left];
		inputData[PlayerKey.Jump] = KeyboardInput.Current[Key.Z];
		inputData[PlayerKey.Dash] = KeyboardInput.Current[Key.X];

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
		ignoreGravityTimer = 0;
	}

	public void WallJump()
	{
		Vector2 velo = physics.WallJumpVector;

		velocity = new Vector2(velo.X * -WallTouch, velo.Y);

		wallStick = 0f;

		canDoublejump = true;
		ignoreGravityTimer = 0;
	}

	public void Warp()
	{
		if (!CanWarp) return;

		map.AddEffect(new EffectSpike(position, shadow.CurrentPosition, map));

		float velo = (float)(1 - Math.Exp(-(shadow.CurrentPosition - position).Length / 2f)) * physics.WarpVelocity;

		velocity = (shadow.CurrentPosition - position).Normalized() * velo;
		position = shadow.CurrentPosition;

		warpCooldown.Reset();

		map.AddEffect(new EffectRing(position, 0.2f + (velo / physics.WarpVelocity) * 5f, 0.9f, map));

		ignoreGravityTimer = 0;
	}

	public void Dash(int dir)
	{
		Vector2 target = position + new Vector2(physics.DashLength * dir, 0);

		float distance = (target - position).Length;

		int accuracy = (int)(distance * 6);
		float step = distance / accuracy;
		Vector2 checkpos = position, dirVector = (target - position).Normalized();

		for (int i = 0; i < accuracy; i++)
		{
			Vector2 buffer = checkpos;
			buffer += dirVector * step;
			if (map.GetCollision(buffer, size)) break;

			checkpos = buffer;
		}

		map.AddEffect(new EffectSpike(position, checkpos, map));

		position = checkpos;
		velocity = new Vector2(physics.DashVelocity * dir, 0);

		map.AddEffect(new EffectRing(position, 2f, 0.4f, map));

		ignoreGravityTimer = 0.2f;
		dashCooldown.Reset();
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

		if (!warpCooldown.IsDone)
		{
			mesh.Color = new Color(1f, 1f, 1f, 1 - warpCooldown.PercentageDone);
			mesh.FillColor = true;

			mesh.Draw();
		}

		if (!dashCooldown.IsDone)
		{
			mesh.Color = new Color(1f, 1f, 1f, 1 - dashCooldown.PercentageDone);
			mesh.FillColor = true;

			mesh.Draw();
		}

		/*
		mesh.Color = new Color(0, 0, 1, 0.5f);
		mesh.FillColor = true;

		mesh.Reset();

		mesh.Translate(serverPosition);
		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));

		mesh.Draw();
		*/

		if (CanWarp && IsLocalPlayer) shadow.Draw();
	}
}