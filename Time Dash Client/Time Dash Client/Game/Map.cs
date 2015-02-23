using System;

using OpenTK;
using TKTools;
using EZUDP;
using System.Collections.Generic;
using MapScene;

public class Map : IDisposable
{
	public Random rng = new Random();

	public string filename;
	public GameMode mode;

	public int myID;
	public Camera camera;
	public Scene scene;

	public Team[] teamList = new Team[10];
	public Team GetTeam(int id)
	{
		if (id < 0 || id >= teamList.Length) return null;
		return teamList[id];
	}

	public Player[] playerList = new Player[10];
	List<Effect> effectList = new List<Effect>(), effectBufferList = new List<Effect>();

	public TextDrawer hudDrawer = new TextDrawer(2000, 2000);
	Mesh hudMesh;

	protected Player winPlayer = null;
	
	public void AddEffect(Effect e)
	{
		effectList.Add(e);
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

	public int NumberOfPlayers
	{
		get
		{
			int n = 0;
			foreach (Player p in playerList) if (p != null) n++;
			return n;
		}
	}

	public virtual void PlayerJoin(int id, string name)
	{
		playerList[id] = new Player(id, name, new Vector2(4, 10), this);
	}

	public void PlayerJoinTeam(int id, int teamID)
	{
		if (playerList[id] == null) return;
		PlayerJoinTeam(playerList[id], teamID);
	}
	public void PlayerJoinTeam(Player p, int teamID)
	{
		if (teamList[teamID] == null) teamList[teamID] = new Team(teamID);
		if (teamList[teamID].IsMember(p)) return;

		teamList[teamID].AddMember(p);
	}

	public void PlayerLeave(int id)
	{
		if (playerList[id].team != null) playerList[id].team.RemoveMember(playerList[id]);

		playerList[id].Dispose();
		playerList[id] = null;
	}

	public void PlayerWin(Player p)
	{
		winPlayer = p;
		hudDrawer.Clear();

		Vector2 nameSize = hudDrawer.MeasureString(p.playerName, 0.4f);
		Vector2 winSize = hudDrawer.MeasureString("WINS", 0.2f);

		hudDrawer.Write(p.playerName, 0.5f, 0.5f, 0.4f);
		hudDrawer.Write("WINS", 0.5f, 0.5f + nameSize.Y / 2 + winSize.Y / 2 + 0.016f, 0.2f);

		hudDrawer.UpdateTexture();
	}

	public void TeamWin(Team t)
	{
		winPlayer = t.memberList[0];

		string teamName = t.id == 0 ? "Blue team" : "Orange team";

		Vector2 nameSize = hudDrawer.MeasureString(teamName, 0.4f);
		Vector2 winSize = hudDrawer.MeasureString("WINS", 0.2f);

		hudDrawer.Write(teamName, 0.5f, 0.5f, 0.4f);
		hudDrawer.Write("WINS", 0.5f, 0.5f + nameSize.Y / 2 + winSize.Y / 2 + 0.016f, 0.2f);

		hudDrawer.UpdateTexture();
	}

	public Map(int id, string filename, GameMode mode)
	{
		this.filename = filename;
		this.mode = mode;

		myID = id;

		scene = new Scene(filename, this);
		camera = new Camera(this);

		hudMesh = new Mesh(hudDrawer);
		hudDrawer.UpdateTexture();

		MouseInput.SetCamera(camera);
	}

	public virtual void Dispose()
	{
		foreach (Player p in playerList)
			if (p != null) p.Dispose();

		foreach (Effect e in effectList)
			if (e != null) e.Dispose();

		hudMesh.Dispose();
		hudDrawer.Dispose();
	}

	public virtual void SceneZone(int typeID, Polygon pos)
	{
	}

	public bool GetCollision(Entity e) { return GetCollision(e.position, e.size); }
	public bool GetCollision(Entity e, Vector2 offset) { return GetCollision(e.position + offset, e.size); }
	public bool GetCollision(Vector2 pos, Vector2 size)
	{
		return scene.GetCollision(pos, size);
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

	public List<Player> GetPlayerRadius(Vector2 pos, float radius, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>(exclude);
		List<Player> returnList = new List<Player>(10);

		foreach (Player p in playerList) if (p != null && !excludeList.Contains(p) && p.CollidesWith(pos, radius)) returnList.Add(p);

		return returnList;
	}

	public List<Player> RayTracePlayer(Vector2 start, Vector2 end, Vector2 size, params Player[] exclude)
	{
		List<Player> excludeList = new List<Player>(exclude);
		List<Player> returnList = new List<Player>(10);

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

	public virtual void Logic()
	{
		camera.Logic();
		scene.Logic();
		 
		if (LocalPlayer != null) LocalPlayer.LocalInput();
		foreach (Player p in playerList) if (p != null) p.Logic();

		if (!effectList.Equals(effectBufferList))
		{
			effectBufferList.Clear();
			effectBufferList.AddRange(effectList.ToArray());
		}

		foreach (Effect e in effectBufferList) e.Logic();

		if (!effectList.Equals(effectBufferList))
		{
			effectBufferList.Clear();
			effectBufferList.AddRange(effectList.ToArray());
		}

		if (winPlayer != null)
			Log.Debug(winPlayer.playerID + " WINS!");
	}

	public virtual void Draw()
	{
		Game.defaultShader["view"].SetValue(camera.ViewMatrix);
		Tileset.tileProgram["view"].SetValue(camera.ViewMatrix);

		scene.Draw();

		foreach (Player p in playerList) if (p != null) p.Draw();
		foreach (Effect e in effectList) e.Draw();
		if (LocalPlayer != null) LocalPlayer.DrawHUD();

		Game.hudShader["view"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY));

		if (winPlayer != null)
		{
			hudMesh.Color = Player.colorList[winPlayer.playerID];

			hudMesh.Reset();
			hudMesh.Translate(camera.position.Xy);
			hudMesh.Scale(20f);

			hudMesh.Draw();
		}
	}

	//ONLINE
	public virtual void MessageHandle(MessageBuffer msg)
	{
		try
		{
			switch ((Protocol)msg.ReadShort())
			{
				case Protocol.PlayerJoin:
					PlayerJoin(msg.ReadByte(), msg.ReadString());
					break;

				case Protocol.PlayerLeave:
					PlayerLeave(msg.ReadByte());
					break;

				case Protocol.PlayerInput:
					playerList[msg.ReadByte()].ReceiveInput(msg.ReadVector2(), msg.ReadVector2(), msg.ReadByte());
					break;

				case Protocol.PlayerInputPure:
					playerList[msg.ReadByte()].ReceiveInput(msg.ReadByte());
					break;

				case Protocol.PlayerJump:
					playerList[msg.ReadByte()].ReceiveJump(msg.ReadVector2());
					break;

				case Protocol.PlayerPosition:
					playerList[msg.ReadByte()].ReceivePosition(msg.ReadVector2(), msg.ReadVector2());
					break;

				case Protocol.PlayerHit:
					playerList[msg.ReadByte()].ReceiveHit(msg.ReadFloat(), playerList[msg.ReadByte()], msg.ReadFloat(), msg);
					break;

				case Protocol.PlayerDie:
					playerList[msg.ReadByte()].Die(msg.ReadVector2());
					break;

				case Protocol.PlayerRespawn:
					playerList[msg.ReadByte()].Respawn(msg.ReadVector2());
					break;

				case Protocol.PlayerDodge:
					playerList[msg.ReadByte()].ReceiveDodge(msg.ReadVector2(), (Direction)msg.ReadByte());
					break;

				case Protocol.PlayerDash:
					playerList[msg.ReadByte()].ReceiveDash(msg.ReadVector2(), msg.ReadVector2());
					break;

				case Protocol.PlayerDodgeCollision:
					playerList[msg.ReadByte()].ReceiveDodgeCollision(msg.ReadByte(), msg.ReadVector2(), msg.ReadVector2());
					break;

				case Protocol.PlayerDashCollision:
					playerList[msg.ReadByte()].ReceiveDashCollision(msg.ReadByte(), msg.ReadVector2(), msg.ReadVector2());
					break;

				case Protocol.PlayerShoot:
					playerList[msg.ReadByte()].ReceiveShoot(msg.ReadVector2(), msg.ReadVector2());
					break;
				
				case Protocol.PlayerEquipWeapon:
					playerList[msg.ReadByte()].EquipWeapon(msg.ReadByte());
					break;
					
				case Protocol.PlayerReload:
					playerList[msg.ReadByte()].ReceiveReload();
					break;

				case Protocol.PlayerWin:
					PlayerWin(playerList[msg.ReadByte()]);
					break;

				case Protocol.TeamWin:
					TeamWin(teamList[msg.ReadByte()]);
					break;

				case Protocol.PlayerJoinTeam:
					PlayerJoinTeam(msg.ReadByte(), msg.ReadByte());
					break;
	
				case Protocol.ServerPosition:
					playerList[msg.ReadByte()].ReceiveServerPosition(msg.ReadVector2());
					break;
			}
		}
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Yellow, "Packet corrupt!");
			Log.Write(ConsoleColor.Red, e.Message);
			Log.Write(ConsoleColor.DarkRed, e.StackTrace);
		}

		if (msg != null) msg.Reset();
	}
}