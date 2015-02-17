using System;

using OpenTK;
using EZUDP.Server;
using EZUDP;
using System.Collections.Generic;

using MapScene;
using TKTools;

public class Map
{
	public static Map currentMap;
	public Random rng = new Random();
	protected Scene scene;

	public string filename;
	public GameMode mode;

	protected Player winPlayer;
	Timer winTimer = new Timer(4f, false);

	public Team[] teamList = new Team[10];

	public Player[] playerList = new Player[10];

	public int NumberOfPlayers
	{
		get
		{
			int n = 0;
			foreach (Player p in playerList) if (p != null) n++;

			return n;
		}
	}

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

	public virtual Player PlayerJoin(Client c, string name)
	{
		int id = GetFreePlayerSlot();

		playerList[id] = CreatePlayer(id, name, c);
		playerList[id].position = GetFreeSpawnPosition(playerList[id]);

		MessageBuffer msg = new MessageBuffer();
		msg.WriteShort((short)Protocol.EnterMap);
		msg.WriteByte(id);
		msg.WriteString(filename);
		msg.WriteByte((byte)mode);
		c.Send(msg);

		foreach (Player p in playerList)
		{
			if (p == null) continue;

			if (p == playerList[id])
				p.SendExistanceToPlayer(playerList);
			else
				p.SendExistanceToPlayer(playerList[id]);
		}

		playerList[id].SendPositionToPlayerForce(playerList);

		foreach (Team t in teamList)
		{
			if (t != null)
				t.SendMemberListToPlayer(playerList[id]);
		}

		Log.Write(ConsoleColor.Yellow, name + " joined!");

		return playerList[id];
	}

	public virtual Player CreatePlayer(int id, string name, Client c)
	{
		return new Player(id, name, c, Vector2.Zero, this);
	}

	public void PlayerJoinTeam(int id, int team) { if (playerList[id] == null) return; PlayerJoinTeam(playerList[id], team); }
	public void PlayerJoinTeam(Player p, int team)
	{
		if (teamList[team] == null) teamList[team] = new Team(team, this);

		teamList[team].AddMember(p);
	}

	public virtual void PlayerLeave(Client c)
	{
		int id = GetPlayerID(c);
		if (id != -1) playerList[id] = null;

		MessageBuffer msg = new MessageBuffer();
		msg.WriteShort((short)Protocol.PlayerLeave);
		msg.WriteByte(id);
		SendToAllPlayers(msg);
	}

	public Map(string filename, GameMode mode)
	{
		this.filename = filename;
		this.mode = mode;

		currentMap = this;
		scene = new Scene(filename, this);
	}

	public Map(string filename, GameMode mode, Player[] players)
	{
		this.filename = filename;
		this.mode = mode;

		currentMap = this;
		scene = new Scene(filename, this);

		foreach (Player p in players)
		{
			if (p != null)
				PlayerJoin(p.client, p.playerName);
		}
	}

	public void PlayerWin(Player p)
	{
		if (winPlayer != null) return;
		winPlayer = p;

		SendPlayerWin(p);
	}

	public void TeamWin(Team t)
	{
		if (winPlayer != null) return;

		winPlayer = t.memberList[0];

		SendTeamWin(t);
	}

	public virtual void SceneZone(int typeID, Polygon p)
	{

	}

	public bool GetCollision(Entity e) { return GetCollision(e.position, e.size); }
	public bool GetCollision(Entity e, Vector2 offset) { return GetCollision(e.position + offset, e.size); }
	public bool GetCollision(Vector2 pos, Vector2 size)
	{
		return scene.GetCollision(pos, size);
	}

	public virtual void Logic()
	{
		scene.Logic();
		foreach (Player p in playerList) if (p != null) p.Logic();

		if (winPlayer != null)
		{
			winTimer.Logic();
			if (winTimer.IsDone) Game.currentGame.LoadMap(filename);
		}
	}

