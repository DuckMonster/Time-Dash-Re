using System;
using OpenGL;

class Camera
{
	private Vector3 position = new Vector3(0, 0, 10);

	public Matrix4 GetViewMatrix()
	{
		return Matrix4.LookAt(position, new Vector3(position.x, position.y, 0), Vector3.Up);
	}

	public void Logic()
	{
		if (Keyboard.Current['d']) position.x += 2f * Game.delta;
		if (Keyboard.Current['a']) position.x -= 2f * Game.delta;
		if (Keyboard.Current['w']) position.y += 2f * Game.delta;
		if (Keyboard.Current['s']) position.y -= 2f * Game.delta;
		if (Keyboard.Current['e']) position.z -= 5f * Game.delta;
		if (Keyboard.Current['q']) position.z += 5f * Game.delta;
	}
}
