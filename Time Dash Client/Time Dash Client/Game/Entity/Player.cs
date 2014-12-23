using EZUDP;
using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using TKTools;

public partial class Player : Actor
{
	public class WarpTarget
	{
		public float timeTraveled = 0;
		public float lastStep = 0;

		public float angle;

		public Vector2 startPosition;
		public Vector2 endPosition;

		public Vector2 Direction
		{
			get
			{
				return (endPosition - startPosition).Normalized();
			}
		}

		public WarpTarget(Vector2 a, Vector2 b)
		{
			startPosition = a;
			endPosition = b;
			angle = TKMath.GetAngle(a, b);
		}
	}
	public class DashTarget
	{
		public float timeTraveled = 0;
		public float lastStep = 0;

		public float angle;

		public Vector2 startPosition;
		public Vector2 endPosition;

		public Vector2 Direction
		{
			get
			{
				return (endPosition - startPosition).Normalized();
			}
		}

		public DashTarget(Vector2 a, Vector2 b)
		{
			startPosition = a;
			endPosition = b;
			angle = TKMath.GetAngle(a, b);
		}
	}

	protected PlayerInput inputData = new PlayerInput();
	protected PlayerInput input, oldInput;
	protected Vector2 inputDirection, oldInputDirection, lastDirection;

	public Color[] colorList = new Color[] {
		Color.Blue,
		Color.Orange,
		Color.Green,
		Color.Teal,
		Color.Violet,
		Color.Yellow
	};	

	protected PlayerShadow shadow;

	float wallStick = -1;
	bool canDoublejump = true;

	WarpTarget warpTarget = null;
	DashTarget dashTarget = null;

	Timer warpCooldown;
	Timer dashCooldown;
	Timer disableTimer;

	float dashInterval = 0.2f;
	float dashIntervalTimer = 0;

	float dashGravityIgnoreTime = 0.2f;
	float gravityIgnore = 0f;

	public Tileset playerTileset = new Tileset(200, 200, "Res/jackTileset.png");

	public int WallTouch
	{
		get
		{
			if (map.GetCollision(this, new Vector2(-0.2f, 0f))) return -1;
			if (map.GetCollision(this, new Vector2(0.2f, 0))) return 1;
			return 0;
		}
	}

	public Color MyColor
	{
		get
		{
			return colorList[playerID];
		}
	}

	public bool WallStickable
	{
		get
		{
			return wallStick == -1;
		}
		set
		{
			if (value) wallStick = -1;
		}
	}
	public bool IsWarping
	{
		get
		{
			return warpTarget != null;
		}
	}
	public bool IsDashing
	{
		get
		{
			return dashTarget != null;
		}
	}
	public bool CanWarp
	{
		get
		{
			return shadow != null && warpCooldown.IsDone;
		}
	}
	public bool CanDash
	{
		get
		{
			if (IsOnGround)
				return !Disabled;
			else
				return !Disabled && airDashNmbr > 0;
		}
	}
	public bool IgnoresGravity
	{
		get
		{
			return gravityIgnore > 0;
		}
	}
	public bool Disabled
	{
		get
		{
			return !disableTimer.IsDone;
		}
		set
		{
			if (!value) disableTimer.IsDone = true;
			else disableTimer.Reset();
		}
	}

	public Player(int id, Vector2 position, Map m)
		: base(position, m)
	{
		playerID = id;
		float w = (size.X / size.Y) / 2;

		mesh.UV = new Vector2[] {
			new Vector2(0.5f-w, 0f),
			new Vector2(0.5f+w, 0f),
			new Vector2(0.5f-w, 1f),
			new Vector2(0.5f+w, 1f)
		};

		shadow = new PlayerShadow(this, mesh);

		warpCooldown = new Timer(stats.WarpCooldown, false);
		dashCooldown = new Timer(stats.DashCooldown, true);
		disableTimer = new Timer(stats.DisableTime, true);
	}

