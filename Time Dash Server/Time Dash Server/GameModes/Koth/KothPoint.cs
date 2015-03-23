using EZUDP;
using OpenTK;
using System.Collections.Generic;
using TKTools;

public class KothPoint : Entity
{
	Player owner = null;
	float radius = 7f;

	int[] score = new int[10];
	int scoreGoal = 40;

	Timer scoreTimer = new Timer(0.7f, false);

	public KothPoint(Vector2 position, Map map)
		: base(position, map)
	{
	}

	void LookForPlayer()
	{
		List<Player> playerList = Map.GetActorRadius<Player>(position, radius);

		Player contender = null;
		foreach (Player p in playerList)
		{
			if (p.position.Y < position.Y) continue;

			if (contender == null) contender = p;
			else
			{
				owner = null;
				return;
			}
		}

		if (contender != null) owner = contender;
		else owner = null;
	}

	public void ChangeScore(int id, int scr)
	{
		score[id] = MathHelper.Clamp(score[id] + scr, 0, scoreGoal);
		SendScoreToPlayer(id, Map.playerList);

		if (score[id] >= scoreGoal) Map.PlayerWin(Map.playerList[id]);
	}

	public override void Logic()
	{
		base.Logic();
		LookForPlayer();

		if (owner != null)
		{
			scoreTimer.Logic();

			if (scoreTimer.IsDone)
			{
				ChangeScore(owner.id, 1);
				scoreTimer.Reset();
			}
		}
	}

	public void SendScoreboardToPlayer(params Player[] players)
	{
		for (int i = 0; i < score.Length; i++)
			SendScoreToPlayer(i, players);
	}

	void SendScoreToPlayer(int index, params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_KOTH.PlayerScore);

		msg.WriteByte(index);
		msg.WriteShort(score[index]);

		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}
}