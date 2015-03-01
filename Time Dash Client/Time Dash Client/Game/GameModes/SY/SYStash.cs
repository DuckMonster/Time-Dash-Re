using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using TKTools;

public class SYStash : Entity
{
	protected new SYMap Map
	{
		get
		{
			return (SYMap)base.Map;
		}
	}

	public int id;

	Mesh progressMesh = Mesh.Box;

	float areaSize = 5f;
	Texture pointTexture = new Texture("Res/circlebig.png");

	int scrap = 0;
	int targetScrap = 50;

	float progress = 0f;
	float progressTime = 2f;

	CircleBar scrapBar = new CircleBar(6f, 1f, -180, -180);

	public SYStash(int id, Vector2 position, Map map)
		: base(position, map)
	{
		this.id = id;

		mesh.Texture = pointTexture;

		progressMesh.Vertices = new Vector2[] {
			new Vector2(0, 0.5f),
			new Vector2(1, 0.5f),
			new Vector2(1, -0.5f),
			new Vector2(0, -0.5f)
		};
	}

	public void SetScrap(int n)
	{
		scrap = n;
	}

	public void AddScrap(int n)
	{
		scrap += n;
		Map.AddEffect(new EffectRing(position, 8f, 1.2f, Color.White, Map));
		progress = 0;
	}

	public void Hold()
	{
		progress += (1f / progressTime) * Game.delta;
	}

	public override void Logic()
	{
		base.Logic();

		scrapBar.Progress = (float)scrap / targetScrap;
		scrapBar.Logic();

		if (Map.LocalPlayer != null && ((SYPlayer)Map.LocalPlayer).CollectedScrap > 0)
		{
			if (Map.LocalPlayer.position.Y >= position.Y && (Map.LocalPlayer.position - position).Length < areaSize / 2)
				Hold();
			else
				progress = 0;
		}
	}

	public override void Draw()
	{
		GL.Enable(EnableCap.StencilTest);
		GL.Clear(ClearBufferMask.StencilBufferBit);

		GL.StencilFunc(StencilFunction.Always, 1, 0xff);
		GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);

		mesh.Color = new Color(1f, 1f, 1f, progress > 0 ? 0.3f : 0.1f);

		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(areaSize);

		mesh.Draw();

		GL.StencilFunc(StencilFunction.Equal, 1, 0xff);

		progressMesh.Reset();

		progressMesh.Translate(position);
		progressMesh.Translate(new Vector2(-0.5f, 0) * areaSize);
		progressMesh.Scale(new Vector2(progress, 1) * areaSize);

		progressMesh.Draw();

		GL.Disable(EnableCap.StencilTest);

		scrapBar.Draw(position, Color.White);
	}
}