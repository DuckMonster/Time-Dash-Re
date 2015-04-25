using EZUDP;
using OpenTK;
using System.Collections.Generic;

public class CPPoint : Entity
{
	public int id;

	Team owner;
	Team currentContender;

	float progress = 0f;

	float radius = 4f;

	public CPPoint(int id, Vector2 position, Map m)
		: base(position, m)
	{
		this.id = id;
	}

	public void SetOwner(Team t)
	{
		owner = t;
		progress = t == null ? 0f : 1f;

		SendOwnerToPlayer(Map.playerList);
	}

	public List<Team> GetContenders()
	{
		List<Team> returnList = new List<Team>(5);
		List<Player> playerList = Map.GetActorRadius<Player>(Position, radius);

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
					SetOwner(contenders[0]);
			}
			else if (owner != contenders[0])
			{
				progress -= 0.4f * Game.delta;
				if (progress <= 0f)
					SetOwner(null);
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

	public void SendOwnerToPlayer(params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CP.TeamOwner);

		msg.WriteByte(id);
		msg.WriteByte(owner == null ? 100 : owner.id);

		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}

	public void SendProgressToPlayer(params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CP.TeamProgress);

		msg.WriteByte(id);
		msg.WriteFloat(progress);

		foreach (Player p in players) if (p != null) p.client.Send(msg);
	}
}