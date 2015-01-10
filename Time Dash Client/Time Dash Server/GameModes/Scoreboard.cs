using EZUDP;
using OpenTK;
using System.Collections.Generic;
using TKTools;

public class Scoreboard : Entity
{
	public List<Player> leaderList = new List<Player>();

	int[] score = new int[10];

	int maxScore = 15;

	public Scoreboard(int maxScore, Vector2 position, Map map)
		: base(position, map)
	{
		this.maxScore = maxScore;
	}

	public bool IsLeader(Player p)
	{
		return leaderList.Contains(p);
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
		SendScoreToPlayer(index, map.playerList);

		if (score[index] >= maxScore)
			map.PlayerWin(map.playerList[index]);
	}

	public void ChangeScore(int index, int scr)
	{
		score[index] += scr;
		UpdateLeaders();
		SendScoreToPlayer(index, map.playerList);

		if (score[index] >= maxScore)
			map.PlayerWin(map.playerList[index]);
	}

	public override void Logic()
	{
		base.Logic();
	}

	public void SendScoreboardToPlayer(params Player[] players)
	{
		for (int i = 0; i < score.Length; i++)
			SendScoreToPlayer(i, players);
	}

	public void SendScoreToPlayer(int index, params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_DM.PlayerScore);

		msg.WriteByte(index);
		msg.WriteShort(score[index]);

		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}
}