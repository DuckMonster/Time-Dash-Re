using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools;

public class EffectBaseHit : Effect
{
	Mesh mesh;

	Timer effectTimer = new Timer(0.1f, false);

	public EffectBaseHit(SYBase b, float size, Map m)
		:base(m)
	{
		size = MathHelper.Clamp(size, 0.2f, 2f);

		mesh = Mesh.Box;
		mesh.Texture = b.mesh.Texture;
		mesh.ModelMaxtrix = b.mesh.ModelMaxtrix;

		mesh.Scale(1.1f);

		effectTimer.Reset(size / 7f);
	}

	public override void Dispose()
	{
		base.Dispose();

		mesh.Dispose();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
			Remove();

		Color color = new Color(1, 1, 1, 1f - effectTimer.PercentageDone);
		mesh.Color = color;

		effectTimer.Logic();

		base.Logic();
	}

	public override void Draw()
	{
		mesh.Draw();
	}
}