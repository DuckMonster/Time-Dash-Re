using System;
using OpenTK;
using GRFX = OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Threading;
using OpenTK.Input;

namespace MapEditor
{
	public class EditorProgram : GameWindow
	{
		Container container;
		string startFile = null;

		public EditorProgram(int w, int h, GRFX.GraphicsMode mode)
			: base(w, h, mode)
		{
		}

		public void OpenFile(string path)
		{
			if (container != null)
				container.Load(path);
			else
				startFile = path;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Title = "Map Editor";

			GL.ClearColor(0f, 0f, 0f, 1f);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

			WindowBorder = OpenTK.WindowBorder.Resizable;
			VSync = VSyncMode.Off;

			container = new Container(this);
			if (startFile != null) container.Load(startFile);
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			container.DisposeEditor();
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);
			MouseInput.CurrentX = e.X;
			MouseInput.CurrentY = e.Y;
		}

		protected override void OnKeyDown(KeyboardKeyEventArgs e)
		{
			base.OnKeyDown(e);

			//if (e.Key == Key.Escape) Exit();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(ClientRectangle);
			container.editor.UpdateProjection(new Vector2(Width, Height));
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			if (Focused)
			{
				MouseInput.Update(this);
				KeyboardInput.Update();
			}

			container.Logic();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			if (MouseInput.Current == null || KeyboardInput.Current == null) return;

			base.OnRenderFrame(e);

			container.Draw();

			SwapBuffers();
		}
	}
}