using System;
using OpenTK;
using GRFX = OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Threading;
using OpenTK.Input;

namespace MapEditor
{
	class EditorProgram : GameWindow
	{
		Editor editor;

		public EditorProgram(int w, int h, GRFX.GraphicsMode mode)
			: base(w, h, mode)
		{
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

			editor = new Editor();
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

			if (e.Key == Key.Escape) Exit();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(ClientRectangle);
			editor.UpdateProjection(new Vector2(ClientSize.Width, ClientSize.Height));
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

			if (Focused)
			{
				MouseInput.Update(this);
				KeyboardInput.Update();
			}

			Console.Clear();
			Console.WriteLine("X: {0}, Y: {1}", MouseInput.Current.X, MouseInput.Current.Y);
			Console.WriteLine("LB: {0}, RB: {1}, MB: {2}, X1: {3}, X2: {4}",
				MouseInput.Current[MouseButton.Left], MouseInput.Current[MouseButton.Right], MouseInput.Current[MouseButton.Middle],
				MouseInput.Current[MouseButton.Button1], MouseInput.Current[MouseButton.Button2]);

			Console.WriteLine("Scroll: {0}", MouseInput.Current.Wheel);

			editor.Logic();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			editor.Draw();

			SwapBuffers();
		}
	}
}