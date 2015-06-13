using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectSmoke : Effect
{
	static Random rng = new Random();

	public static Color defaultColor = new Color(212, 190, 166);

	Vector2 position;
	Sprite sprite;

	Timer smokeTimer = new Timer(1f, false);
	Color color;

	float smokeSize;
	Vector2 direction;

	float speed;

	public float Size
	{
		get
		{
			if (smokeTimer.PercentageDone < 0.3f)
			{
				float s = 1 - TKMath.Exp(smokeTimer.PercentageDone / 0.3f, 10f);
				return smokeSize * s;
			}
			else
			{
				float p = ((smokeTimer.PercentageDone - 0.3f) / 0.7f);
				return smokeSize - TKMath.Exp(1f - p, 5f) * smokeSize;
			}
		}
	}

	public EffectSmoke(Vector2 position, float size, float time, float floatDirection, float velocity, Color color, Map m)
		:base(m)
	{
		this.position = position;

		this.color = color;
		direction = TKMath.GetAngleVector(floatDirection);
		speed = velocity;
		smokeSize = size;

		sprite = new Sprite(Art.Load("Res/circlebig.png"));

		smokeTimer.Reset(time);

		Logic();
	}

	public override void Dispose()
	{
		base.Dispose();
		sprite.Dispose();
	}

	public override void Logic()
	{
		if (smokeTimer.IsDone)
			Remove();

		base.Logic();

		smokeTimer.Logic();
		position += direction * speed * Game.delta;

		speed -= speed * 4f * Game.delta;

		color.A = 1f;

		sprite.Position = new Vector3(position);
		sprite.ScaleF = Size;
	}

	public override void Draw()
	{
		if (smokeTimer.IsDone) return;

		sprite.Draw();
	}
}
