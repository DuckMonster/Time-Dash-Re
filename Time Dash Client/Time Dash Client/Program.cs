using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

public class Program : GameWindow
{
	Game game;

	static void Main(string[] args)
	{
		#region Server or client
		bool valid = false;

		while (!valid)
		{
			try
			{
				//string[] comm = Console.ReadLine().Split(' ');
				string[] comm = "join 90.224.59.61".Split(' ');

				if (comm[0] == "join")
				{
					Game.hostIP = comm[1];
					valid = true;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Invalid input.");
				Console.WriteLine(e);
			}
		}
		#endregion

		using (Program prog = new Program())
		{
			prog.Run();
		}
	}

	public Program()
		: base()
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

		Width = 1024;
		Height = 768;

		WindowBorder = OpenTK.WindowBorder.Fixed;

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