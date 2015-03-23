using OpenTK;
using System;
using TKTools;

public class SYTower : Actor
{
	static readonly float TurretHeight = 5f;

	public readonly int id;

	SYTowerPoint point;

	float aimDir = 0f;
	Player target;

	public Vector2 TurretPosition
	{
		get
		{
			return position + new Vector2(0, TurretHeight);
		}
	}

	public SYTower(int id, SYTowerPoint point, Vector2 position, Map map)
		:base(position, map)
	{
		this.id = id;
		this.point = point;

		point.tower = this;
	}

	public void ReceiveRotation(float dir)
	{
		aimDir = dir;
	}

	public void ReceiveTarget(int playerID)
	{
		if (playerID == 255)
			target = null;
		else
			target = Map.playerList[playerID];
	}

	public override void Logic()
	{
		if (target != null)
		{
			float targetDir = TKMath.GetAngle(TurretPosition, target.position);

			float dif = Math.Abs(targetDir - aimDir);

			if (dif + 360 < dif)
				targetDir += 360;
			if (Math.Abs(dif - 360) < dif)
				targetDir -= 360;

			Log.Debug(aimDir + " | " + targetDir);

			aimDir += (targetDir - aimDir) * 4f * Game.delta;
			aimDir = TKMath.Mod(aimDir, -180, 180);
		}
		else
		{
			aimDir += 25f * Game.delta;
		}
	}

	public override void Draw()
	{
		mesh.Reset();

		mesh.Translate(TurretPosition);
		mesh.Scale(3f);
		mesh.Rotate(aimDir);

		mesh.Draw();
	}
}