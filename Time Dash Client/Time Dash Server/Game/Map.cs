using System;

using OpenTK;
using EZUDP.Server;
using EZUDP;
using System.Collections.Generic;

public class Map
{
	Environment environment;

	public Player[] playerList = new Player[10];
	public Player GetPlayer(int id)
	{
		return playerList[id];
	}
	public Player GetPlayer(Client c)
	{
		foreach (Player p in playerList) if (p != null && p.client == c) return p;
		return null;
	}
	public int GetPlayerID(Player p)
	{
		for (int i = 0; i < playerList.Length; i++) if (playerList[i] == p) return i;
		return -1;
	}
	public int GetPlayerID(Client c)
	{
		for (int i = 0; i < playerList.Length; i++) if (playerList[i] != null && playerList[i].client == c) return i;
		return -1;
	}

	public int GetFreePlayerSlot()
	{
		for (int i = 0; i < playerList.Length; i++) if (playerList[i] == null) return i;
		return -1;
	}

	public void PlayerJoin(Client c)
	{
		int id = GetFreePlayerSlot();
		if (id != -1) playerList[id] = new Player(id, c, new Vector2(10, 30), this);

		MessageBuffer msg = new MessageBuffer();
		msg.WriteShort((short)Protocol.EnterMap);
		msg.WriteByte(id);
		c.Send(msg);

		foreach (Player p in playerList)
		{
			if (p == null) continue;

			if (p == playerList[id])
				p.SendExistanceToPlayer(playerList);
			else
				p.SendExistanceToPlayer(playerList[id]);
		}
	}

	public void PlayerLeave(Client c)
	{
		int id = GetPlayerID(c);
		if (id != -1) playerList[id] = null;

		MessageBuffer msg = new MessageBuffer();
		msg.WriteShort((short)Protocol.PlayerLeave);
		msg.WriteByte(id);
		SendToAllPlayers(msg);
	}

	public Map()
	{
		environment = new Environment(this);
	}

	public bool GetCollision(Entity e) { return GetCollision(e.position, e.size); }
	public bool GetCollision(Entity e, Vector2 offset) { return GetCollision(e.position + offset, e.size); }
	public bool GetCollision(Vector2 pos, Vector2 size)
	{
		return environment.GetCollision(pos, size);
	}

	public void Logic()
	{
		environment.Logic();
		foreach (Player p in playerList) if (p != null) p.Logic();
	}

	public Player GetPlayerAtPos(Vector2 pos, Vector2 size, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>();
		excludeList.AddRange(exclude);

		foreach (Player p in playerList) if (!excludeList.Contains(p) && p != null && p.CollidesWith(pos, size)) return p;
		return null;
	}

	public void SendToAllPlayers(MessageBuffer msg, params Player[] exceptions)
	{
		List<Player> exceptionList = new List<Player>();
		exceptionList.AddRange(exceptions);

		foreach (Player p in playerList) if (p != null && !exceptionList.Contains(p)) p.client.Send(msg);
	}

	public void MessageHandle(Client c, MessageBuffer msg)
	{
		try
		{
			Player p = GetPlayer(c);

			if (p != null)
			{
				switch ((Protocol)msg.ReadShort())
				{
					case Protocol.PlayerInput:
						p.ReceiveInput(msg.ReadVector2(), msg.ReadVector2(), msg.ReadByte());
						break;

					case Protocol.PlayerPosition:
						p.ReceivePosition(msg.ReadVector2(), msg.ReadVector2());
						break;
				}
			}

			msg.Reset();
		}
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Red, "Packet corrupt!\n" + e.Message);
		}
	}
}