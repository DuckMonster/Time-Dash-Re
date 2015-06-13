using OpenTK;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectGainScrap : Effect
{
	static Random rng = new Random();

	Timer effectTimer;

	TextBox amountBox = new TextBox(50f);
	Sprite scrapSprite = new Sprite(Art.Load("Res/scrap.png"));

	int amount;
	float scrapRotation;

	Vector2 position;

	Color Color
	{
		get
		{
			if (amount >= 0)
				return new Color(0f, 0.8f, 0f);
			else
				return new Color(0.8f, 0f, 0f);
		}
	}

	public EffectGainScrap(Vector2 position, int amount, Map map)
		:base(map)
	{
		this.position = position;
		this.amount = amount;

		amountBox.SetHeight = 1f;
		amountBox.VerticalAlign = TextBox.VerticalAlignment.Center;
		amountBox.Text = amount.ToString();

		effectTimer = new Timer(1.2f + (float)amount / 40, false);

		scrapRotation = (float)rng.NextDouble() * 360f;
	}

	public override void Dispose()
	{
		scrapSprite.Dispose();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
		{
			Remove();
			return;
		}

		effectTimer.Logic();
		scrapRotation += 50f * Game.delta;

		base.Logic();
	}

	public override void Draw()
	{
		float f = 1f - TKMath.Exp(effectTimer.PercentageDone, 8f);
		float size = 0.8f + (float)amount / 40;

		scrapSprite.Draw(position + new Vector2(-0.1f, 1f * f), size * 0.8f, scrapRotation);

		Mesh m = amountBox.Mesh;

		m.Color = Color * new Color(1f, 1f,1f, 1f - effectTimer.PercentageDone);

		m.Reset();
		m.Translate(position);
		m.Translate(0f, 0.1f + 1f * f);
		m.Scale(size);

		m.Draw();
	}
}