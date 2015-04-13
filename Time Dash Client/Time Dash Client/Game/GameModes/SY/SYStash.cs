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
	Texture pointTexture = Art.Load("Res/circlebig.png");

	protected int scrap = 0;
	protected int targetScrap = 50;

	float progress = 0f;
	float progressTime = 2f;

	CircleBar scrapBar;

	public SYStash(int id, float size, int target, Vector2 position, Map map)
		: base(position, map)
	{
		this.id = id;

		areaSize = size;
		targetScrap = target;

		scrapBar = new CircleBar(size + size * 0.2f, size * 0.2f, -180, -180);

		mesh.Texture = pointTexture;

		mesh.Vertices = new Vector2[] {
			new Vector2(-0.5f, 0),
			new Vector2(-0.5f, 0.5f),
			new Vector2(0.5f, 0.5f),
			new Vector2(0.5f, 0)
		};

		mesh.UV = new Vector2[] {
			new Vector2(0, 0.5f),
			new Vector2(0, 0),
			new Vector2(1, 0),
			new Vector2(1, 0.5f)
		};

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
		if (scrap >= targetScrap)
			scrap = targetScrap;
	}

	public int AddScrap(int n)
	{
		if (scrap + n > targetScrap)
			n = (targetScrap - scrap);

		SetScrap(scrap + n);
		Map.AddEffect(new EffectRing(position, 8f, 1.2f, Color.White, Map));
		progress = 0;

		return n;
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
			if (Map.LocalPlayer.Position.Y >= position.Y && (Map.LocalPlayer.Position - position).Length < areaSize / 2)
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