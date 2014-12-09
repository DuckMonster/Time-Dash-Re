using System;

using OpenTK;
using TKTools;
using EZUDP;

public class Map
{
	public static ShaderProgram defaultProgram = new ShaderProgram("Shaders/standardShader.glsl");
	public int myID;
	Camera camera;
	Environment environment;

	public Player[] playerList = new Player[10];

	public Player LocalPlayer
	{
		get
		{
			return playerList[myID];
		}
	}

	public void PlayerJoin(int id)
	{
		playerList[id] = new Player(id, new Vector2(4, 4), this);
		Log.Write("Player " + id + " joined!");
	}

	public void PlayerLeave(int id)
	{
		playerList[id] = null;
	}

	public Map(int id)
	{
		myID = id;
		Log.Write("My id is " + id);

		environment = new Environment(this);
		camera = new Camera(this);
	}

	public bool GetCollision(Entity e) { return GetCollision(e.position, e.size); }
	public bool GetCollision(Entity e, Vector2 offset) { return GetCollision(e.position + offset, e.size); }
	public bool GetCollision(Vector2 pos, Vector2 size)
	{
		return environment.GetCollision(pos, size);
	}

	public void Logic()
	{
		camera.Logic();
		environment.Logic();

		if (LocalPlayer != null) LocalPlayer.LocalInput();
		foreach (Player p in playerList) if (p != null) p.Logic();
	}

	public void Draw()
	{
		defaultProgram["view"].SetValue(camera.ViewMatrix);
		environment.Draw();
		foreach (Player p in playerList) if (p != null) p.Draw();
	}

	//ONLINE
	public void MessageHandle(MessageBuffer msg)
	{
		try
		{
			switch ((Protocol)msg.ReadShort())
			{
				case Protocol.PlayerJoin:
					PlayerJoin(msg.ReadByte());
					break;

				case Protocol.PlayerLeave:
					PlayerLeave(msg.ReadByte());
					break;

				case Protocol.PlayerInput:
					playerList[msg.ReadByte()].ReceiveInput(msg.ReadVector2(), msg.ReadVector2(), msg.ReadByte());
					break;

				case Protocol.PlayerPosition:
					playerList[msg.ReadByte()].ReceivePosition(msg.ReadVector2(), msg.ReadVector2());
					break;
			}

			msg.Reset();
		}
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Red, "Packet corrupt!\n" + e.Message);
		}
	}
}