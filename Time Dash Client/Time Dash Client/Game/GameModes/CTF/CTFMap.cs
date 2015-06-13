using OpenTK;
using System;
using TKTools;
using TKTools.Context;

public class CTFMap : Map
{
	public CTFFlag[] flags = new CTFFlag[2];
	int[] score = new int[2];

	TextDrawer text;
	Mesh mesh;

	TextDrawer roundTimerText;
	Mesh roundTimerMesh;

	TextDrawer scoreText;
	Mesh scoreMesh;

	Timer messageTimer = new Timer(2f, true);
	Timer roundTimer = new Timer(60 * 5, false);

	public CTFMap(int id, string filename)
		: base(id, filename, GameMode.CaptureTheFlag)
	{
		text = new TextDrawer(1000, 1000);
		mesh = new Mesh(text);

		roundTimerText = new TextDrawer(1000, 1000);
		roundTimerMesh = new Mesh(roundTimerText);
		roundTimerMesh.Orthographic = true;

		roundTimerMesh.Translate(0.5f, -0.04f);
		roundTimerMesh.Scale(0.2f);

		scoreText = new TextDrawer(500, 500);

		scoreMesh = new Mesh(scoreText);
		scoreMesh.Orthographic = true;

		scoreMesh.Translate(0.5f, -0.08f);
		scoreMesh.Scale(0.1f);

		UpdateScoreText();
	}

	public override void Dispose()
	{
		roundTimerText.Dispose();
		roundTimerMesh.Dispose();

		scoreText.Dispose();
		scoreMesh.Dispose();

		text.Dispose();
		mesh.Dispose();

		base.Dispose();
	}

	public void FlagStolen(CTFFlag f)
	{
		string teamName = f.ownerID == 0 ? "Blue teams" : "Orange teams";

		mesh.Color = Player.colorList[f.ownerID];

		text.Clear();
		text.Write(teamName, 0.5f, 0.4f, 1f);
		text.Write("flag has been stolen!", 0.5f, 0.55f, 1f);

		messageTimer.Reset();
	}

	public void FlagCaptured(CTFFlag f)
	{
		string teamName = f.ownerID == 0 ? "Blue teams" : "Orange teams";

		Color c = Player.colorList[(f.ownerID + 1) % 2];

		mesh.Color = c;

		text.Clear();
		text.Write(teamName, 0.5f, 0.4f, 1f);
		text.Write("flag has been captured!", 0.5f, 0.55f, 1f);

		messageTimer.Reset();

		score[(f.ownerID + 1) % 2]++;

		UpdateScoreText();
	}

	public void UpdateScoreText()
	{
		scoreText.Clear();
		scoreText.Color = new Color(0.5f, 0.5f, 0.5f);
		scoreText.Write("|", 0.5f, 0.5f, 0.4f);

		scoreText.Color = Player.colorList[0];
		scoreText.Write(score[0].ToString(), 0.3f, 0.5f, 0.4f);
		scoreText.Color = Player.colorList[1];
		scoreText.Write(score[1].ToString(), 0.7f, 0.5f, 0.4f);
	}

	public void UpdateTimeText()
	{
		roundTimerText.Clear();
		roundTimerText.Write(string.Format("{0:00}:{1:00}", roundTimer.MinutesLeft, roundTimer.SecondsLeft % 60), 0.5f, 0.5f, 1f);
	}

	public override void PlayerJoin(int id, string name)
	{
		playerList[id] = new CTFPlayer(id, name, Vector2.Zero, this);
	}

	/*
	public override void SceneEvent(int typeID, TKTools.Polygon p)
	{
		base.SceneEvent(typeID, p);

		if (typeID == 2)
			flags[0] = new CTFFlag(0, p.Center, this);
		if (typeID == 1)
			flags[1] = new CTFFlag(1, p.Center, this);
	}
	*/

	public override void Logic()
	{
		base.Logic();
		foreach (CTFFlag flag in flags)
			flag.Logic();

		messageTimer.Logic();

		int sec = roundTimer.SecondsLeft;
		roundTimer.Logic();
		if (roundTimer.SecondsLeft != sec)
			UpdateTimeText();

		if (roundTimer.IsDone && winPlayer == null)
		{
			if (score[0] > score[1])
				TeamWin(teamList[0]);
			if (score[1] > score[0])
				TeamWin(teamList[1]);
		}
	}

	public override void Draw()
	{
		base.Draw();
		foreach (CTFFlag flag in flags) 
			flag.Draw();

		roundTimerMesh.Color = LocalPlayer.Color;
		roundTimerMesh.Draw();

		scoreMesh.Draw();

		if (!messageTimer.IsDone)
		{
			Color c = mesh.Color;
			c.A = 1f - messageTimer.PercentageDone;

			mesh.Color = c;

			mesh.Reset();
			mesh.Translate(camera.position.Xy);
			mesh.Scale(15f);
			mesh.Draw();
		}
	}

	public override void MessageHandle(EZUDP.MessageBuffer msg)
	{
		try
		{
			if ((Protocol)msg.ReadShort() == Protocol.MapArgument)
			{
				switch ((Protocol_CTF)msg.ReadShort())
				{
					case Protocol_CTF.FlagGrabbed:
						{
							CTFPlayer player = (CTFPlayer)playerList[msg.ReadByte()];
							player.GrabFlag(flags[(player.Team.id + 1) % 2]);
							break;
						}

					case Protocol_CTF.FlagDropped:
						flags[msg.ReadByte()].Drop(msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol_CTF.FlagReturned:
						flags[msg.ReadByte()].Return();
						break;

					case Protocol_CTF.FlagCaptured:
						FlagCaptured(flags[msg.ReadByte()]);
						break;

					case Protocol_CTF.RoundTimer:
						roundTimer = new Timer(msg.ReadFloat(), false);
						UpdateTimeText();
						break;

					case Protocol_CTF.FlagPosition:
						flags[msg.ReadByte()].ReceivePosition(msg.ReadVector2(), msg.ReadVector2());
						break;

					case Protocol_CTF.TeamScore:
						score[0] = msg.ReadByte();
						score[1] = msg.ReadByte();

						UpdateScoreText();
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