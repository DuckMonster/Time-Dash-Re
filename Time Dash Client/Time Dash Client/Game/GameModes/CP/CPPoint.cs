using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using TKTools;

public class CPPoint : Entity
{
	public int id;

	public Team owner = null;
	public Team currentContender = null;
	public float radius = 4f;

	float progress = 0f;

	Texture circle = Art.Load("Res/circleBig.png");

	public CPPoint(int id, Vector2 position, Map map)
		: base(position, map)
	{
		this.id = id;

		sprite.Mesh.UV = new Vector2[] {
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(0, 0.5f),
			new Vector2(1, 0.5f)
		};

		sprite.Texture = circle;

		sprite.Position = new Vector3(position);
		sprite.ScaleF = radius * 2;
	}

	public override void Dispose()
	{
		base.Dispose();
		circle.Dispose();
	}

	public void SetOwner(Team t)
	{
		owner = t;
		progress = t == null ? 0f : 1f;
	}

	public void SetProgress(float p)
	{
		progress = p;
	}

	public List<Team> GetContenders()
	{
		List<Team> returnList = new List<Team>(5);
		List<Player> playerList = Map.GetActorRadius<Player>(position, radius);

		foreach (Player p in playerList)
		{
			if (p.Position.Y < Position.Y) continue;
			if (!returnList.Contains(p.Team)) returnList.Add(p.Team);
		}

		return returnList;
	}
	
	public override void Logic()
	{
		List<Team> contenders = GetContenders();
		if (contenders.Count == 1)
		{
			currentContender = contenders[0];

			if (owner == null)
			{
				progress += 0.25f * Game.delta;
				if (progress >= 1f)
					owner = contenders[0];
			}
			else if (owner != contenders[0])
			{
				progress -= 0.4f * Game.delta;
				if (progress <= 0f)
					owner = null;
			}
		}
		else
		{
			if (contenders.Count == 0)
			{
				if (owner == null) progress -= 0.15f * Game.delta;
				else progress += 0.15f * Game.delta;
			}

			currentContender = null;
		}

		progress = MathHelper.Clamp(progress, 0, 1);
	}

	public override void Draw()
	{
		GL.Enable(EnableCap.StencilTest);

		GL.StencilMask(0xFF);
		GL.Clear(ClearBufferMask.StencilBufferBit);

		GL.StencilFunc(StencilFunction.Never, 1, 0xff);
		GL.StencilOp(StencilOp.Replace, StencilOp.Keep, StencilOp.Keep);

		sprite.Draw(position, radius * 2 * 1.1f, 180f);

		GL.StencilFunc(StencilFunction.Notequal, 1, 0xff);

		Color c;

		if (owner != null)
			c = owner.Color;
		else
			c = currentContender == null ? Color.Gray : currentContender.Color;

		sprite.Color = Color.Gray * new Color(1, 1, 1, 0.4f);
		sprite.Draw(position, radius * 2, 0f);

		sprite.Color = c * new Color(1, 1, 1, 0.4f);
		sprite.Draw(position, radius * 2, 180f - (180 * progress));

		GL.Disable(EnableCap.StencilTest);
	}
}