	public Player RayTrace(Vector2 start, Vector2 end, Vector2 size, out Vector2 freepos, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>(exclude);
		Vector2 diffVector = end - start, directionVector = diffVector.Normalized();

		int accuracy = (int)(diffVector.Length * 6);
		float step = diffVector.Length / accuracy;
		Vector2 checkpos = start;

		for (int i = 0; i < accuracy; i++)
		{
			Vector2 buffer = checkpos;
			buffer += directionVector * step;

			if (GetCollision(buffer, size))
			{
				freepos = checkpos;
				return null;
			}
			foreach (Player p in playerList)
			{
				if (p == null || excludeList.Contains(p)) continue;

				if (p.CollidesWith(checkpos, size))
				{
					freepos = checkpos;
					return p;
				}
			}

			checkpos = buffer;
		}

		freepos = end;
		return null;
	}

	public bool RayTraceCollision(Vector2 start, Vector2 end, Vector2 size, out Vector2 freepos)
	{
		Vector2 diffVector = end - start, directionVector = diffVector.Normalized();

		int accuracy = (int)(diffVector.Length * 6);
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

	public List<Player> GetPlayerRadius(Vector2 pos, float radius, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>(exclude);
		List<Player> returnList = new List<Player>(10);

		foreach (Player p in playerList) if (p != null && !excludeList.Contains(p) && p.CollidesWith(pos, radius)) returnList.Add(p);

		return returnList;
	}

	public List<Player> RayTracePlayer(Vector2 start, Vector2 end, Vector2 size, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>();
		excludeList.AddRange(exclude);
		List<Player> returnList = new List<Player>();

		Vector2 diffVector = end - start, directionVector = diffVector.Normalized();

		int accuracy = (int)(diffVector.Length * 10);
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

	public Player GetPlayerAtPos(Vector2 pos, Vector2 size, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>();
		excludeList.AddRange(exclude);

		foreach (Player p in playerList) if (!excludeList.Contains(p) && p != null && p.CollidesWith(pos, size)) return p;
		return null;
	}

	public void SendPlayerWin(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerWin);
		msg.WriteByte(p.id);

		SendToAllPlayers(msg);
	}

	public void SendTeamWin(Team t)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.TeamWin);
		msg.WriteByte(t.id);

		SendToAllPlayers(msg);
	}

	public void SendToAllPlayers(MessageBuffer msg, params Player[] exceptions)
	{
		List<Player> exceptionList = new List<Player>();
		exceptionList.AddRange(exceptions);

		foreach (Player p in playerList) if (p != null && !exceptionList.Contains(p)) p.client.Send(msg);
	}

	public virtual Vector2 GetFreeSpawnPosition(Player p)
	{
		Vector2 pos;

		do
		{
			double x = rng.NextDouble(), y = rng.NextDouble();
			pos = new Vector2((float)x * scene.Width, (float)y * scene.Height);
		} while (GetCollision(pos, p.size));

		return pos;
	}

	public virtual void MessageHandle(Client c, MessageBuffer msg)
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

					case Protocol.PlayerInputPure:
						p.ReceiveInput(msg.ReadByte());
						break;

					case Protocol.PlayerJump:
						p.ReceiveJump(msg.ReadVector2());
						break;

					case Protocol.PlayerPosition:
						p.ReceivePosition(msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol.PlayerLand:
						p.ReceiveLand(msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol.PlayerDodge:
						p.ReceiveDodge(msg.ReadVector2(), (Direction)msg.ReadByte());
						break;

					case Protocol.PlayerDash:
						p.ReceiveDash(msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol.PlayerShoot:
						p.ReceiveShoot(msg.ReadVector2(), msg.ReadVector2());
						break;
				}
			}

			msg.Reset();
		}
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Yellow, "Packet corrupt!");
			Log.Write(ConsoleColor.Red, e.Message);
			Log.Write(ConsoleColor.DarkRed, e.StackTrace);
		}
	}
}