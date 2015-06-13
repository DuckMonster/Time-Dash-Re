using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools;
using TKTools.Context;

public class EffectBaseHit : Effect
{
	Sprite sprite;

	Timer effectTimer = new Timer(0.1f, false);

	public EffectBaseHit(SYBase b, float size, Map m)
		:base(m)
	{
		size = MathHelper.Clamp(size, 0.2f, 2f);

		sprite = new Sprite(b.sprite.Texture);
		sprite.SetTransform(b.sprite.Position, b.sprite.Scale * 1.1f, b.sprite.Rotation);

		effectTimer.Reset(size / 7f);
	}

	public override void Dispose()
	{
		base.Dispose();

		sprite.Dispose();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
			Remove();

		Color color = new Color(1, 1, 1, 1f - effectTimer.PercentageDone);
		sprite.Color = color;

		effectTimer.Logic();

		base.Logic();
	}

	public override void Draw()
	{
		sprite.Draw();
	}
}