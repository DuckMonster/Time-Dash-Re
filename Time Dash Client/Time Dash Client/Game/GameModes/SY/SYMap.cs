using OpenTK;
using System;
using System.Drawing;
using System.Collections.Generic;
using TKTools;
using MapScene;

public class SYMap : Map
{
	SYScrap[] scrapList = new SYScrap[500];
	SYStash[] stashList = new SYStash[10];
	SYCreep[] creepList = new SYCreep[50];
	SYTowerWall[] wallList = new SYTowerWall[8];

	public SYTower[] towerList = new SYTower[8];

	SYBase[] baseList = new SYBase[2];

	Menu shopMenu;

	int stashIndex = 0;

	bool showShop = false;

	FrameBuffer frameBuffer;
	Mesh frameBufferMesh;

	public override IEnumerable<Actor> Actors
	{
		get
		{
			foreach (Actor a in base.Actors)
				yield return a;

			foreach (SYCreep c in creepList)
				if (c != null) yield return c;

			foreach (SYTower t in towerList)
				if (t != null) yield return t;

			foreach (SYBase b in baseList)
				if (b != null) yield return b;

			foreach (SYTowerWall w in wallList)
				if (w != null) yield return w;
		}
	}

	public override bool PauseInput
	{
		get { return showShop || !baseList[0].IsAlive || !baseList[1].IsAlive; }
	}

	public SYMap(int id, string filename)
		: base(id, filename, GameMode.ScrapYard)
	{
		shopMenu = new ShopMenu.ShopMenu(this);

		frameBuffer = new FrameBuffer(2056, 2056);
		frameBufferMesh = Mesh.OrthoBox;
		frameBufferMesh.UV = new Vector2[] {
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 0f)
			};

		frameBufferMesh.Texture = frameBuffer.Texture;
		frameBufferMesh.UsingBlur = true;
		frameBufferMesh.BlurIntensity = 1f;
		frameBufferMesh.BlurRadius = 6;

