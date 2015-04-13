using OpenTK;
using TKTools;

public class SlowBullet : Projectile
{
	class Trail : Effect
	{
		Vector2 position;
		Mesh mesh = Mesh.Box;
		Timer effectTimer = new Timer(0.5f, false);

		public Trail(Vector2 position, Texture t, Map map)
			:base(map)
		{
			mesh.Texture = t;
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
			mesh.Color = new Color(1, 1, 1, 1 - effectTimer.PercentageDone);

			mesh.Reset();

			mesh.Translate(position);
			mesh.Scale(0.2f * (1f - effectTimer.PercentageDone));

			mesh.Draw();
		}
	}

	static readonly float speed = 5f;

	Texture texture = Art.Load("Res/circlebig.png");
	Timer trailTimer = new Timer(0.05f, false);

	public SlowBullet(Actor owner, int id, Vector2 position, Vector2 target, Map m)
		:base(owner, id, position, m)
	{
		velocity = (target - position).Normalized() * speed;
		mesh.Texture = texture;

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