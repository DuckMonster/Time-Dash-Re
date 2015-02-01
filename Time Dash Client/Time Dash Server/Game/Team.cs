using System;
using System.Collections.Generic;

using OpenTK;
using TKTools;
using EZUDP;

public class Team
{
	Map map;

	public int id;
	public List<Player> memberList = new List<Player>(10);

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

	public void AddMember(Player p)
	{
		if (p.team != null) p.team.RemoveMember(p);
		memberList.Add(p);
		p.team = this;

		SendPlayerJoinToPlayer(p, map.playerList);
	}

	public void RemoveMember(Player p)
	{
		memberList.Remove(p);
		p.team = null;
	}

	public bool IsMember(Player p)
	{
		return memberList.Contains(p);
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