using System;

using OpenTK;
using TKTools;
using EZUDP;
using System.Collections.Generic;
using MapScene;
using TKTools.Context;
using TKTools.Context.Input;
using MathUtils.Collections;

public class Map : IDisposable
{
	public static Camera GameCamera
	{
		get { return currentMap.gameCamera; }
	}
	public static Camera UICamera
	{
		get { return currentMap.uiCamera; }
	}

	static Map currentMap;

	public Random rng = new Random();

	public string filename;
	public GameMode mode;

	public int myID;
	public Camera gameCamera, uiCamera;
	public CameraControl cameraController;
	public Scene scene;

	public Team[] teamList = new Team[10];
	public Team GetTeam(int id)
	{
		if (id < 0 || id >= teamList.Length) return null;
		return teamList[id];
	}

	public Player[] playerList = new Player[10];
	DataTree<Effect> effectList = new DataTree<Effect>();

	public Projectile[] projectileList = new Projectile[100];
	int projectileIndex = 0;

	protected KeyboardWatch keyboard;

	public int NextProjectileIndex
	{
		get
		{
			int id = projectileIndex;
			projectileIndex = (projectileIndex + 1) % projectileList.Length;

			return projectileIndex;
		}
	}

	protected Player winPlayer = null;
	
	public void AddEffect(Effect e, float w = 0f)
	{
		effectList.AddItem(e, w);
	}
	public void RemoveEffect(Effect e)
	{
		effectList.RemoveItem(e);
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

	public virtual bool PauseInput
	{
		get
		{
			return false;
		}
	}

	public virtual IEnumerable<Actor> Actors
	{
		get
		{
			foreach (Player p in playerList)
				yield return p;
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
		if (playerList[id].Team != null) playerList[id].Team.RemoveMember(playerList[id]);

		playerList[id].Dispose();
		playerList[id] = null;
	}

	public void PlayerWin(Player p)
	{
		winPlayer = p;
		//DRAW WIN PLAYER HERE PLOX LOLOLOLLLOLOLOLLOLOLOLOLOLOLOLOLOLOLOLOLOLOL
	}

	public void TeamWin(Team t)
	{
		winPlayer = t.memberList[0];

		string teamName = t.id == 0 ? "Blue team" : "Orange team";

		//DRAW WIN TEAM HERE LOLOLOLLOLOLOLLOLLOLOLOLLOLOLOLLOLOLOLOLOLO
	}

	public void AddProjectile(Projectile p)
	{
		int id = p.id;

		if (projectileList[id] != null)
			RemoveProjectile(id);

		projectileList[id] = p;
		projectileIndex = id;
	}

	public void RemoveProjectile(Projectile p) { RemoveProjectile(p.id); }
	public void RemoveProjectile(int id)
	{
		if (projectileList[id] == null) return;

		projectileList[id].Dispose();
		projectileList[id] = null;
	}

	public Map(int id, string filename, GameMode mode)
	{
		currentMap = this;

		this.filename = filename;
		this.mode = mode;

		myID = id;

		scene = new Scene(filename, this);

		gameCamera = new Camera();
		gameCamera.Use();

		uiCamera = new Camera();
		uiCamera.Orthogonal = true;

		cameraController = new CameraControl(this, gameCamera);

		keyboard = new KeyboardWatch();
	}

	public virtual void Dispose()
	{
		foreach (Player p in playerList)
			if (p != null) p.Dispose();

		foreach (Effect e in effectList)
			if (e != null) e.Dispose();
	}

	public virtual void SceneEvent(EnvEvent e, int[] args, Polygon pos)
	{
	}

	public bool GetCollision(Entity e) { return GetCollision(e.Position, e.Size, e); }
	public virtual bool GetCollision(Entity e, Vector2 offset) { return GetCollision(e.Position + offset, e.Size, e); }
	public virtual bool GetCollision(Vector2 pos, Vector2 size, Entity e = null)
	{
		return scene.GetCollisionFast(pos, size);
	}

	public T GetActorAtPos<T>(Vector2 pos, Vector2 size, params Actor[] exclude) where T : Actor
	{
		List<Actor> excludeList = new List<Actor>(exclude);

		foreach (Actor a in Actors) if (a != null && (a is T) && a.IsAlive && !excludeList.Contains(a) && a.CollidesWith(pos, size)) return (a as T);
		return null;
	}

	public bool RayTraceCollision(Vector2 start, Vector2 end, Vector2 size, out Vector2 freepos, Entity e = null)
	{
		Vector2 diffVector = end - start, directionVector = diffVector.Normalized();

		int accuracy = 1 + (int)(diffVector.Length * 6);
		float step = diffVector.Length / accuracy;
		Vector2 checkpos = start;

		for (int i = 0; i < accuracy; i++)
		{
			Vector2 buffer = checkpos;
			buffer += directionVector * step;

			if (GetCollision(buffer, size, e))
			{
				freepos = checkpos;
				return true;
			}

			checkpos = buffer;
		}

		freepos = end;
		return false;
	}

	public List<T> GetActorRadius<T>(Vector2 pos, float radius, params Actor[] exclude) where T : Actor
	{
		List<T> returnList = new List<T>(10);
		List<Actor> excludeList = new List<Actor>(exclude);

		foreach (Actor a in Actors)
		{
			if (a == null || !(a is T)) continue;

			if (a.CollidesWith(pos, radius)) 
				returnList.Add(a as T);
		}

		return returnList;
	}

	public List<T> RayTraceActor<T>(Vector2 start, Vector2 end, Vector2 size, params T[] exclude) where T : Actor
	{
		List<T> returnList = new List<T>(10);
		List<Actor> excludeList = new List<Actor>(exclude);

		Vector2 diffVector = end - start, directionVector = diffVector.Normalized();

		int accuracy = 1 + (int)(diffVector.Length * 6);
		float step = diffVector.Length / accuracy;
		Vector2 checkpos = start;

		for (int i = 0; i < accuracy; i++)
		{
			checkpos += directionVector * step;
			T a =  GetActorAtPos<T>(checkpos, size, excludeList.ToArray());
			
			if (a != null && !returnList.Contains(a))
			{
				returnList.Add(a);
				excludeList.Add(a);
			}
		}

		return returnList;
	}

	public virtual void Logic()
	{
		cameraController.Logic();
		scene.Logic();
		 
		if (LocalPlayer != null) LocalPlayer.LocalInput();
		foreach (Actor a in Actors) if (a != null) a.Logic();

		foreach (Effect e in effectList) e.Logic();
		foreach (Projectile p in projectileList)
			if (p != null) p.Logic();

		if (winPlayer != null)
			Log.Debug(winPlayer.playerID + " WINS!");
	}

	public void DrawBackground()
	{
		scene.Draw();
	}

	public void DrawForeground()
	{
		scene.Draw();
	}

	public void DrawMap()
	{
		foreach (Actor a in Actors) if (a != null) a.Draw();
		foreach (Effect e in effectList) e.Draw();
		foreach (Projectile p in projectileList) if (p != null) p.Draw();

		if (winPlayer != null)
		{
		}
	}

	public virtual void Draw()
	{
		DrawBackground();
		DrawMap();

		UICamera.Use();

		DrawHUD();

		GameCamera.Use();
	}

	public virtual void DrawHUD()
	{
		if (LocalPlayer != null) LocalPlayer.DrawHUD();
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
					playerList[msg.ReadByte()].ReceiveHit(msg.ReadFloat(), msg.ReadFloat(), msg);
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
					{
						Player p = playerList[msg.ReadByte()];

						if (p.Weapon.FireType == WeaponStats.FireType.Charge)
							p.ReceiveShoot(msg.ReadVector2(), msg.ReadVector2(), msg.ReadByte(), msg.ReadFloat());
						else
							p.ReceiveShoot(msg.ReadVector2(), msg.ReadVector2(), msg.ReadByte());

						break;
					}

				case Protocol.PlayerBuyWeapon:
					playerList[msg.ReadByte()].ReceiveBuyWeapon((WeaponList)msg.ReadByte());
					break;

				case Protocol.PlayerInventory:
					playerList[msg.ReadByte()].ReceiveInventory(msg.ReadByte(), msg.ReadByte());
					break;

				case Protocol.PlayerEquipWeapon:
					playerList[msg.ReadByte()].ReceiveEquipWeapon(msg.ReadByte());
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