using OpenTK;
using OpenTK.Input;
using EZUDP;
using TKTools;

public class NetworkPlayer : Player
{
	public int playerID;
	public bool IsLocalPlayer
	{
		get
		{
			return map.myID == playerID;
		}
	}

	Vector2 serverPosition = Vector2.Zero;

	public NetworkPlayer(int id, Vector2 position, Map map)
		: base(position, map)
	{
		playerID = id;
	}

	public override void LocalInput()
	{
		PlayerInput oldInput = new PlayerInput(inputData);

		base.LocalInput();

		for (int i = 0; i < inputData.Length; i++)
			if (inputData[i] != oldInput[i])
			{
				SendInput();
				break;
			}
	}

	public void ReceiveInput(Vector2 position, Vector2 velocity, byte k)
	{
		if (IsLocalPlayer)
		{
			serverPosition = position;
			return;
		}

		this.position = position;
		this.velocity = velocity;
		inputData.DecodeFlag(k);
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		if (IsLocalPlayer)
		{
			serverPosition = position;
			return;
		}

		//this.position = position;
		//this.velocity = velocity;
	}

	public void SendInput()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInput);
		msg.WriteVector(position);
		msg.WriteVector(velocity);
		msg.WriteByte(inputData.GetFlag());

		Game.client.Send(msg);
	}

	public void SendPosition()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		Game.client.Send(msg);
	}

	public override void Draw()
	{
		mesh.Color = colorList[playerID];
		mesh.Texture = textureList[tex];

		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(position);

		mesh.Draw();

		mesh.Color = new Color(1, 1, 1, 0.5f);

		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(serverPosition);

		mesh.Draw();
	}
}