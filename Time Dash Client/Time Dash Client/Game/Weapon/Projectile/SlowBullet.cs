using OpenTK;
using TKTools;
using TKTools.Context;

public class SlowBullet : Projectile
{
	class Trail : Effect
	{
		Vector2 position;
		Sprite sprite = new Sprite();
		Timer effectTimer = new Timer(0.5f, false);

		public Trail(Vector2 position, Texture t, Map map)
			:base(map)
		{
			sprite.Texture = t;
			this.position = position;
		}

		public override void Logic()
		{
			base.Logic();

			if (effectTimer.IsDone)
			{
				Remove();
				return;
			}

			effectTimer.Logic();
		}

		public override void Draw()
		{
			sprite.Color = new Color(1, 1, 1, 1 - effectTimer.PercentageDone);
			sprite.Draw(position, 0.2f * (1f - effectTimer.PercentageDone), 0f);
		}
	}

	static readonly float speed = 5f;

	Texture texture = Art.Load("Res/circlebig.png");
	Timer trailTimer = new Timer(0.05f, false);

	public SlowBullet(Actor owner, int id, Vector2 position, Vector2 target, Map m)
		:base(owner, id, position, m)
	{
		velocity = (target - position).Normalized() * speed;
		sprite.Texture = texture;

		size = new Vector2(0.2f, 0.2f);
	}

	public override void Logic()
	{
		if (!Active) return;

		Vector2 stepVector = velocity * Game.delta;

		if (Map.GetCollision(this, stepVector))
		{
			Vector2 collidePos;
			Map.RayTraceCollision(position, position + stepVector, new Vector2(0.1f, 0.1f), out collidePos);
			position = collidePos;

			Hit();
		}

		trailTimer.Logic();
		if (trailTimer.IsDone)
		{
			Map.AddEffect(new Trail(Position, texture, Map));
			trailTimer.Reset();
		}

		base.Logic();
	}
}