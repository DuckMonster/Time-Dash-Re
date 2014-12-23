using EZUDP;
using OpenTK;

public partial class Player : Actor
{
	public static bool SEND_SERVER_POSITION = false;

	DashTarget dashTargetBuffer;
	WarpTarget warpTargetBuffer;

	public void ReceiveInput(byte k)
	{
		inputData.DecodeFlag(k);
		SendInputPureToPlayer(map.playerList);
	}
	public void ReceiveInput(Vector2 position, Vector2 velocity, byte k)
	{
		if (!IsDashing && !IsWarping)
		{
			this.position = position;
			this.velocity = velocity;
		}

		inputData.DecodeFlag(k);
		SendInputToPlayer(map.playerList);
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
		if (dashTarget != null) dashTarget = null;
		SendPositionToPlayer(map.playerList);
	}

	public void ReceiveLand(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
		Land();
	}

	public void ReceiveDash(Vector2 start, Vector2 target)
	{
		dashTargetBuffer = new DashTarget(start, target);

		SendDashToPlayer(start, target, map.playerList);
	}

	public void ReceiveWarp(Vector2 start, Vector2 target)
	{
		warpTargetBuffer = new WarpTarget(start, target);

		SendWarpToPlayer(start, target, map.playerList);
	}

	public void SendExistanceToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetExistanceMessage(), false, players);
		SendPositionToPlayer(players);
	}

	public void SendLeaveToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetLeaveMessage(), false, players);
	}

	public void SendInputToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetInputMessage(), true, players);
	}

	public void SendInputPureToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetInputPureMessage(), true, players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), true, players);
	}

	public void SendPositionToPlayerForce(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), false, players);
	}

	public void SendDieToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetDieMessage(), false, players);
	}

	public void SendDisableToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetDisableMessage(), false, players);
	}

	public void SendDashToPlayer(Vector2 start, Vector2 target, params Player[] players)
	{
		SendMessageToPlayer(GetDashMessage(start, target), true, players);
	}

	public void SendWarpToPlayer(Vector2 start, Vector2 target, params Player[] players)
	{
		SendMessageToPlayer(GetWarpMessage(start, target), true, players);
	}

	public void SendDashCollisionToPlayer(Player p, params Player[] players)
	{
		SendMessageToPlayer(GetDashCollisionMessage(p), false, players);
	}

	public void SendWarpCollisionToPlayer(Player p, params Player[] players)
	{
		SendMessageToPlayer(GetWarpCollisionMessage(p), false, players);
	}

	public void SendServerPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetServerPositionMessage(), false, players);
	}

	void SendMessageToPlayer(MessageBuffer msg, bool excludeSelf, params Player[] players)
	{
		foreach (Player p in players) if (p != null && !(excludeSelf && p == this)) p.client.Send(msg);
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

	MessageBuffer GetInputPureMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInputPure);
		msg.WriteByte(playerID);
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

	MessageBuffer GetDieMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDie);
		msg.WriteByte(playerID);
		msg.WriteVector(position);

		return msg;
	}

	MessageBuffer GetDisableMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDisable);
		msg.WriteByte(playerID);

		return msg;
	}

	MessageBuffer GetDashMessage(Vector2 start, Vector2 target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDash);
		msg.WriteByte(playerID);

		msg.WriteVector(start);
		msg.WriteVector(target);

		return msg;
	}

	MessageBuffer GetWarpMessage(Vector2 start, Vector2 target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerWarp);
		msg.WriteByte(playerID);

		msg.WriteVector(start);
		msg.WriteVector(target);

		return msg;
	}

	MessageBuffer GetDashCollisionMessage(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDashCollision);
		msg.WriteByte(playerID);
		msg.WriteByte(p.playerID);
		msg.WriteVector(position);
		msg.WriteVector(p.position);

		return msg;
	}

	MessageBuffer GetWarpCollisionMessage(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerWarpCollision);
		msg.WriteByte(playerID);
		msg.WriteByte(p.playerID);
		msg.WriteVector(position);
		msg.WriteVector(p.position);

		return msg;
	}

	MessageBuffer GetServerPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.ServerPosition);
		msg.WriteByte(playerID);

		msg.WriteVector(position);

		return msg;
	}
}