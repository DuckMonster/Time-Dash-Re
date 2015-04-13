using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using TKTools;

public class Tileset : IDisposable
{
	public static readonly ShaderProgram tileProgram = new ShaderProgram("Shaders/tileShader.glsl");

	public Texture sourceTexture;
	int tileWidth, tileHeight;
	int tilex, tiley;

	public int X
	{
		get
		{
			return tilex;
		}
		set
		{
			tilex = value;
		}
	}
	public int Y
	{
		get
		{
			return tiley;
		}
		set
		{
			tiley = value;
		}
	}
	public int TileWidth
	{
		get
		{
			return tileWidth;
		}
	}
	public int TileHeight
	{
		get
		{
			return tileHeight;
		}
	}

	public Tileset(int width, int height, string filename)
	{
		tileWidth = width;
		tileHeight = height;
		Load(filename);
	}

	public void Dispose()
	{
	}

	public void Load(string filename)
	{
		sourceTexture = Art.Load(filename);
	}

	public void Upload() { Upload(X, Y); }
	public void Upload(int x, int y)
	{
		tileProgram["textureSize"].SetValue(new Vector2(sourceTexture.Width, sourceTexture.Height));
		tileProgram["tileSize"].SetValue(new Vector2(tileWidth, tileHeight));
		tileProgram["tilePosition"].SetValue(new Vector2(x, y));

		sourceTexture.Bind();
	}
}
