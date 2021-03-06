﻿using OpenTK.Graphics;
using System;
using TKTools.Context;

public class Program : IDisposable
{
	public static Context context;

	Game game;
	Camera defaultCamera;

	public Program(int width, int height, GraphicsMode gm)
	{
		context = new Context(width, height, gm);

		context.OnBegin += Begin;
		context.OnUpdate += Update;
		context.OnRender += Render;

		defaultCamera = new Camera();
		defaultCamera.Use();
	}

	public void Run()
	{
		context.Run();
	}

	public void Dispose()
	{
		game.Dispose();
		Log.ShutDown();
		game = null;
	}

	void Begin()
	{
		game = new Game(this);
		GL.Disable(EnableCap.DepthTest);
		GL.ClearColor(.7f, .7f, .7f, 1f);
	}

	void Update()
	{
		game.Logic();
	}

	void Render()
	{
		game.Draw();
	}
}

/*
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

	int mousex, mousey;

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

		if (a.Key == Key.F3)
		{
			this.Size = new Size(400, 400);
		}
	}

	protected override void OnMouseMove(MouseMoveEventArgs e)
	{
		base.OnMouseMove(e);
		mousex = e.X;
		mousey = e.Y;
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

		Icon = new Icon("icon.ico");

		WindowBorder = WindowBorder.Resizable;
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

		if (Focused)
		{
			KeyboardInput.Update();
			MouseInput.Update(mousex, mousey, this);
		}

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
*/