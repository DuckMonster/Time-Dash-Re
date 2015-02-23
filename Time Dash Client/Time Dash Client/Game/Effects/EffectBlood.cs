using OpenTK;
using TKTools;
public class EffectBlood : Effect
{
	static Color defaultColor = new Color(0.9f, 0f, 0.01f);

	Vector2 position;
	Vector2 velocity;

	float size;

	Mesh mesh;

	bool active = true;

	Timer effectTimer = new Timer(1.2f, false);

	public EffectBlood(Vector2 position, float size, Vector2 force, Map m)
		:base(m)
	{
		this.position = position;
		this.velocity = force;
		this.size = size;

		mesh = Mesh.Box;
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
			Remove();

		effectTimer.Logic();

		Color c = defaultColor;
		c.A = (1f - effectTimer.PercentageDone);
		mesh.Color = c;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Rotate(TKMath.GetAngle(velocity));
		mesh.Scale(size + size * velocity.Length * 0.02f, size);

		if (!active)
			return;

		base.Logic();

		velocity.Y -= Stats.defaultStats.Gravity * Game.delta;
		position += velocity * Game.delta;

		if (map.GetCollision(position, new Vector2(0.01f, 0.01f)))
		{
			active = false;
			velocity = Vector2.Zero;
		}
	}

	public override void Draw()
	{
		if (effectTimer.IsDone) return;

		mesh.Draw();
	}
}