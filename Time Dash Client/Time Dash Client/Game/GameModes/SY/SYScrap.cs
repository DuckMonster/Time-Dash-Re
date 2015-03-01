using OpenTK;
using TKTools;
public class SYScrap : Entity
{
	public static Texture scrapTexture = new Texture("Res/circlebig.png");

	protected new SYMap Map
	{
		get { return (SYMap)base.Map; }
	}

	public int id;
	Vector2 velocity;

	public SYScrap(int id, Vector2 position, Vector2 velocity, Map map)
		: base(position, map)
	{
		this.id = id;
		this.position = position;
		this.velocity = velocity;

		size = new Vector2(0.5f, 0.5f);
		mesh.Texture = scrapTexture;
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

		if (velocity.LengthFast > 0.5f)
		{
			if (Map.GetCollision(this, new Vector2(velocity.X, 0) * Game.delta))
				velocity.X *= -0.6f;
			if (Map.GetCollision(this, new Vector2(0, velocity.Y) * Game.delta))
				velocity.Y *= -0.6f;
		}
	}

	public override void Draw()
	{
		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(size);

		mesh.Draw();
	}
}