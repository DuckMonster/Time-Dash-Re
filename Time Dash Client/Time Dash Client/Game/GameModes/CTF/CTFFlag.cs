using OpenTK;
using System;
using TKTools;
public class CTFFlag : Entity
{
	public Player holder;
	Vector2 originPosition;
	public int ownerID;

	Vector2 velocity = Vector2.Zero;

	Texture flagTexture;

	public Team OwnerTeam
	{
		get
		{
			return map.teamList[ownerID];
		}
	}

	public Vector2 Position
	{
		get
		{
			if (holder == null) return position;
			else return holder.position + new Vector2(0f, 1.2f);
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

		flagTexture = new Texture("Res/flag.png");
		mesh.Texture = flagTexture;
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

		if (map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
			velocity.Y *= -0.4f;
		if (map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
			velocity.X *= -0.2f;
		if (map.GetCollision(this, velocity * Game.delta))
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
		position = holder.position + new Vector2(0, 2f);
		velocity = TKMath.GetAngleVector(90 + ((float)map.rng.NextDouble() - 0.5f) * 70f) * 20f;
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
		((CTFMap)map).FlagCaptured(this);
		Return();
	}

	public void Return()
	{
		holder = null;
		position = originPosition;
	}

	public override void Draw()
	{
		mesh.Color = Player.colorList[ownerID];

		mesh.Reset();
		mesh.Translate(Position);
		mesh.Draw();
	}
}