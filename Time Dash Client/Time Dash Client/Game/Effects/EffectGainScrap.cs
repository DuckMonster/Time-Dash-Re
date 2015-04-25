using OpenTK;
using System;
using TKTools;

public class EffectGainScrap : Effect
{
	static Random rng = new Random();

	Timer effectTimer;

	TextBox amountBox = new TextBox(50f);
	Mesh scrapMesh = Mesh.Box, amountMesh = Mesh.Box;

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
		scrapMesh.Texture = Art.Load("Res/scrap.png");

		amountBox.SetHeight = 1f;
		amountBox.VerticalAlign = TextBox.VerticalAlignment.Center;
		amountBox.Text = amount.ToString();

		amountMesh.Vertices = amountBox.Vertices;
		amountMesh.UV = amountBox.UV;
		amountMesh.Texture = amountBox.Texture;

		effectTimer = new Timer(1.2f + (float)amount / 40, false);

		scrapRotation = (float)rng.NextDouble() * 360f;
	}

	public override void Dispose()
	{
		amountMesh.Dispose();
		scrapMesh.Dispose();
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

		scrapMesh.Color = new Color(1, 1, 1, 1f - effectTimer.PercentageDone);

		scrapMesh.Reset();
		scrapMesh.Translate(position);
		scrapMesh.Translate(-0.1f, 1f * f);
		scrapMesh.Scale(size * 0.8f);
		scrapMesh.Rotate(scrapRotation);

		scrapMesh.Draw();

		amountMesh.Color = Color * new Color(1f, 1f,1f, 1f - effectTimer.PercentageDone);

		amountMesh.Reset();
		amountMesh.Translate(position);
		amountMesh.Translate(0f, 0.1f + 1f * f);
		amountMesh.Scale(size);

		amountMesh.Draw();
	}
}