	public void Die(Vector2 diePos)
	{
		base.Die();
		map.AddEffect(new EffectSkull(diePos, MyColor, map));

		warpTarget = null;
		dashTarget = null;

		warpCooldown.Reset();
	}

	public override void Logic()
	{
		if (shadow != null && !warpCooldown.IsDone)
		{
			warpCooldown.Logic();
			if (warpCooldown.IsDone)
			{
				map.AddEffect(new EffectRing(shadow.CurrentPosition, 1.2f, 0.5f, MyColor, map));
			}
		}

		dashCooldown.Logic();
		disableTimer.Logic();

		oldInput = input;
		input = new PlayerInput(inputData);
		if (oldInput == null) oldInput = input;

		if (gravityIgnore > 0 && !input.Equals(oldInput)) { gravityIgnore = 0; }
		gravityIgnore -= Game.delta;

		dashIntervalTimer -= Game.delta;

		if (warpTarget == null && dashTarget == null)
		{
			Input();

			if (!WallStickable && WallTouch == 0) WallStickable = true;
			if (wallStick > 0)
			{
				if (WallTouch == 0 || IsOnGround)
				{
					wallStick = 0;
					WallStickable = true;
				}
				wallStick -= Game.delta;
			}

			if (!canDoublejump && IsOnGround) canDoublejump = true;

			base.Logic();

			if (shadow == null && (IsOnGround || WallTouch != 0)) CreateShadow();
			if (shadow != null) shadow.Logic();
		}
		else if (warpTarget != null)
		{
			WarpStep();
		}
		else if (dashTarget != null)
		{
			DashStep();
			if (shadow != null) shadow.Logic();
		}
	}

	public override void DoPhysics()
	{
		bool aboveMaxSpeed = Math.Abs(velocity.X) > stats.MaxVelocity &&
			((velocity.X > 0 && input[PlayerKey.Right]) ||
			(velocity.X < 0 && input[PlayerKey.Left]));

		if (aboveMaxSpeed)
		{
			if (!IsOnGround)
			{
				Log.Debug("GOTTA GO FAST!");
			}
			else
			{
				velocity.X += (currentAcceleration * Game.delta - velocity.X * Friction * Game.delta) * 0.2f;
			}
		}
		else
		{
			velocity.X += currentAcceleration * Game.delta - velocity.X * Friction * Game.delta;
		}

		currentAcceleration = 0;
		if (!IgnoresGravity) velocity.Y -= stats.Gravity * Game.delta;
	}

	public override void Land()
	{
		airDashNmbr = stats.AirDashMax;

		base.Land();

		if (IsLocalPlayer) SendLand();
	}

	public void Input()
	{
		inputDirection = Vector2.Zero;
		if (input[PlayerKey.Right]) inputDirection.X++;
		if (input[PlayerKey.Left]) inputDirection.X--;

		if (input[PlayerKey.Up]) inputDirection.Y++;
		if (input[PlayerKey.Down]) inputDirection.Y--;

		if (IsLocalPlayer)
		{
			//Dashing hori
			if (inputDirection.X != 0)
			{
				if (inputDirection.X != lastDirection.X) dashIntervalTimer = 0;

				if (inputDirection.X != oldInputDirection.X)
				{
					if (dashIntervalTimer > 0 && dashCooldown.IsDone)
						Dash((int)inputDirection.X);
					else
						dashIntervalTimer = dashInterval;

					lastDirection.X = inputDirection.X;
				}
			}

			//Dashing vert
			if (inputDirection.Y != 0)
			{
				if (inputDirection.Y != oldInputDirection.Y)
				{
					if (dashIntervalTimer > 0 && dashCooldown.IsDone)
						DashVertical((int)inputDirection.Y);
					else
						dashIntervalTimer = dashInterval;

					lastDirection.Y = inputDirection.Y;
				}
			}

			//Warp
			if (input[PlayerKey.Warp] && !oldInput[PlayerKey.Warp] && CanWarp)
			{
				Warp(shadow.CurrentPosition);
			}
		}
		else
		{
			if (warpTargetBuffer != null)
			{
				Warp(warpTargetBuffer);
				warpTargetBuffer = null;
			}

			if (dashTargetBuffer != null)
			{
				Dash(dashTargetBuffer);
				dashTargetBuffer = null;
			}
		}

		if (inputDirection.X != 0)
		{
			if (WallStickable && WallTouch == -inputDirection.X)
				wallStick = 0.3f;

			if (wallStick <= 0) currentAcceleration += Acceleration * inputDirection.X;
		}

		oldInputDirection = inputDirection;

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
	}

