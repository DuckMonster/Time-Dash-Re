using System;
using System.Diagnostics;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using TKTools;

using EZUDP;
using EZUDP.Client;

public class Game
{
	public const int portTCP = 1765, portUDP = 1780;
	public static string hostIP;

	public static float delta;
	public static EzClient client;

	Program program;

	Map map;
	Stopwatch tickWatch, frameWatch;

	public Game(Program p)
	{
		program = p;

		Log.Init();

		client = new EzClient();

		client.OnConnect += OnConnect;
		client.OnDisconnect += OnDisconnect;
		client.OnMessage += OnMessage;

		client.Connect(hostIP, portTCP, portUDP);
	}

	public void Dispose()
	{
		client.Disconnect();
		client = null;
	}

	public void UpdateProjection(Matrix4 proj)
	{
		Map.defaultProgram["projection"].SetValue(proj);
	}

	public virtual void Logic()
	{
		CalculateDelta();
		Log.Logic();
		Log.Debug("Calculations: {0}\nDraw Calls: {1}", Mesh.CALCULATIONS, Mesh.DRAW_CALLS);

		client.Update();
		if (map != null) map.Logic();
	}

	public void CalculateDelta()
	{
		if (tickWatch == null) tickWatch = Stopwatch.StartNew();

		tickWatch.Stop();
		delta = tickWatch.ElapsedTicks / (float)Stopwatch.Frequency;
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

		if (map != null) map.Draw();
	}

	//ONLINE
	public void OnConnect()
	{
		Log.Write("Connected to server!");
	}
	public void OnDisconnect()
	{
		//Log.Write("Disconnected from server!");
		//program.Exit();
	}
	public void OnMessage(MessageBuffer msg)
	{
		switch ((Protocol)msg.ReadShort())
		{
			case Protocol.EnterMap:
				map = new Map(msg.ReadByte());
				client.OnMessage += map.MessageHandle;
				break;
		}

		msg.Reset();
	}
}