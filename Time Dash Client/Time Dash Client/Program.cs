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
		GL.DepthFunc(DepthFunction.Lequal);
		GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

		//GL.Enable(EnableCap.DepthTest);

		Icon = new System.Drawing.Icon("icon.ico");

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