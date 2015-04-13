using OpenTK;
using System;
using TKTools;
public class SYScrap : Entity
{
	static Random rng = new Random();

	protected new SYMap Map
	{
		get { return (SYMap)base.Map; }
	}

	Texture scrapTexture = Art.Load("Res/scrap.png");

	public int id;
	Vector2 velocity;
	float rotation;

	Timer ringTimer = new Timer(1.2f, false);

	public SYScrap(int id, Vector2 position, Vector2 velocity, Map map)
		: base(position, map)
	{
		this.id = id;
		this.position = position;
		this.velocity = velocity;

		size = new Vector2(0.5f, 0.5f);
		mesh.Texture = scrapTexture;

		rotation = 360f * (float)rng.NextDouble();
	}

	public void CollectedBy(Player p)
	{
		Map.AddEffect(new EffectRing(position, 3f, 0.6f, Color.White, Map));
		Map.RemoveScrap(this);
	}

	public override void Logic()
	{
		base.Logic();

		position += velocity * Game.delta;
		velocity -= velocity * 6f * Game.delta;

		rotation += 35f * Game.delta;

		if (velocity.LengthFast > 0.5f)
		{
			if (Map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
				velocity.X *= -0.6f;
			if (Map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
				velocity.Y *= -0.6f;
		}

		ringTimer.Logic();
		if (ringTimer.IsDone)
		{
			Map.AddEffect(new EffectRing(Position, 1.5f, 0.7f, Color.White, Map));
			ringTimer.Reset();
		}
	}

	public override void Draw()
	{
		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(size);
		mesh.Rotate(rotation);

		mesh.Draw();
	}
}