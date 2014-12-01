using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

class Program : GameWindow
{
	ShaderProgram program;
	int vbo_position;
	Vector3[] positionData;

	static void Main(string[] args)
	{
		using (Program prog = new Program())
		{
			prog.Run(30.0);
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		Title = "OpenTK Test";

		GL.ClearColor(1f, 0f, 0f, 1f);

		program = new ShaderProgram("Shaders/standardShader.glsl");

		vbo_position = GL.GenBuffer();

		positionData = new Vector3[] {
			new Vector3(-1f, -1f, 0f),
			new Vector3(1f, -1f, 0f),
			new Vector3(0f, 1f, 0f)
		};

		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
		GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(Vector3.SizeInBytes * positionData.Length), positionData, BufferUsageHint.StaticDraw);
		program.BindVBO(vbo_position, "vertexPosition");

		GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

		program["projection"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, Width / (float)Height, 1f, 1000f));
		program["view"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.UnitY));
		program["model"].SetValue(Matrix4.Identity);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);

		GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
	}

	protected override void OnRenderFrame(FrameEventArgs e)
	{
		base.OnRenderFrame(e);
		GL.Clear(ClearBufferMask.ColorBufferBit);

		program.Use();
		GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

		SwapBuffers();
	}
}