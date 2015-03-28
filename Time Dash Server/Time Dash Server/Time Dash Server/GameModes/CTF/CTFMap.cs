using EZUDP;
using EZUDP.Server;
using OpenTK;
using System.Collections.Generic;
using System.Drawing;
public class CTFMap : Map
{
	SpawnPoint[] spawnPoints = new SpawnPoint[2];
	public CTFFlag[] flags = new CTFFlag[2];

	int[] score = new int[2];

	Timer roundTimer = new Timer(60 * 5, false);

	public CTFMap(string filename, Player[] players)
		: base(filename, GameMode.CaptureTheFlag, players)
	{
	}

	public override Player CreatePlayer(int id, string name, Client c)
	{
		return new CTFPlayer(id, name, c, Vector2.Zero, this);
	}

	public override Player PlayerJoin(Client c, string name)
	{
		Player p = base.PlayerJoin(c, name);
		PlayerJoinTeam(p, p.id % 2);

		p.Position = GetFreeSpawnPosition(p);
		p.SendPositionToPlayerForce(playerList);

		SendRoundTimerToPlayer(p);
		SendScoreboardToPlayer(p);

		flags[0].SendExistenceToPlayer(p);
		flags[1].SendExistenceToPlayer(p);

		return p;
	}

	public override void PlayerLeave(Client c)
	{
		CTFPlayer player = (CTFPlayer)GetPlayer(c);
		if (player.HoldingFlag)
			player.DropFlag();

		base.PlayerLeave(c);
	}

	public void FlagCaptured(CTFFlag f)
	{
		score[(f.ownerID + 1) % 2]++;
	}

	public override void SceneZone(int typeID, TKTools.Polygon p)
	{
		base.SceneZone(typeID, p);

		RectangleF rect = p.Bounds;

		if (typeID == 4)
			spawnPoints[1] = new SpawnPoint(p.Center, new Vector2(rect.Width, rect.Height), this);
		if (typeID == 3)
			spawnPoints[0] = new SpawnPoint(p.Center, new Vector2(rect.Width, rect.Height), this);

		if (typeID == 2)
			flags[0] = new CTFFlag(0, p.Center, this);
		if (typeID == 1)
			flags[1] = new CTFFlag(1, p.Center, this);
	}

	public void SendScoreboardToPlayer(params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CTF.TeamScore);

		msg.WriteByte(score[0]);
		msg.WriteByte(score[1]);

		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}

	public override void Logic()
	{
		base.Logic();
		foreach (CTFFlag flag in flags) flag.Logic();

		int sec = roundTimer.SecondsLeft;
		roundTimer.Logic();
		if (sec != roundTimer.SecondsLeft) SendRoundTimerToPlayer(playerList);

		if (roundTimer.IsDone && winPlayer == null)
		{
			if (score[0] > score[1])
				TeamWin(teamList[0]);
			if (score[1] > score[0])
				TeamWin(teamList[1]);
		}
	}

	public void SendRoundTimerToPlayer(params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CTF.RoundTimer);

		msg.WriteFloat((1f - roundTimer.PercentageDone) * roundTimer.TimerLength);

		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}

	public override Vector2 GetFreeSpawnPosition(Player p)
	{
		if (p.team == null) return Vector2.Zero;

		Vector2 pos;

		do
		{
			pos = spawnPoints[p.team.id].GetSpawnPosition();
		} while (GetCollision(pos, p.Size));

		return pos;
	}
}