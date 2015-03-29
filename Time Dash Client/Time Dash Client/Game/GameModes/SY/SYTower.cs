using OpenTK;
using System;
using TKTools;

public class SYTower : Actor
{
	static readonly float TurretHeight = 5f;
	public static Texture 
		texBase, 
		texHead,
		texBarrel;

	new SYMap Map
	{
		get { return base.Map as SYMap; }
	}

	public readonly int id;

	SYTowerPoint point;

	float aimDir = 0f;
	Player target;

	Mesh baseMesh = Mesh.Box;
	Mesh barrelMesh = Mesh.Box;
	float barrelPosition = 0f;

	Timer reloadTimer = new Timer(1.2f, false);
	int ammo = 0;

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
		if (texBase == null)
		{
			texBase = new Texture("Res/cannon_base.png");
			texHead = new Texture("Res/cannon_head.png");
			texBarrel = new Texture("Res/cannon_brl.png");
		}

		this.id = id;
		this.point = point;

		point.tower = this;

		mesh.Texture = texHead;
		barrelMesh.Texture = texBarrel;
		baseMesh.Texture = texBase;
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
		Vector2 dirVec = (target - TurretPosition).Normalized();

		new TowerBullet(this, projID, TurretPosition + dirVec * 1.2f, target, Map);
		barrelPosition = -1;

		ammo--;
	}

	public void ReceiveHit(float damage, float direction, int projID, Vector2 hitpos)
	{
		Hit(damage);

		if (Map.projectileList[projID] != null)
			Map.projectileList[projID].OnHit(this, hitpos);

		Map.AddEffect(new EffectTowerHit(this, aimDir, damage, Map));
		Map.AddEffect(new EffectImpactQuad(hitpos, damage, Map));
	}

	public void ReceiveDie()
	{
		EffectExplosion.CreateExplosion(Position, 1.8f, Map);
		point.Reset();

		Map.TowerDestroyed(this);
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

			aimDir += dif * 8f * Game.delta;
			aimDir = TKMath.Mod(aimDir, -180, 180);

			if (ammo <= 0)
			{
				reloadTimer.Logic();
				if (reloadTimer.IsDone)
				{
					ammo = 5;
					reloadTimer.Reset();
				}
			}
		}
		else
		{
			aimDir += 25f * Game.delta;
		}

		barrelPosition -= barrelPosition * 5f * Game.delta;
	}

	public override void Draw()
	{
		baseMesh.Reset();
		baseMesh.Translate(Position);

		baseMesh.Scale(2f, TurretHeight);
		baseMesh.Translate(0, -0.5f);

		baseMesh.Draw();

		barrelMesh.FillColor = false;
		barrelMesh.Color = Color.White;

		barrelMesh.Reset();

		barrelMesh.Translate(TurretPosition);
		barrelMesh.Scale(2.1f);
		barrelMesh.Rotate(aimDir);
		barrelMesh.Translate(1f + 0.5f * barrelPosition, 0f);

		barrelMesh.Draw();

		if (reloadTimer.PercentageDone > 0f)
		{
			barrelMesh.FillColor = true;
			barrelMesh.Color = new Color(1, 1, 1, reloadTimer.PercentageDone);
			barrelMesh.Draw();
		}

		mesh.FillColor = false;
		mesh.Color = new Color(1f, 1f, 1f);

		mesh.Reset();

		mesh.Translate(TurretPosition);
		mesh.Scale(3f);
		mesh.Rotate(aimDir);

		mesh.Draw();
	}
}