	public void LocalInput()
	{
		PlayerInput oldInput = new PlayerInput(inputData);

		if (Program.focused)
		{
			inputData[PlayerKey.Right] = KeyboardInput.Current[Key.Right];
			inputData[PlayerKey.Left] = KeyboardInput.Current[Key.Left];
		}
		else
		{
			inputData[PlayerKey.Right] = KeyboardInput.Current[Key.Left];
			inputData[PlayerKey.Left] = KeyboardInput.Current[Key.Right];
		}

		//inputData[PlayerKey.Up] = KeyboardInput.Current[Key.Up];
		inputData[PlayerKey.Down] = KeyboardInput.Current[Key.Down];
		inputData[PlayerKey.Jump] = KeyboardInput.Current[Key.Z];
		inputData[PlayerKey.Warp] = KeyboardInput.Current[Key.X];

		for (int i = 0; i < inputData.Length; i++)
			if (inputData[i] != oldInput[i])
			{
				if (IsDashing || IsWarping)
					SendInputPure();
				else
					SendInput();
	
				break;
			}
	}

	public override void Draw()
	{
		if (WallTouch != 0 && !IsOnGround)
		{
			playerTileset.X = 0;
		}
		else if (IsOnGround)
		{
			if (Math.Abs(velocity.X) > 1f)
			{
				playerTileset.X = 1;
			}
			else
			{
				playerTileset.X = 3;
			}
		}
		else
		{
			if (velocity.Y > 0) playerTileset.X = 4;
			else playerTileset.X = 2;
		}

		mesh.Color = MyColor;
		mesh.FillColor = true;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(size);

		if (warpTarget != null)
		{
			mesh.Rotate(warpTarget.angle);
			mesh.Scale(1 + warpTarget.lastStep * 8.5f, 1 - warpTarget.lastStep * 2.7f);
			mesh.Rotate(-warpTarget.angle);
		}

		if (dashTarget != null)
		{
			mesh.Rotate(dashTarget.angle);
			mesh.Scale(1 + dashTarget.lastStep * 1.5f, 1 - dashTarget.lastStep * 0.7f);
			mesh.Rotate(-dashTarget.angle);
		}

		mesh.Scale(new Vector2(-dir, 1));

		mesh.Draw(playerTileset);

		if (!warpCooldown.IsDone)
		{
			mesh.Color = new Color(1, 1, 1, 1 - warpCooldown.PercentageDone);
			mesh.FillColor = true;

			mesh.Draw(playerTileset);
		}

		if (!dashCooldown.IsDone)
		{
			mesh.Color = new Color(1, 1, 1, 1 - dashCooldown.PercentageDone);
			mesh.FillColor = true;

			mesh.Draw(playerTileset);
		}

		if (Disabled)
		{
			mesh.Color = new Color(0.3f, 0.3f, 1f, 1 - disableTimer.PercentageDone);
			mesh.FillColor = true;

			mesh.Draw(playerTileset);
		}

		if (receivedServerPosition)
		{
			mesh.Color = new Color(0, 1, 0, 0.5f);
			mesh.FillColor = true;

			mesh.Reset();

			mesh.Translate(serverPosition);
			mesh.Scale(size);
			mesh.Scale(new Vector2(-dir, 1));

			mesh.Draw(playerTileset);
		}

		//if (shadow != null && IsLocalPlayer && (warpCooldown.IsDone || IsWarping) && !Disabled) shadow.Draw();
		if (IsLocalPlayer && (CanWarp || IsWarping)) shadow.Draw();
	}
}