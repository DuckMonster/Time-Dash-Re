using EZUDP;
using OpenTK;

public partial class Player : Actor
{
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

	public void ReceiveDash(Vector2 start, Vector2 target)
	{
		this.position = start;
		Dash(target);

		SendDashToPlayer(start, target, map.playerList);

		Log.Write("DASHING");
	}

	public void ReceiveWarp(Vector2 start, Vector2 target)
	{
		this.position = start;
		Warp(target);

		SendWarpToPlayer(start, target, map.playerList);

		Log.Write("WARPING");
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

	public void SendPositionToPlayer(params Player[] players)
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
}