﻿using OpenTK;
using System;

public class SYPlayer : Player
{
	Mesh scrapMesh = Mesh.Box;

	protected new SYMap Map
	{
		get { return (SYMap)base.Map; }
	}

	int scrap = 0;

	public int CollectedScrap
	{
		get { return scrap; }
	}

	public SYPlayer(int id, string name, Vector2 position, Map m)
		: base(id, name, position, m)
	{
		scrapMesh.Texture = SYScrap.scrapTexture;
		scrap = 1;
	}

	public override void Die(Vector2 diePos)
	{
		base.Die(diePos);
		scrap = 0;
	}

	public void CollectScrap(SYScrap s)
	{
		s.CollectedBy(this);
		scrap++;
	}

	public void ReturnScrap(SYStash stash)
	{
		stash.AddScrap(scrap);
		scrap = 0;
	}

	public override void DrawHUD()
	{
		base.DrawHUD();

		Vector2 scrapHud = position + new Vector2(1, -1);
		float stepSize = 2 / 5f;

		for (int i = 0; i < scrap; i++)
		{
			scrapMesh.Reset();

			scrapMesh.Translate(scrapHud);
			scrapMesh.Translate(new Vector2((int)Math.Floor(i / 5f), i % 5) * stepSize);
			scrapMesh.Scale(stepSize);

			scrapMesh.Draw();
		}
	}
}