using System;
using System.Collections.Generic;

using OpenTK;
using TKTools;

public class Team
{
	public int id;
	public List<Player> memberList = new List<Player>(10);

	public Color Color
	{
		get
		{
			return Player.colorList[id];
		}
	}

	public int Size
	{
		get
		{
			return memberList.Count;
		}
	}

	public Team(int id)
	{
		this.id = id;
	}

	public void AddMember(Player p)
	{
		if (p.team != null) p.team.RemoveMember(p);
		memberList.Add(p);
		p.team = this;
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
}