using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using GRFX = OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

public class Program : GameWindow
{
	Game game;

	static void Main(string[] args)
	{
		#region Server ip
		string[] serverList = new string[] {
			"127.0.0.1",
			"90.224.59.61"
		};

		bool valid = false;

		while (!valid)
		{
			try
			{
				Console.WriteLine("Connect to:");
				for (int i = 0; i < serverList.Length; i++)
					Console.WriteLine("({0}) {1}", i, serverList[i]);

				int n = int.Parse(Console.ReadKey().KeyChar.ToString());

				Game.hostIP = serverList[n];
				valid = true;
			}
			catch (Exception e)
			{
				Console.Clear();
				Console.WriteLine("Invalid input.");
			}
		}
		#endregion

		using (Program p = new Program(1025, 768, new GRFX.GraphicsMode(new GRFX.ColorFormat(32), 24, 8, 3)))
		{
			p.Run(180.0, 200.0);
		}
	}

	public Program(int w, int h, GRFX.GraphicsMode mode)
		: base(w, h, mode)
	{
		KeyDown += KeyHandle;
		Closed += OnClose;
	}

	public void OnClose(object sender, EventArgs e)
	{
		game.Dispose();
		Log.ShutDown();
	}

	public void KeyHandle(object sender, KeyboardKeyEventArgs a)
	{
		if (a.Key == Key.Escape) Exit();
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		Title = "Time Dash";

		GL.ClearColor(1f, 0f, 0f, 1f);
		GL.Enable(EnableCap.Blend);
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

		Width = 400;
		Height = 300;

		WindowBorder = OpenTK.WindowBorder.Resizable;
		VSync = VSyncMode.Off;

		game = new Game(this);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);

		GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

		float ratio = Width / (float)Height;
		game.UpdateProjection(Matrix4.CreatePerspectiveOffCenter(-1, 1, -1 / ratio, 1 / ratio, 1, 1000));
	}

	protected override void OnUpdateFrame(FrameEventArgs e)
	{
		base.OnUpdateFrame(e);

		if (this.Focused) KeyboardInput.Update();
		game.Logic();
	}

	protected override void OnRenderFrame(FrameEventArgs e)
	{
		base.OnRenderFrame(e);

		GL.Clear(ClearBufferMask.ColorBufferBit);

		game.Draw();

		SwapBuffers();
	}
}