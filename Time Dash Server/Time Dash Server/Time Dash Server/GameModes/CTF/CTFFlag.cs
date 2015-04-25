using EZUDP;
using OpenTK;
using TKTools;

public class CTFFlag : Entity
{
	protected new CTFMap Map
	{
		get
		{
			return (CTFMap)base.Map;
		}
	}

	public CTFPlayer holder;
	Vector2 originPosition;
	public int ownerID;

	Vector2 velocity = Vector2.Zero;

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
	}

	public override void Logic()
	{
		if (holder == null)
		{
			foreach (CTFPlayer p in Map.playerList)
				if (p != null)
				{
					if (p.CollidesWith(Position, 0.5f))
					{
						if (p.Team == OwnerTeam && !IsInBase)
							Return();
						if (p.Team != OwnerTeam)
							p.GrabFlag(this);
					}
				}
		}
		else
		{
			CTFFlag otherFlag = Map.flags[(ownerID + 1) % 2];
			if (otherFlag.IsInBase && holder.CollidesWith(otherFlag.Position, 0.5f))
				Capture();
		}

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

		base.Position += velocity * Game.delta;
	}

	public void Drop()
	{
		if (holder == null) return;

		base.Position = holder.Position + new Vector2(0, 2f);
		velocity = TKMath.GetAngleVector(90 + ((float)Map.rng.NextDouble() - 0.5f) * 2 * 70f) * 10f;
		holder = null;

		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CTF.FlagDropped);
		msg.WriteByte(ownerID);

		msg.WriteVector(base.Position);
		msg.WriteVector(velocity);

		Map.SendToAllPlayers(msg);
	}

	public void Capture()
	{
		Map.FlagCaptured(this);
		Return();

		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CTF.FlagCaptured);
		msg.WriteByte(ownerID);

		Map.SendToAllPlayers(msg);
	}

	public void Return()
	{
		holder = null;
		base.Position = originPosition;

		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CTF.FlagReturned);
		msg.WriteByte(ownerID);

		Map.SendToAllPlayers(msg);
	}

	public void SendExistenceToPlayer(params Player[] players)
	{
		if (holder != null)
			holder.SendFlagGrabToPlayer(players);
		else
			SendPositionToPlayer(players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CTF.FlagPosition);
		msg.WriteByte(ownerID);

		msg.WriteVector(Position);
		msg.WriteVector(velocity);

		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}
}