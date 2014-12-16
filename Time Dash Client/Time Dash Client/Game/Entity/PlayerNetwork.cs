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

	Vector2 serverPosition = Vector2.Zero;

	public void ReceiveInput(Vector2 position, Vector2 velocity, byte k)
	{
		ReceivePosition(position, velocity);
		inputData.DecodeFlag(k);

		Log.Write("Received input");
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		serverPosition = position;

		this.position = position;
		this.velocity = velocity;
	}

	public void ReceiveDash(Vector2 start, Vector2 target)
	{
		dashTargetBuffer = new DashTarget(start, target);
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

	void SendPosition()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
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