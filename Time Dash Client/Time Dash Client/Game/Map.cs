using System;

using OpenTK;
using TKTools;
using EZUDP;
using System.Collections.Generic;

public class Map
{
	public static ShaderProgram defaultProgram = new ShaderProgram("Shaders/standardShader.glsl");
	public int myID;
	Camera camera;
	Environment environment;

	public Player[] playerList = new Player[10];
	List<Effect> effectList = new List<Effect>(), effectBufferList = new List<Effect>();

	public void AddEffect(Effect e)
	{
		effectList.Add(e);
		effectBufferList.Add(e);
	}
	public void RemoveEffect(Effect e)
	{
		effectList.Remove(e);
		e.Dispose();
	}

	public Player LocalPlayer
	{
		get
		{
			return playerList[myID];
		}
	}

	public void PlayerJoin(int id)
	{
		playerList[id] = new Player(id, new Vector2(4, 10), this);
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

	public Player GetPlayerAtPos(Vector2 pos, Vector2 size, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>();
		excludeList.AddRange(exclude);

		foreach (Player p in playerList) if (!excludeList.Contains(p) && p != null && p.CollidesWith(pos, size)) return p;
		return null;
	}

	public bool RayTraceCollision(Vector2 start, Vector2 end, Vector2 size, out Vector2 freepos)
	{
		Vector2 diffVector = end - start, directionVector = diffVector.Normalized();

		int accuracy = 1 + (int)(diffVector.Length * 6);
		float step = diffVector.Length / accuracy;
		Vector2 checkpos = start;

		for (int i = 0; i < accuracy; i++)
		{
			Vector2 buffer = checkpos;
			buffer += directionVector * step;

			if (GetCollision(buffer, size))
			{
				freepos = checkpos;
				return true;
			}

			checkpos = buffer;
		}

		freepos = end;
		return false;
	}

	public List<Player> RayTrace(Vector2 start, Vector2 end, Vector2 size, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>();
		excludeList.AddRange(exclude);
		List<Player> returnList = new List<Player>();

		Vector2 diffVector = end - start, directionVector = diffVector.Normalized();

		int accuracy = (int)(diffVector.Length * 6);
		float step = diffVector.Length / accuracy;
		Vector2 checkpos = start;

		for (int i = 0; i < accuracy; i++)
		{
			Player p = GetPlayerAtPos(checkpos, size, excludeList.ToArray());
			if (p != null && !returnList.Contains(p))
			{
				returnList.Add(p);
				excludeList.Add(p);
			}

			checkpos += directionVector * step;
		}

		return returnList;
	}

	public void Logic()
	{
		camera.Logic();
		environment.Logic();
		 
		if (LocalPlayer != null) LocalPlayer.LocalInput();
		foreach (Player p in playerList) if (p != null) p.Logic();
		foreach (Effect e in effectBufferList) e.Logic();

		if (!effectList.Equals(effectBufferList))
		{
			effectBufferList.Clear();
			effectBufferList.AddRange(effectList.ToArray());
		}
	}

	public void Draw()
	{
		defaultProgram["view"].SetValue(camera.ViewMatrix);
		environment.Draw();
		foreach (Player p in playerList) if (p != null) p.Draw();
		foreach (Effect e in effectList) e.Draw();
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

				case Protocol.PlayerDie:
					playerList[msg.ReadByte()].Die(msg.ReadVector2());
					break;

				case Protocol.PlayerDisable:
					playerList[msg.ReadByte()].Disabled = true;
					break;

				case Protocol.PlayerDash:
					playerList[msg.ReadByte()].ReceiveDash(msg.ReadVector2(), msg.ReadVector2());
					break;

				case Protocol.PlayerWarp:
					playerList[msg.ReadByte()].ReceiveWarp(msg.ReadVector2(), msg.ReadVector2());
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