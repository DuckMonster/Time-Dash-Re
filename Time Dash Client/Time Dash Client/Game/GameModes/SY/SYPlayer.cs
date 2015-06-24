using OpenTK;
using System;
using TKTools;
using TKTools.Context;

public class SYPlayer : Player
{
	Sprite scrapSprite = new Sprite(Art.Load("Res/scrap.png"));
	TextBox scrapTextBox;

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
		scrapTextBox = new TextBox(new System.Drawing.Font("Adobe Song Std L", 120f));
		scrapTextBox.UpdateRate = 0;
		scrapTextBox.SetHeight = 1f;
		scrapTextBox.VerticalAlign = TextBox.VerticalAlignment.Center;
		scrapTextBox.Text = scrap.ToString();

		scrapTextBox.Color = SYScrap.HUDColor;
	}

	public override void Die(Vector2 diePos)
	{
		base.Die(diePos);
	}

	public void CollectScrap(SYScrap s)
	{
		s.CollectedBy(this);
	}

	public void ReturnScrap(SYStash stash)
	{
		int scrapReturned = stash.AddScrap(scrap);
	}

	public void ReceiveScrap(int n)
	{
		scrap = n;
		scrapTextBox.Text = n.ToString();
	}

	public override void DrawHUD()
	{
		base.DrawHUD();
	}

	public void DrawScrap()
	{
		scrapSprite.Draw(new Vector2(-5f * Game.AspectRatio + 0.8f, 5f - 0.8f), 1f, 15f);

		Mesh m = scrapTextBox.Mesh;

		m.Reset();

		m.Translate(-5f * Game.AspectRatio + 0.8f, 5f - 0.8f);
		m.Scale(1f);

		m.Draw();
	}
}