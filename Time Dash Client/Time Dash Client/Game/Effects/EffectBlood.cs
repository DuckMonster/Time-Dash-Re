using OpenTK;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectBlood : Effect
{
	static Color defaultColor = new Color(0.9f, 0f, 0.01f);

	Vector2 position;
	Vector2 velocity;

	float size;

	Sprite sprite;

	bool active = true;

	Timer effectTimer = new Timer(1.2f, false);

	public EffectBlood(Vector2 position, float size, Vector2 force, Map m)
		:base(m)
	{
		this.position = position;
		this.velocity = force;
		this.size = size;

		sprite = new Sprite();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
			Remove();

		effectTimer.Logic();

		Color c = defaultColor;
		c.A = (1f - effectTimer.PercentageDone);
		sprite.Color = c;

		sprite.SetTransform(position, size, TKMath.GetAngle(velocity));

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

		sprite.Draw();
	}
}