		teamList[0] = new Team(0);
		teamList[1] = new Team(1);
	}

	public override bool GetCollision(Entity e, Vector2 offset)
	{
		bool c = base.GetCollision(e, offset);
		if (!c)
		{
			foreach (SYTowerWall w in wallList)
			{
				if (w == null) continue;
				if (e is Player && (e as Player).Team == w.Team) continue;

				if (w.CollidesWith(e.Position, e.Size))
					return true;
			}
		}

		return c;
	}

	public override bool GetCollision(Vector2 pos, Vector2 size, Entity e = null)
	{
		bool c = base.GetCollision(pos, size);
		if (!c)
		{
			foreach (SYTowerWall w in wallList)
			{
				if (w == null) continue;
				if (e != null && (e is Actor && (e as Actor).Team == w.Team)) continue;

				if (w.CollidesWith(pos, size))
					return true;
			}
		}

		return c;
	}

	public void SpawnCreep(CreepType type, int id, Vector2 position, Vector2 velocity)
	{
		if (creepList[id] != null) RemoveCreep(id);

		switch(type)
		{
			case CreepType.Scroot:
				creepList[id] = new SYScroot(id, position, velocity, this);
				break;
		}
	}

	public void RemoveCreep(SYCreep e) { if (e != null) RemoveCreep(e.id); }
	public void RemoveCreep(int id)
	{
		if (creepList[id] == null) return;

		creepList[id].Dispose();
		creepList[id] = null;
	}

	public void CreateScrap(int id, Vector2 position, Vector2 velocity)
	{
		if (scrapList[id] != null)
			RemoveScrap(id);

		scrapList[id] = new SYScrap(id, position, velocity, this);
	}

	public void RemoveScrap(SYScrap scrap) { RemoveScrap(scrap.id); }
	public void RemoveScrap(int id)
	{
		scrapList[id].Dispose();
		scrapList[id] = null;
	}

	public void TowerDestroyed(SYTower t)
	{
		towerList[t.id] = null;
		t.Dispose();
	}

	public override void PlayerJoin(int id, string name)
	{
		playerList[id] = new SYPlayer(id, name, Vector2.Zero, this);
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void SceneEvent(EnvEvent e, int[] args, Polygon p)
	{
		base.SceneEvent(e, args, p);

		RectangleF rect = p.Bounds;

		if (e.ID == 1)
			baseList[args[0]] = new SYBase(p.Center, args[0], this);

		if (e.ID == 6)
			wallList[args[0]] = new SYTowerWall(p, args[0], this);
	}

	public override void Logic()
	{
		base.Logic();
		foreach (SYScrap s in scrapList) if (s != null) s.Logic();
		foreach (SYStash b in stashList) if (b != null) b.Logic();

		if (KeyboardInput.KeyPressed(OpenTK.Input.Key.B))
		{
			showShop = !showShop;
			if (showShop)
				shopMenu.Open();
			else
				shopMenu.Close();
		}

		if (showShop)
			shopMenu.Logic();

		if (!baseList[0].IsAlive)
		{
			camera.FocusObject = baseList[0];
		} else if (!baseList[1].IsAlive)
		{
			camera.FocusObject = baseList[1];
		}
	}

	public override void Draw()
	{
		UpdateView();
		/*
		if (showShop)
		{
			frameBuffer.Bind();
			GL.Viewport(0, 0, frameBuffer.Width, frameBuffer.Height);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			DrawBackground();
			foreach (SYScrap s in scrapList) if (s != null) s.Draw();
			foreach (SYStash b in stashList) if (b != null) b.Draw();
			foreach (SYCreep c in creepList) if (c != null) c.Draw();
			foreach (SYTower t in towerList) if (t != null) t.Draw();
			DrawMap();

			frameBuffer.Release();
			GL.Viewport(Game.program.ClientRectangle);

			frameBufferMesh.Reset();
			frameBufferMesh.Scale(20f, 20f * Game.windowRatio);

			frameBufferMesh.Draw();

			shopMenu.Draw();
		} else
		{
			DrawBackground();
			foreach (SYScrap s in scrapList) if (s != null) s.Draw();
			foreach (SYStash b in stashList) if (b != null) b.Draw();
			foreach (SYCreep c in creepList) if (c != null) c.Draw();
			foreach (SYTower t in towerList) if (t != null) t.Draw();
			DrawMap();
		}
		*/

		DrawBackground();
		foreach (SYScrap s in scrapList) if (s != null) s.Draw();
		foreach (SYStash b in stashList) if (b != null) b.Draw();
		DrawMap();

		if (showShop)
			shopMenu.Draw();

		(LocalPlayer as SYPlayer).DrawScrap();
	}

	public override void MessageHandle(EZUDP.MessageBuffer msg)
	{
		try
		{
			if ((Protocol)msg.ReadShort() == Protocol.MapArgument)
			{
				switch ((Protocol_SY)msg.ReadShort())
				{
					case Protocol_SY.PlayerScrap:
						(playerList[msg.ReadByte()] as SYPlayer).ReceiveScrap(msg.ReadByte());
						break;

					case Protocol_SY.ScrapGainEffect:
						AddEffect(new EffectGainScrap(msg.ReadVector2(), msg.ReadShort(), this));
						break;

					case Protocol_SY.ScrapExistance:
						CreateScrap(msg.ReadShort(), msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol_SY.ScrapCollect:
						{
							SYScrap scrap = scrapList[msg.ReadShort()];
							SYPlayer player = (SYPlayer)playerList[msg.ReadByte()];

							player.CollectScrap(scrap);

							break;
						}

					case Protocol_SY.ScrapReturn:
						((SYPlayer)playerList[msg.ReadByte()]).ReturnScrap(stashList[msg.ReadByte()]);
						break;

					case Protocol_SY.StashScrapAmount:
						stashList[msg.ReadByte()].SetScrap(msg.ReadShort());
						break;

					case Protocol_SY.CreepExistance:
						SpawnCreep((CreepType)msg.ReadByte(), msg.ReadByte(), msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol_SY.CreepPosition:
						creepList[msg.ReadByte()].ReceivePosition(msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol_SY.CreepHit:
						creepList[msg.ReadByte()].ReceiveHit(msg.ReadFloat(), msg.ReadFloat(), msg);
						break;

					case Protocol_SY.CreepDie:
						creepList[msg.ReadByte()].Die(msg.ReadVector2());
						break;

					case Protocol_SY.CreepCustom:
						creepList[msg.ReadByte()].ReceiveCustom(msg);
						break;

					case Protocol_SY.TowerExistance:
						{
							int id = msg.ReadByte();
							//SYTowerPoint stash = stashList[msg.ReadByte()] as SYTowerPoint;

							towerList[id] = new SYTower(id, msg.ReadByte(), msg.ReadVector2(), this);
							break;
						}

					case Protocol_SY.TowerRotation:
						towerList[msg.ReadByte()].ReceiveRotation(msg.ReadFloat());
						break;

					case Protocol_SY.TowerTarget:
						towerList[msg.ReadByte()].ReceiveTarget(msg.ReadByte());
						break;

					case Protocol_SY.TowerShoot:
						towerList[msg.ReadByte()].ReceiveShoot(msg.ReadVector2(), msg.ReadByte());
						break;

					case Protocol_SY.TowerHit:
						towerList[msg.ReadByte()].ReceiveHit(msg.ReadFloat(), msg.ReadFloat(), msg.ReadByte(), msg.ReadVector2());
						break;

					case Protocol_SY.TowerDie:
						towerList[msg.ReadByte()].ReceiveDie();
						break;

					case Protocol_SY.BaseHit:
						baseList[msg.ReadByte()].ReceiveHit(msg.ReadFloat(), msg.ReadFloat(), msg.ReadByte(), msg.ReadVector2());
						break;
				}
			}
		}
		catch (Exception e)
		{
			Log.Write(ConsoleColor.Yellow, "Packet corrupt!");
			Log.Write(ConsoleColor.Red, e.Message);
			Log.Write(ConsoleColor.DarkRed, e.StackTrace);
		}

		if (msg != null) msg.Reset();

		base.MessageHandle(msg);
	}
}