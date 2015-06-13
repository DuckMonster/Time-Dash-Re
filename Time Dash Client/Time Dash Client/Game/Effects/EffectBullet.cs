using OpenTK;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class EffectBullet : Effect
{
	Timer effectTimer;
	Mesh mesh;

	Vector2 a, b;

	public EffectBullet(Vector2 a, Vector2 b, Map map)
		: base(map)
	{
		this.a = a;
		this.b = b;

		mesh = new Mesh(new Vector3[] {
			new Vector3(0, 0, 0),
			new Vector3(0.2f, -0.5f, 0),
			new Vector3(1f, 0, 0),
			new Vector3(0.2f, 0.5f, 0)
		});

		effectTimer = new Timer(0.8f, false);
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

		float w = TKMath.Exp(effectTimer.PercentageDone, 5);

		mesh.Reset();
		mesh.Translate(a);
		mesh.RotateZ(TKMath.GetAngle(a, b));
		mesh.Scale((b - a).Length, w);

		effectTimer.Logic();

		base.Logic();
	}

	public override void Draw()
	{
		mesh.Draw();
	}
}