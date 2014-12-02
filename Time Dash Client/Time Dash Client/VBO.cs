using System;
using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;

class VBO<T> : IDisposable where T : struct
{
	protected int vbo_ID;
	List<T> vertexList = new List<T>();

	public int Count
	{
		get
		{
			return vertexList.Count;
		}
	}

	public VBO()
	{
		vbo_ID = GL.GenBuffer();
	}

	public void Dispose()
	{
		Console.WriteLine("Disposed of buffer...");
		GL.DeleteBuffer(vbo_ID);
	}

	public void Bind()
	{
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_ID);
	}

	public void UploadData(T[] data)
	{
		vertexList.Clear();
		vertexList.AddRange(data);

		int size = 0;

		switch (typeof(T).Name)
		{
			case "Vector2": size = Vector2.SizeInBytes; break;
			case "Vector3": size = Vector3.SizeInBytes; break;
			case "Vector4": size = Vector4.SizeInBytes; break;
		}

		Bind();
		GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(size * data.Length), data, BufferUsageHint.StaticDraw);
	}
}