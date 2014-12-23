using OpenTK;
using OpenTK.Input;
using EZUDP;
using TKTools;

public partial class Player
{
	DashTarget dashTargetBuffer;
	WarpTarget warpTargetBuffer;

	public int playerID;
	public bool IsLocalPlayer
	{
		get
		{
			return map.myID == playerID;
		}
	}

	bool receivedServerPosition = false;
	Vector2 serverPosition = Vector2.Zero;

	public void ReceiveInput(byte k)
	{
		inputData.DecodeFlag(k);
	}
	public void ReceiveInput(Vector2 position, Vector2 velocity, byte k)
	{
		if (!IsDashing && !IsWarping)
		{
			this.position = position;
			this.velocity = velocity;
		}

		inputData.DecodeFlag(k);
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
	}

	public void ReceiveServerPosition(Vector2 position)
	{
		receivedServerPosition = true;
		serverPosition = position;
	}

	public void ReceiveDash(Vector2 start, Vector2 target)
	{
		dashTargetBuffer = new DashTarget(start, target);
	}

	public void ReceiveDashCollision(byte player, Vector2 myPos, Vector2 colPos)
	{
		Player p = map.playerList[player];

		position = myPos;
		p.position = colPos;

		DashEnd(p);
		p.DashEnd(this);

		map.AddEffect(new EffectCollision(this, p, map));
	}

	public void ReceiveWarpCollision(byte player, Vector2 myPos, Vector2 colPos)
	{
		Player p = map.playerList[player];

		position = myPos;
		p.position = colPos;

		WarpEnd(p);
		p.WarpEnd(this);

		map.AddEffect(new EffectCollision(this, p, map));
	}

	public void ReceiveWarp(Vector2 start, Vector2 target)
	{
		warpTargetBuffer = new WarpTarget(start, target);
	}

	void SendInput()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInput);
		msg.WriteVector(position);
		msg.WriteVector(velocity);
		msg.WriteByte(inputData.GetFlag());

		Game.client.Send(msg);
	}

	void SendInputPure()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInputPure);
		msg.WriteByte(inputData.GetFlag());

		Game.client.Send(msg);
	}

	void SendPosition()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		Game.client.Send(msg);
	}

	void SendLand()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerLand);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		Game.client.Send(msg);
	}

	void SendDash(DashTarget target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDash);
		msg.WriteVector(target.startPosition);
		msg.WriteVector(target.endPosition);

		Game.client.Send(msg);
	}

	void SendWarp(WarpTarget target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerWarp);
		msg.WriteVector(target.startPosition);
		msg.WriteVector(target.endPosition);

		Game.client.Send(msg);
	}
}