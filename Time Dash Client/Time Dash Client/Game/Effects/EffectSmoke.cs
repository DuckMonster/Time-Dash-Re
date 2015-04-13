﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using TKTools;

public class EffectSmoke : Effect
{
	static Random rng = new Random();

	public static Color defaultColor = new Color(212, 190, 166);

	Texture smokeTexture;

	Vector2 position;
	Mesh mesh;

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
		smokeTexture = Art.Load("Res/circlebig.png");

		this.position = position;

		this.color = color;
		direction = TKMath.GetAngleVector(floatDirection);
		speed = velocity;
		smokeSize = size;

		mesh = Mesh.Box;
		mesh.Texture = smokeTexture;

		smokeTimer.Reset(time);

		Logic();
	}

	public override void Dispose()
	{
		base.Dispose();

		mesh.Dispose();
	}

	public override void Logic()
	{
		if (smokeTimer.IsDone)
			Remove();

		base.Logic();

		smokeTimer.Logic();
		position += direction * speed * Game.delta;

		speed -= speed * 4f * Game.delta;

		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(Size);

		color.A = 1f;
		mesh.Color = color;
	}

	public override void Draw()
	{
		if (smokeTimer.IsDone) return;

		mesh.Draw();
	}
}
