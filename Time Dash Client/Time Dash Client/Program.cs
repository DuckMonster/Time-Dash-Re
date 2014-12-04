﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

class Program : GameWindow
{
	Game game;

	static void Main(string[] args)
	{
		using (Program prog = new Program())
		{
			prog.Run(150.0);
		}
	}

	public Program()
		: base()
	{
		KeyDown += KeyHandle;
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

		VSync = VSyncMode.Off;

		game = new Game();
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

		KeyboardInput.Update();
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