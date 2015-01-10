using OpenTK;
using System.Collections.Generic;
using TKTools;

public class Scoreboard : Entity
{
	public List<Player> leaderList = new List<Player>();

	float width = 10f, height = 6f;

	int[] score = new int[10];
	float[] scoreShow = new float[10];

	TextDrawer[] scoreName = new TextDrawer[10];
	Mesh nameMesh;

	int maxScore = 15;
	Mesh scoreMesh;

	public Scoreboard(int maxScore, float boardWidth, float boardHeight, Vector2 position, Map map)
		: base(position, map)
	{
		width = boardWidth;
		height = boardHeight;

		this.maxScore = maxScore;

		scoreMesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.TriangleStrip);

		scoreMesh.Vertices = new Vector2[] {
			new Vector2(-0.5f, 0f),
			new Vector2(0.5f, 0f),
			new Vector2(-0.5f, 1f),
			new Vector2(0.5f, 1f)
		};

		nameMesh = Mesh.Box;

		nameMesh.Vertices = new Vector2[] {
			new Vector2(-0.5f, 0f),
			new Vector2(0.5f, 0f),
			new Vector2(-0.5f, 1f),
			new Vector2(0.5f, 1f)
		};

		nameMesh.UV = new Vector2[] {
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(0, 0),
			new Vector2(1, 0)
		};
	}

	public virtual bool IsLeader(Player p)
	{
		return leaderList.Contains(p);
	}

	public override void Dispose()
	{
		base.Dispose();
		scoreMesh.Dispose();
		nameMesh.Dispose();
		foreach (TextDrawer t in scoreName)
			if (t != null) t.Dispose();
	}

	public void SetName(int index, string name)
	{
		if (scoreName[index] != null) scoreName[index].Dispose();

		scoreName[index] = new TextDrawer(1000, 1000);
		scoreName[index].Write(name, 0.5f, 1f, 1f, System.Drawing.StringAlignment.Center, System.Drawing.StringAlignment.Far);
		scoreName[index].UpdateTexture();
	}

	public void UpdateLeaders()
	{
		leaderList.Clear();

		int maxScore = 0;

		for (int i = 0; i < map.playerList.Length; i++)
		{
			if (map.playerList[i] == null || score[i] == 0) continue;

			if (score[i] > maxScore)
			{
				leaderList.Clear();
				leaderList.Add(map.playerList[i]);
				maxScore = score[i];
			}
			else if (score[i] == maxScore)
			{
				leaderList.Add(map.playerList[i]);
			}
		}
	}

	public void SetScore(int index, int scr)
	{
		score[index] = scr;
		UpdateLeaders();
	}

	public void ChangeScore(int index, int scr)
	{
		score[index] += scr;
		UpdateLeaders();
	}

	public override void Logic()
	{
		base.Logic();

		for (int i = 0; i < score.Length; i++)
			scoreShow[i] += (score[i] - scoreShow[i]) * 4 * Game.delta;
	}

	public override void Draw()
	{
		int nmbrOfPlayers = map.NumberOfPlayers;
		float w = width / nmbrOfPlayers;

		scoreMesh.Reset();
		scoreMesh.Translate(position);
		scoreMesh.Scale(width, height);
		scoreMesh.Color = new Color(1f, 1f, 1f, 0.2f);
		scoreMesh.Draw();

		int n = 0;

		for (int i = 0; i < map.playerList.Length; i++)
		{
			if (map.playerList[i] == null) continue;

			float x = position.X - width/2 + (w * n) + w / 2;
			scoreMesh.Reset();
			scoreMesh.Translate(x, position.Y);
			scoreMesh.Scale(w, scoreShow[i] * (height / maxScore));

			Color c;

			if (IsLeader(map.playerList[i]))
				c = Player.colorList[i];
			else
				c = Player.colorList[i] * new Color(0.4f, 0.4f, 0.4f, 1f);

			scoreMesh.Color = c;

			scoreMesh.Draw();

			nameMesh.Texture = scoreName[i];

			nameMesh.Reset();
			nameMesh.Translate(x, position.Y + scoreShow[i] * (height / maxScore));
			nameMesh.Scale(w);

			nameMesh.Color = c;
			nameMesh.Draw();

			n++;
		}
	}
}