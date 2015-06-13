using OpenTK;
using System;
using TKTools;
using TKTools.Mathematics;

public class CTFFlag : Entity
{
	protected new CTFMap Map
	{
		get
		{
			return (CTFMap)base.Map;
		}
	}

	public Player holder;
	Vector2 originPosition;
	public int ownerID;

	Vector2 velocity = Vector2.Zero;

	Texture flagTexture;

	public Team OwnerTeam
	{
		get
		{
			return Map.teamList[ownerID];
		}
	}

	public override Vector2 Position
	{
		get
		{
			if (holder == null) return base.Position;
			else return holder.Position + new Vector2(0f, 1.2f);
		}
	}

	public bool IsInBase
	{
		get
		{
			return Position == originPosition;
		}
	}

	public CTFFlag(int teamID, Vector2 position, Map map)
		: base(position, map)
	{
		ownerID = teamID;
		originPosition = position;

		size = new Vector2(1, 1);

		flagTexture = Art.Load("Res/flag.png");
		sprite.Texture = flagTexture;
	}

	public override void Dispose()
	{
		flagTexture.Dispose();

		base.Dispose();
	}

	public override void Logic()
	{
		if (holder == null && !IsInBase)
			DoPhysics();

		base.Logic();
	}

	public void DoPhysics()
	{
		velocity.Y -= Stats.defaultStats.Gravity * 0.5f * Game.delta;
		if (velocity.Y < -20) velocity.Y = -20;

		velocity.X -= velocity.X * 1.2f * Game.delta;

		if (Map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
			velocity.Y *= -0.4f;
		if (Map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
			velocity.X *= -0.2f;
		if (Map.GetCollision(this, velocity * Game.delta))
			velocity = Vector2.Zero;

		position += velocity * Game.delta;
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
	}

	public void Drop()
	{
		position = holder.Position + new Vector2(0, 2f);
		velocity = TKMath.GetAngleVector(90 + ((float)Map.rng.NextDouble() - 0.5f) * 70f) * 20f;
		holder = null;
	}

	public void Drop(Vector2 pos, Vector2 velo)
	{
		position = pos;
		velocity = velo;

		holder = null;
	}

	public void Capture()
	{
		Map.FlagCaptured(this);
		Return();
	}

	public void Return()
	{
		holder = null;
		position = originPosition;
	}

	public override void Draw()
	{
		sprite.Color = Player.colorList[ownerID];
		sprite.Draw(Position, 0f, 0f);
	}
}