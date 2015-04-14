using OpenTK;
using System;
using TKTools;

public class SYPlayer : Player
{
	Mesh scrapMesh = Mesh.Box;
	TextBox scrapTextBox;
	Mesh scrapTextMesh;

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
		scrap = 20;

		scrapMesh = Mesh.OrthoBox;
		scrapMesh.Texture = Art.Load("Res/scrap.png");

		scrapTextMesh = Mesh.OrthoBox;

		scrapTextBox = new TextBox(new System.Drawing.Font("Adobe Song Std L", 120f));
		scrapTextBox.UpdateRate = 0;
		scrapTextBox.SetHeight = 1f;
		scrapTextBox.VerticalAlign = TextBox.VerticalAlignment.Center;
		scrapTextBox.Text = scrap.ToString();

		scrapTextMesh.Vertices = scrapTextBox.Vertices;
		scrapTextMesh.UV = scrapTextBox.UV;
		scrapTextMesh.Texture = scrapTextBox.Texture;
		scrapTextMesh.Color = Color.Yellow;
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

		scrapTextMesh.Texture = scrapTextBox.Texture;
		scrapTextMesh.Vertices = scrapTextBox.Vertices;
	}

	public override void DrawHUD()
	{
		base.DrawHUD();
	}

	public void DrawScrap()
	{
		scrapMesh.Reset();

		scrapMesh.Translate(-9f, 10f * Game.windowRatio - 1f);
		scrapMesh.Scale(2f);
		scrapMesh.Rotate(15f);

		scrapMesh.Draw();

		scrapTextMesh.Reset();

		scrapTextMesh.Translate(-9f, 10f * Game.windowRatio - 1f);
		scrapTextMesh.Scale(2f);

		scrapTextMesh.Draw();
	}
}