using System;
using Tao.FreeGlut;
using OpenGL;

class Program
{
	static void Main(string[] args)
	{
		Glut.glutInit();
		Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE);
		Glut.glutInitWindowSize(1024, 768);
		Glut.glutCreateWindow("OpenGL");

		Glut.glutIdleFunc(OnRenderFrame);
		Glut.glutDisplayFunc(OnDisplay);
		Glut.glutCloseFunc(OnClose);

		Glut.glutMainLoop();
	}

	static void OnDisplay()
	{
	}

	static void OnClose()
	{
	}

	static void OnRenderFrame()
	{
		Gl.ClearColor(1, 0, 0, 1);
		Gl.Clear(ClearBufferMask.ColorBufferBit);

		Glut.glutSwapBuffers();
	}
}