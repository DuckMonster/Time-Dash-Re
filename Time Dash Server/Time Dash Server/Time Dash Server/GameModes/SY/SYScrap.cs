using EZUDP;
using OpenTK;
using System;
using TKTools;
public class SYScrap : Entity
{
	static Random rng = new Random();

	protected new SYMap Map
	{
		get { return (SYMap)base.Map; }
	}

	public int id;
	Vector2 velocity;

	public bool Grabbable
	{
		get
		{
			return velocity.LengthFast < 10f;
		}
	}

	public SYScrap(int id, Vector2 position, Map map)
		: base(position, map)
	{
		this.id = id;
		
		size = new Vector2(0.5f, 0.5f);

		DropAt(position);
	}

	public void DropAt(Vector2 pos)
	{
		float angle = (float)rng.NextDouble() * 360f;
		float velo = 15f + (float)rng.NextDouble() * 15f;

		position = pos;
		velocity = TKMath.GetAngleVector(angle) * velo;

		SendExistanceToPlayer(Map.playerList);
	}

	public void CollectedBy(Player p)
	{
		SendCollectToPlayer(p.id, Map.playerList);
		Map.RemoveScrap(this);
	}

	public override void Logic()
	{
		base.Logic();

		position += velocity * Game.delta;

		float oldVelocity = velocity.Length;
		velocity -= velocity * 6f * Game.delta;

		if (velocity.Length < 0.1 && oldVelocity >= 0.1f)
			SendPositionToPlayer(Map.playerList);

		if (velocity.LengthFast > 0.5f)
		{
			if (Map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
			{
				velocity.X *= -0.6f;
				SendPositionToPlayer(Map.playerList);
			}
			if (Map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
			{
				velocity.Y *= -0.6f;
				SendPositionToPlayer(Map.playerList);
			}
		}
	}

	public void SendExistanceToPlayer(params Player[] players)
	{
		SendMessageToPlayers(GetExistanceMessage(), players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayers(GetPositionMessage(), players);
	}

	public void SendCollectToPlayer(int playerID, params Player[] players)
	{
		SendMessageToPlayers(GetCollectMessage(playerID), players);
	}

	public void SendMessageToPlayers(MessageBuffer msg, Player[] players)
	{
		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}

	MessageBuffer GetExistanceMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.ScrapExistance);
		msg.WriteShort(id);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		return msg;
	}

	MessageBuffer GetPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.ScrapPosition);
		msg.WriteShort(id);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		return msg;
	}

	MessageBuffer GetCollectMessage(int playerID)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.ScrapCollect);
		msg.WriteShort(id);
		msg.WriteByte(playerID);

		return msg;
	}
}