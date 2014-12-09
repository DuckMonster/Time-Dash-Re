using System;

using OpenTK;
using TKTools;

public class Map
{
	public static ShaderProgram defaultProgram;
	Camera camera;
	Environment environment;

	public Actor actor;

	public Map()
	{
		defaultProgram = new ShaderProgram("Shaders/standardShader.glsl");

		environment = new Environment(this);
		camera = new Camera(this);
		actor = new Actor(new Vector2(5, 5), this);
	}

	public bool GetCollision(Entity e) { return GetCollision(e.position, e.size); }
	public bool GetCollision(Entity e, Vector2 offset) { return GetCollision(e.position + offset, e.size); }
	public bool GetCollision(Vector2 pos, Vector2 size)
	{
		return environment.GetCollision(pos, size);
	}

	public void Logic()
	{
		camera.Logic();
		environment.Logic();
		actor.Logic();
	}

	public void Draw()
	{
		defaultProgram["view"].SetValue(camera.ViewMatrix);
		environment.Draw();
		actor.Draw();
	}
}