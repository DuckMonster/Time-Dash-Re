using System;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

using EZUDP;
using EZUDP.Client;
using System.Collections.Generic;
using System.IO;

public class Game
{
	public const int portTCP = Port.TCP, portUDP = Port.UDP;
	public static string hostIP;

	public static ShaderProgram defaultShader;
	public static ShaderProgram hudShader;

	public static string myName;

	public static float delta;
	public static EzClient client;

	public static float windowRatio;

	public static Program program;
	Map map;
	Stopwatch tickWatch, frameWatch;

	public Game(Program p)
	{
		program = p;

		client = new EzClient();

		client.OnConnect += OnConnect;
		client.OnDisconnect += OnDisconnect;
		client.OnMessage += OnMessage;
		client.OnException += OnException;
		client.OnDebug += OnDebug;

		Log.Init();

		defaultShader = new ShaderProgram("Shaders/standardShader.glsl");
		hudShader = new ShaderProgram("Shaders/standardShader.glsl");

		Log.Write(defaultShader.Log);

		ConnectTo(hostIP);
	}

	public void ConnectTo(string ip)
	{
		client.Connect(ip, portTCP, portUDP);
	}

	public void Dispose()
	{
		defaultShader.Dispose();
		hudShader.Dispose();

		client.Disconnect();
		client = null;
	}

	public void UpdateProjection(int width, int height)
	{
		windowRatio = (float)height / width;

		Matrix4 pers = Matrix4.CreatePerspectiveOffCenter(-1, 1, -windowRatio, windowRatio, 1, 200f);
		Matrix4 orth = Matrix4.CreateOrthographicOffCenter(-10f, 10f, -windowRatio * 10f, windowRatio * 10f, 1, 200f);

		defaultShader["projection"].SetValue(pers);
		hudShader["projection"].SetValue(orth);
		hudShader["view"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY));
	
		Tileset.tileProgram["projection"].SetValue(pers);
	}

	public void LoadMap(int id, string filename)
	{
		if (!filename.EndsWith(".tdm")) filename += ".tdm";

		if (!File.Exists("Maps/" + filename))
		{
			Log.Write(ConsoleColor.Red, "Map \"" + filename + "\" doesn't exist!");
			return;
		}

		if (map != null)
		{
			client.OnMessage -= map.MessageHandle;
			map.Dispose();
			map = null;
		}

		string mapname;
		GameMode mode;
		string modeName = "Unknown";

		using (BinaryReader reader = new BinaryReader(new FileStream("Maps/" + filename, FileMode.Open)))
		{
			mapname = reader.ReadString();
			int modei = reader.ReadInt32();
			mode = (GameMode)modei;
		}

		switch (mode)
		{
			case GameMode.KingOfTheHill:
				map = new KothMap(id, filename);
				modeName = "King of the Hill";
				break;

			case GameMode.DeathMatch:
				map = new DMMap(id, filename);
				modeName = "Deathmatch";
				break;

			case GameMode.ControlPoints:
				map = new CPMap(id, filename);
				modeName = "Control Points";
				break;

			case GameMode.CaptureTheFlag:
				map = new CTFMap(id, filename);
				modeName = "Capture The Flag";
				break;

			case GameMode.ScrapYard:
				map = new SYMap(id, filename);
				modeName = "Scrapyard";
				break;
		}

		client.OnMessage += map.MessageHandle;

		Log.Write(ConsoleColor.Yellow, "Loaded \"" + mapname + "\" | " + modeName);
	}

	public virtual void Logic()
	{
		CalculateDelta();
		Log.Logic();
		Log.Debug("Calculations: {0}\nDraw Calls: {1}", Mesh.CALCULATIONS, Mesh.DRAW_CALLS);

		if (client.Connected)
		{
			client.Update();
			if (map != null) map.Logic();
		}
	}

	public void CalculateDelta()
	{
		if (tickWatch == null) tickWatch = Stopwatch.StartNew();

		tickWatch.Stop();
		delta = tickWatch.ElapsedTicks / (float)Stopwatch.Frequency;
		if (delta > 0.2f) delta = 0;
		tickWatch.Restart();

		Log.CalculateTick(delta);
	}

	public void CalculateFrameDelta()
	{
		if (frameWatch == null) frameWatch = Stopwatch.StartNew();

		frameWatch.Stop();
		float d = frameWatch.ElapsedTicks / (float)Stopwatch.Frequency;
		frameWatch.Restart();

		Log.CalculateFrame(d);
	}

	public void Draw()
	{
		CalculateFrameDelta();
		Mesh.DRAW_CALLS = Mesh.CALCULATIONS = 0;

		if (client.Connected)
		{
			if (map != null)
			{
				//map.scene.Draw();
				map.Draw();
			}
		}
	}

	public void SendName()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerName);
		msg.WriteString(myName);

		client.Send(msg);
	}

	//ONLINE
	public void OnConnect()
	{
		Log.Write(ConsoleColor.Green, "Connected to server!");
		SendName();
		client.Ping();
	}
	public void OnDisconnect()
	{
		Log.Write("Disconnected from server!");
		program.Exit();
	}
	public void OnMessage(MessageBuffer msg)
	{
		try
		{
			switch ((Protocol)msg.ReadShort())
			{
				case Protocol.EnterMap:
					LoadMap(msg.ReadByte(), msg.ReadString());
					break;
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
	public void OnException(Exception e)
	{
		Log.Write(e.Message);
	}

	public void OnDebug(string msg)
	{
		Log.Write(msg);
	}
}