using OpenTK;
using OpenTK.Graphics.OpenGL;
using TKTools;
using TKTools.Context;

public class EffectTowerHit : Effect
{
	Mesh headMesh, barrelMesh;

	Timer effectTimer = new Timer(0.1f, false);

	public EffectTowerHit(SYTower tower, float dir, float size, Map m)
		:base(m)
	{
		size = MathHelper.Clamp(size, 0.2f, 2f);

		headMesh = Mesh.CreateFromTexture(Art.Load("Res/cannon_head.png"));
		barrelMesh = Mesh.CreateFromTexture(Art.Load("Res/cannon_brl.png"));

		headMesh.FillColor = true;
		barrelMesh.FillColor = true;

		headMesh.Color = Color.White;
		barrelMesh.Color = Color.White;
		
		headMesh.Translate(tower.Position);
		headMesh.Scale(3f);
		headMesh.RotateZ(dir);

		barrelMesh.Translate(tower.Position);
		barrelMesh.Scale(2.1f);
		barrelMesh.RotateZ(dir);
		barrelMesh.Translate(1f, 0f);

		barrelMesh.Scale(1f);

		effectTimer.Reset(size / 7f);
	}

	public override void Dispose()
	{
		base.Dispose();
		headMesh.Dispose();
		barrelMesh.Dispose();
	}

	public override void Logic()
	{
		if (effectTimer.IsDone)
			Remove();

		Color color = new Color(1, 1, 1, 1f - effectTimer.PercentageDone);
		headMesh.Color = color;
		barrelMesh.Color = color;

		effectTimer.Logic();

		base.Logic();
	}

	public override void Draw()
	{
		headMesh.Draw();
		barrelMesh.Draw();
	}
}