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

	public override Vector2 Position
	{
		get { return TurretPosition; }
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

	public void ReceiveShoot(Vector2 target, int projID)
	{
		new Bullet(this, projID, new Vector2(1f, 0.8f), TurretPosition, target, Map);
	}

	public void ReceiveHit(float damage, float direction, int projID)
	{
		Hit(damage);

		if (Map.projectileList[projID] != null)
			Map.projectileList[projID].OnHit(this);

		Map.AddEffect(new EffectRing(TurretPosition, 6f, 1.2f, Color.White, Map));
	}

	public override void Logic()
	{
		if (target != null)
		{
			float targetDir = TKMath.GetAngle(TurretPosition, target.Position);

			float dif = targetDir - aimDir;

			while (dif > 180)
				dif -= 360;
			while (dif < -180)
				dif += 360;

			aimDir += dif * 4f * Game.delta;
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