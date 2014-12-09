using OpenTK;
using OpenTK.Input;
using System;
using TKTools;

public class Actor : Entity
{
	public static float GRAVITY = 40f, MAX_VELOCITY = 8f, ACCELERATION_SPEED = 0.4f, JUMP_FORCE = 10f,
		JUMP_ADD_FORCE = 20f, JUMP_ADD_LIM = 2f;

	Vector2 velocity = Vector2.Zero;
	float acceleration, friction;
	int dir = 1;

	public void CalculatePhysics()
	{
		friction = -(float)(Math.Log(0.02, Math.E) / ACCELERATION_SPEED);
		acceleration = MAX_VELOCITY * friction;
	}

	public Actor(Vector2 position, Map m)
		: base(position, m)
	{
		CalculatePhysics();
		mesh.Texture = new TKTools.Texture("Res/guy.png");

		float w = (size.X / size.Y)/2;

		mesh.UV = new Vector2[] {
			new Vector2(0.5f-w, 0f),
			new Vector2(0.5f+w, 0f),
			new Vector2(0.5f-w, 1f),
			new Vector2(0.5f+w, 1f)
		};
	}

	public bool IsOnGround
	{
		get
		{
			return map.GetCollision(this, new Vector2(0, -0.1f));
		}
	}

	public void Logic()
	{
		Input();
		velocity.X -= velocity.X * friction * Game.delta;
		velocity.Y -= GRAVITY * Game.delta;

		if (map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
			velocity.Y = 0;
		if (map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
			velocity.X = 0;

		position += velocity * Game.delta;

		if (velocity.X > 0) dir = 1;
		if (velocity.X < 0) dir = -1;
	}

	public void Input()
	{
		if (KeyboardInput.Current[Key.Right]) velocity.X += acceleration * Game.delta;
		if (KeyboardInput.Current[Key.Left]) velocity.X -= acceleration * Game.delta;
		if (IsOnGround && KeyboardInput.Current[Key.Z] && !KeyboardInput.Previous[Key.Z]) Jump();
		if (KeyboardInput.Current[Key.Z]) JumpHold();
	}

	public void Jump()
	{
		velocity.Y = JUMP_FORCE;
	}

	public void JumpHold()
	{
		if (velocity.Y >= JUMP_ADD_LIM) velocity.Y += JUMP_ADD_FORCE * Game.delta;
	}

	new public void Draw()
	{
		mesh.Color = Color.White;

		mesh.Reset();

		mesh.Scale(size);
		mesh.Scale(new Vector2(-dir, 1));
		mesh.Translate(position);

		mesh.Draw();
	}
}