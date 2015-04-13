using System.Collections.Generic;
using TKTools;

public class Art
{
	static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
	public static Texture Load(string path)
	{
		if (!textures.ContainsKey(path))
			textures[path] = new Texture(path);

		return textures[path];
	}

	public static void Dispose(string path)
	{
		if (textures.ContainsKey(path)) textures[path].Dispose();
	}

	public static void DisposeAll()
	{
		foreach(KeyValuePair<string, Texture> p in textures)
			p.Value.Dispose();

		textures.Clear();
	}
}