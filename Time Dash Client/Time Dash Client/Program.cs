using System;
using System.IO;
using Tao.FreeGlut;
using OpenGL;

class Program
{
	static Game game;

	static void Main(string[] args)
	{
		Glut.glutInit();
		Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE);
		Glut.glutInitWindowSize(1024, 768);
		Glut.glutCreateWindow("OpenGL");

		Glut.glutIdleFunc(OnRenderFrame);
		Glut.glutDisplayFunc(OnDisplay);
		Glut.glutCloseFunc(OnClose);

		Glut.glutKeyboardFunc(KeyboardDown);
		Glut.glutKeyboardUpFunc(KeyboardUp);

		game = new Game();

		Glut.glutMainLoop();
	}

	static void OnDisplay()
	{
	}

	static void OnClose()
	{
		Garbage.Dispose();
	}

	static void KeyboardDown(byte key, int x, int y)
	{
		Keyboard.SetKey(key, true);
	}

	static void KeyboardUp(byte key, int x, int y)
	{
		Keyboard.SetKey(key, false);
	}

	static void OnRenderFrame()
	{
		Gl.ClearColor(1, 0, 0, 1);
		Gl.Clear(ClearBufferMask.ColorBufferBit);

		Keyboard.Update();

		game.Logic();
		game.Draw();

		Glut.glutSwapBuffers();
	}

	public static ShaderProgram CreateShaderProgram(string vertexPath, string fragmentPath)
	{
		ShaderProgram program = new ShaderProgram(ReadFile(vertexPath), ReadFile(fragmentPath));
		if (program.ProgramLog != "")
		{
			Console.WriteLine(program.ProgramLog);
			Console.ReadKey();

			throw new NullReferenceException();
		}

		program.DisposeChildren = true;
		Garbage.Add(program);

		return program;
	}

	public static string ReadFile(string fileName)
	{
		using (StreamReader stream = new StreamReader(fileName))
		{
			string line = stream.ReadToEnd();
			return line;
		}
	}
}