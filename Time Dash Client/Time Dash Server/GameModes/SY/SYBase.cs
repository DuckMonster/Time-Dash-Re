﻿using OpenTK;
public class SYBase : SYStash
{
	int teamID;

	Team Team
	{
		get { return Map.teamList[teamID]; }
	}

	public SYBase(int id, int teamID, Vector2 position, Map map)
		: base(id, 5f, 1, position, map)
	{
		this.teamID = teamID;
	}

	public override void Finish()
	{
		Map.TeamWin(Team);
	}
}