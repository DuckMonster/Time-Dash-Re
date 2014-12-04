using OpenTK;
using OpenTK.Input;

public class Actor : Entity
{
	public static float GRAVITY = 40f, FRICTION = 2.5f, ACCERELATION = 20f, JUMP_FORCE = 10f;
	Vector2 velocity = Vector2.Zero;

	public Actor(Vector2 position, Map m)
		: base(position, m)
	{

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
		velocity.X -= velocity.X * FRICTION * Game.delta;
		velocity.Y -= GRAVITY * Game.delta;

		if (map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
			velocity.Y = 0;
		if (map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
			velocity.X = 0;

		position += velocity * Game.delta;
	}

	public void Input()
	{
		if (KeyboardInput.Current[Key.Right]) velocity.X += ACCERELATION * Game.delta;
		if (KeyboardInput.Current[Key.Left]) velocity.X -= ACCERELATION * Game.delta;
		if (IsOnGround && KeyboardInput.Current[Key.Z]) velocity.Y = JUMP_FORCE;
	}
}