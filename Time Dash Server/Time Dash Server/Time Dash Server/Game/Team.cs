using System;
using System.Collections.Generic;

using OpenTK;
using TKTools;
using EZUDP;

public class Team
{
	Map map;

	public int id;
	public List<Actor> memberList = new List<Actor>(10);

	public int Size
	{
		get
		{
			return memberList.Count;
		}
	}

	public Team(int id, Map m)
	{
		map = m;
		this.id = id;
	}

	public void AddMember(Actor a)
	{
		if (a.Team != null) a.Team.RemoveMember(a);
		memberList.Add(a);
		a.Team = this;

		if (a is Player)
			SendPlayerJoinToPlayer(a as Player, map.playerList);
	}

	public void RemoveMember(Actor a)
	{
		memberList.Remove(a);
		a.Team = null;
	}

	public bool IsMember(Actor a)
	{
		return memberList.Contains(a);
	}

	public void SendMemberListToPlayer(params Player[] players)
	{
		foreach (Player p in memberList)
		{
			MessageBuffer msg = new MessageBuffer();

			msg.WriteShort((short)Protocol.PlayerJoinTeam);

			msg.WriteByte(p.id);
			msg.WriteByte(id);

			foreach(Player pp in players) if (pp != null) pp.client.Send(msg);
		}
	}

	public void SendPlayerJoinToPlayer(Player p, params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerJoinTeam);

		msg.WriteByte(p.id);
		msg.WriteByte(id);

		foreach (Player pp in players) if (pp != null) pp.client.Send(msg);
	}
}