using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using GRFX = OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;

public class Program : GameWindow
{
	public static Program client;
	Game game;

	static void Main(string[] args)
	{
		#region Name

		Console.Write("Name: ");
		Game.myName = Console.ReadLine();
		Console.Clear();

		#endregion
		#region Server ip
		List<string> ipList = new List<string>();

		if (!File.Exists(".serverlist")) File.Create(".serverlist");

		using (StreamReader str = new StreamReader(".serverlist"))
		{
			while (!str.EndOfStream)
			{
				string ip = str.ReadLine();

				if (ip.Split('.').Length == 4)
					ipList.Add(ip);
			}
		}

		bool valid = false;

		if (args.Length > 0)
		{
			Game.hostIP = args[0];
			valid = true;
		}

		while (!valid)
		{
			try
			{
				Console.WriteLine("Connect to:");
				for (int i = 0; i < ipList.Count; i++)
					Console.WriteLine("({0}) {1}", i, ipList[i]);
				Console.WriteLine("\n(N) Add...");

				char input = Console.ReadKey().KeyChar;

				if (input == 'n' || input == 'N')
				{
					Console.Clear();
					Console.Write("New IP: ");
					string ip = Console.ReadLine();

					using (StreamWriter str = new StreamWriter(".serverlist", true))
					{
						str.WriteLine(ip);
					}

					ipList.Add(ip);

					Console.Clear();
					continue;
				}

				int n = int.Parse(input.ToString());

				Game.hostIP = ipList[n];
				valid = true;
			}
#pragma warning disable
			catch (Exception e)
			{
				Console.Clear();
				Console.WriteLine("Invalid input.");
			}
#pragma warning enable
		}
		#endregion

		using (Program p = new Program(1025, 768, new GRFX.GraphicsMode(new GRFX.ColorFormat(32), 24, 8, 3)))
		{
			p.Run(200.0);
		}
	}

	public Program(int w, int h, GRFX.GraphicsMode mode)
		: base(w, h, mode)
	{
		KeyDown += KeyHandle;
		Closed += OnClose;

		client = this;
	}

	public void OnClose(object sender, EventArgs e)
	{
		game.Dispose();
		Log.ShutDown();
	}

	public void KeyHandle(object sender, KeyboardKeyEventArgs a)
	{
		if (a.Key == Key.Escape) Exit();
		if (a.Key == Key.F4)
		{
			if (WindowState == OpenTK.WindowState.Maximized) this.WindowState = OpenTK.WindowState.Normal;
			else this.WindowState = OpenTK.WindowState.Maximized;
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		Title = "Time Dash";

		GL.ClearColor(0.4f, 0.4f, 0.4f, 1f);
		GL.Enable(EnableCap.Blend);
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

		//GL.Enable(EnableCap.DepthTest);

		WindowBorder = OpenTK.WindowBorder.Resizable;
		VSync = VSyncMode.Off;

		game = new Game(this);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);

		GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
		game.UpdateProjection(ClientSize.Width, ClientSize.Height);
	}

	protected override void OnUpdateFrame(FrameEventArgs e)
	{
		base.OnUpdateFrame(e);

		if (Focused) KeyboardInput.Update();
		game.Logic();
	}

	protected override void OnRenderFrame(FrameEventArgs e)
	{
		base.OnRenderFrame(e);

		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		game.Draw();

		SwapBuffers();
	}
}