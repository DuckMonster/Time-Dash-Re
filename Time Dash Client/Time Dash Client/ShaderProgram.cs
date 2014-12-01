using System;
using System.Collections.Generic;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;

class ShaderProgram
{
	public enum ArgumentType
	{
		Uniform,
		Attribute
	}

	public struct Argument
	{
		ShaderProgram program;
		ArgumentType type;
		int ID;

		public Argument(ArgumentType type, int ID, ShaderProgram prog)
		{
			program = prog;
			this.type = type;
			this.ID = ID;
		}

		public void SetValue(Vector2 vec)
		{
			GL.UseProgram(program.programID);
			GL.Uniform2(ID, ref vec);
		}

		public void SetValue(Vector3 vec)
		{
			GL.UseProgram(program.programID);
			GL.Uniform3(ID, ref vec);
		}

		public void SetValue(Vector4 vec)
		{
			GL.UseProgram(program.programID);
			GL.Uniform4(ID, ref vec);
		}

		public void SetValue(Matrix4 mat)
		{
			GL.UseProgram(program.programID);
			GL.UniformMatrix4(ID, false, ref mat);
		}
	}

	Dictionary<string, int> attributeList = new Dictionary<string, int>();
	Dictionary<string, int> uniformList = new Dictionary<string, int>();
	int vertexID, fragmentID, programID;

	public ShaderProgram(string source)
	{
		string vertexSrc = "", fragmentSrc = "";
		ReadFileCombined(source, ref vertexSrc, ref fragmentSrc);

		vertexID = CreateShader(ShaderType.VertexShader, vertexSrc);
		fragmentID = CreateShader(ShaderType.FragmentShader, fragmentSrc);
		programID = CreateProgram(vertexID, fragmentID);
	}

	static int CreateShader(ShaderType type, string src)
	{
		int id = GL.CreateShader(type);
		GL.ShaderSource(id, src);
		GL.CompileShader(id);

		return id;
	}

	static int CreateProgram(int vertex, int fragment)
	{
		int id = GL.CreateProgram();
		GL.AttachShader(id, vertex);
		GL.AttachShader(id, fragment);
		GL.LinkProgram(id);

		Console.WriteLine("Program log:");
		Console.WriteLine(GL.GetProgramInfoLog(id));

		return id;
	}

	public int GetAttribute(string name)
	{
		if (attributeList.ContainsKey(name)) return attributeList[name];

		int attrib = GL.GetAttribLocation(programID, name);
		if (attrib != -1)
		{
			attributeList.Add(name, attrib);
		}

		if (attrib == -1) throw new NullReferenceException();
		return attrib;
	}

	public int GetUniform(string name)
	{
		if (uniformList.ContainsKey(name)) return uniformList[name];

		int uni = GL.GetUniformLocation(programID, name);
		if (uni != -1)
		{
			uniformList.Add(name, uni);
		}

		if (uni == -1) throw new NullReferenceException();
		return uni;
	}

	public void BindVBO(int vbo, string attr)
	{
		int att = GetAttribute(attr);
		GL.VertexAttribPointer(att, 3, VertexAttribPointerType.Float, false, 0, 0);
	}

	public void Use()
	{
		GL.UseProgram(programID);
		foreach (KeyValuePair<string, int> entry in attributeList)
		{
			GL.EnableVertexAttribArray(entry.Value);
		}
	}

	public void Clean()
	{
		foreach (KeyValuePair<string, int> entry in attributeList)
		{
			GL.DisableVertexAttribArray(entry.Value);
		}
	}

	public Argument this[string name]
	{
		get
		{
			return new Argument(ArgumentType.Uniform, GetUniform(name), this);
		}
	}

	public static void ReadFileCombined(string filename, ref string vertexSrc, ref string fragmentSrc)
	{
		using (StreamReader reader = new StreamReader(filename))
		{
			string head = "";
			string src = "";
			string line = "";

			while (!reader.EndOfStream)
			{
				line = reader.ReadLine();

				if (head == "")
				{
					if (line.StartsWith("@")) head = line;
				}
				else
				{
					if (line.StartsWith("@"))
					{
						switch (head[1])
						{
							case 'v':
								vertexSrc = src;
								break;

							case 'f':
								fragmentSrc = src;
								break;
						}

						head = "";
						src = "";
					}
					else src += line + "\n";
				}
			}
		}
	}
}
