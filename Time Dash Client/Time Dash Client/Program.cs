using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;

class Program : GameWindow
{
	public static ShaderProgram program;
	Mesh mesh;
	float f = 0;

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

		//mesh = Mesh.Triangle;		

		//program["projection"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, Width / (float)Height, 1f, 1000f));
		//program["view"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.UnitY));
		//program["model"].SetValue(Matrix4.Identity);
	}

	protected override void OnResize(EventArgs e)
	{
		base.OnResize(e);

		GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
	}

	protected override void OnUpdateFrame(FrameEventArgs e)
	{
		base.OnUpdateFrame(e);


	}

	protected override void OnRenderFrame(FrameEventArgs e)
	{
		base.OnRenderFrame(e);

		f += 0.005f;

		GL.Clear(ClearBufferMask.ColorBufferBit);

		VBO<Vector3> vbo = new VBO<Vector3>();
		vbo.UploadData(new Vector3[] {
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(0f, 0.5f, 0f)
		});

		VBO<Vector3> vbo2 = new VBO<Vector3>();
		vbo2.UploadData(new Vector3[] {
			new Vector3(f, f, 0),
			new Vector3(f, f, 0),
			new Vector3(f, f, 0)
		});

		program["vertexPosition"].SetValue(vbo);
		program["vertexTemp"].SetValue(vbo2);

		/*
		int manuVBO = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, manuVBO);
		Vector2[] data = new Vector2[]{
			new Vector2(f, 0),
			new Vector2(f, 0),
			new Vector2(f, 0)
		};

		GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr)(Vector2.SizeInBytes * 3), data, BufferUsageHint.StaticDraw);
		program.Use();
		int attr = GL.GetAttribLocation(program.programID, "vertexTemp");
		GL.EnableVertexAttribArray(attr);
		GL.VertexAttribPointer(attr, 3, VertexAttribPointerType.Float, false, 0, 0);
		 * */

		program.Use();
		GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
		program.Clean();

		SwapBuffers();
	}
}