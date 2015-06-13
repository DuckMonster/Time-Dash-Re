using OpenTK;
using System.Collections.Generic;
using System.Drawing;
using TKTools;

public class SYTowerArea : Entity
{
	protected new SYMap Map
	{
		get { return base.Map as SYMap; }
	}

	SYTower tower;

	public List<Player> BoundPlayers
	{
		get
		{
			List<Player> returnList = new List<Player>();

			foreach (Player p in Map.playerList)
				if (p != null && p.CollidesWith(Position, Size)) returnList.Add(p);

			return returnList;
		}
	}

	public SYTowerArea(Vector2 position, Vector2 size, Map map)
		: base(position, map)
	{
		this.size = size;
	}

	public override void Logic()
	{
		if (tower == null)
			tower = Map.GetActorAtPos<SYTower>(Position, Size);
		else
		{
			if (Map.LocalPlayer != null && Map.LocalPlayer.Team == tower.Team)
				return;

			if (Map.cameraController.secondaryObject == null)
			{
				bool localPlayer = BoundPlayers.Contains(Map.LocalPlayer);
				if (localPlayer)
					Map.cameraController.secondaryObject = tower;
			}
			else if (Map.cameraController.secondaryObject == tower)
			{
				bool localPlayer = BoundPlayers.Contains(Map.LocalPlayer);
				if (!localPlayer)
					Map.cameraController.secondaryObject = null;
			}

			if (!tower.IsAlive)
			{
				if (Map.cameraController.secondaryObject == tower)
					Map.cameraController.secondaryObject = null;

				tower = null;
			}
		}
	}
}