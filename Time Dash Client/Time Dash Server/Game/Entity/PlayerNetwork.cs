using EZUDP;
using OpenTK;

public partial class Player : Actor
{
	public static bool SEND_SERVER_POSITION = false;

	DodgeTarget dodgeTargetBuffer;
	DashTarget dashTargetBuffer;

	public void ReceiveInput(byte k)
	{
		inputData.DecodeFlag(k);
		SendInputPureToPlayer(map.playerList);
	}
	public void ReceiveInput(Vector2 position, Vector2 velocity, byte k)
	{
		if (!IsDodging && !IsDashing)
		{
			this.position = position;
			this.velocity = velocity;
		}

		inputData.DecodeFlag(k);
		SendInputToPlayer(map.playerList);
	}

	public void ReceiveJump(Vector2 position)
	{
		this.position = position;

		if (IsOnGround) Jump();
		else if (WallTouch != 0) WallJump();
		else if (canDoublejump)
		{
			Jump();
			canDoublejump = false;
		}

		SendJumpToPlayer(map.playerList);
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
		if (dodgeTarget != null) dodgeTarget = null;
		SendPositionToPlayer(map.playerList);
	}

	public void ReceiveLand(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
		Land();
	}

	public void ReceiveDodge(Vector2 start, Vector2 target)
	{
		dodgeTargetBuffer = new DodgeTarget(start, target);

		SendDodgeToPlayer(start, target, map.playerList);
	}

	public void ReceiveDash(Vector2 start, Vector2 target)
	{
		dashTargetBuffer = new DashTarget(start, target);

		SendDashToPlayer(start, target, map.playerList);
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

	public void SendJumpToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetJumpMessage(), true, players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), true, players);
	}

	public void SendPositionToPlayerForce(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), false, players);
	}

	public void SendKillToPlayer(Player target, params Player[] players)
	{
		SendMessageToPlayer(GetKillMessage(target), false, players);
	}

	public void SendDieToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetDieMessage(), false, players);
	}

	public void SendRespawnToPlayer(Vector2 pos, params Player[] players)
	{
		SendMessageToPlayer(GetRespawnMessage(pos), false, players);
	}

	public void SendDisableToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetDisableMessage(), false, players);
	}

	public void SendDodgeToPlayer(Vector2 start, Vector2 target, params Player[] players)
	{
		SendMessageToPlayer(GetDodgeMessage(start, target), true, players);
	}

	public void SendDashToPlayer(Vector2 start, Vector2 target, params Player[] players)
	{
		SendMessageToPlayer(GetDashMessage(start, target), true, players);
	}

	public void SendDodgeCollisionToPlayer(Player p, params Player[] players)
	{
		SendMessageToPlayer(GetDodgeCollisionMessage(p), false, players);
	}

	public void SendDashCollisionToPlayer(Player p, params Player[] players)
	{
		SendMessageToPlayer(GetDashCollisionMessage(p), false, players);
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
		msg.WriteByte(id);
		msg.WriteString(playerName);

		return msg;
	}

	MessageBuffer GetLeaveMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerLeave);
		msg.WriteByte(id);

		return msg;
	}

	MessageBuffer GetInputMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInput);
		msg.WriteByte(id);
		msg.WriteVector(position);
		msg.WriteVector(velocity);
		msg.WriteByte(inputData.GetFlag());

		return msg;
	}

	MessageBuffer GetInputPureMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInputPure);
		msg.WriteByte(id);
		msg.WriteByte(inputData.GetFlag());

		return msg;
	}

	MessageBuffer GetJumpMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerJump);
		msg.WriteByte(id);
		msg.WriteVector(position);

		return msg;
	}

	MessageBuffer GetPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
		msg.WriteByte(id);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		return msg;
	}

	MessageBuffer GetKillMessage(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerKill);
		msg.WriteByte(id);
		msg.WriteByte(p.id);

		return msg;
	}

	MessageBuffer GetDieMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDie);
		msg.WriteByte(id);
		msg.WriteVector(position);

		return msg;
	}

	MessageBuffer GetRespawnMessage(Vector2 pos)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerRespawn);
		msg.WriteByte(id);
		msg.WriteVector(pos);

		return msg;
	}

	MessageBuffer GetDisableMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDisable);
		msg.WriteByte(id);

		return msg;
	}

	MessageBuffer GetDodgeMessage(Vector2 start, Vector2 target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDodge);
		msg.WriteByte(id);

		msg.WriteVector(start);
		msg.WriteVector(target);

		return msg;
	}

	MessageBuffer GetDashMessage(Vector2 start, Vector2 target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDash);
		msg.WriteByte(id);

		msg.WriteVector(start);
		msg.WriteVector(target);

		return msg;
	}

	MessageBuffer GetDodgeCollisionMessage(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDodgeCollision);
		msg.WriteByte(id);
		msg.WriteByte(p.id);
		msg.WriteVector(position);
		msg.WriteVector(p.position);

		return msg;
	}

	MessageBuffer GetDashCollisionMessage(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDashCollision);
		msg.WriteByte(id);
		msg.WriteByte(p.id);
		msg.WriteVector(position);
		msg.WriteVector(p.position);

		return msg;
	}

	MessageBuffer GetServerPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.ServerPosition);
		msg.WriteByte(id);

		msg.WriteVector(position);

		return msg;
	}
}