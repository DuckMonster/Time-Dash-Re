using OpenTK;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class SYTower : Actor
{
	static readonly float TurretHeight = 5f;

	new SYMap Map
	{
		get { return base.Map as SYMap; }
	}

	public readonly int id;

	SYTowerPoint point;

	float aimDir = 0f;
	Player target;

	Sprite baseSprite = new Sprite(Art.Load("Res/cannon_base.png"));
	Sprite headSprite = new Sprite(Art.Load("Res/cannon_head.png"));
	Mesh barrelMesh = Mesh.CreateFromTexture(Art.Load("Res/cannon_brl.png"));
	float barrelPosition = 0f;

	Mesh ammoMesh = Mesh.CreateFromPrimitive(MeshPrimitive.Quad);

	Timer reloadTimer = new Timer(1.2f, false);
	int ammo = 0;

	public Vector2 TurretPosition
	{
		get
		{
			return position + new Vector2(0, TurretHeight);
		}
	}

	float TurnSpeed { get { return VariousStats.towerStats.GetStat<float>("turnSpeed"); } }
	float FireSpeed { get { return VariousStats.towerStats.GetStat<float>("fireSpeed"); } }
	float MaxAmmo { get { return VariousStats.towerStats.GetStat<float>("ammo"); } }
	public override float MaxHealth { get { return VariousStats.towerStats.GetStat<float>("health"); } }

	public override Vector2 Position
	{
		get { return TurretPosition; }
	}

	public SYTower(int id, int teamID, Vector2 position, Map map)
		: this(id, teamID, null, position, map)
	{ }
	public SYTower(int id, int teamID, SYTowerPoint point, Vector2 position, Map map)
		: base(position, map)
	{
		this.id = id;
		this.point = point;
		this.teamID = teamID;

		if (point != null)
			point.tower = this;

		baseSprite.Mesh.Vertices2 = new Vector2[] {
			new Vector2(-1f, 0f),
			new Vector2(-1f, TurretHeight),
			new Vector2(1f, TurretHeight),
			new Vector2(1f, 0f)
		};

		baseSprite.Position = new Vector3(position);
		headSprite.Position = new Vector3(TurretPosition);
		headSprite.ScaleF = 3f;
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

		reloadTimer.Reset();
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
		if (point != null) point.Reset();

		Map.TowerDestroyed(this);

		health = 0;
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

			aimDir += dif * TurnSpeed * Game.delta;
			aimDir = TKMath.Mod(aimDir, -180, 180);

			if (ammo <= 0)
			{
				reloadTimer.Logic();
				if (reloadTimer.IsDone)
					ammo = 5;
			}
		}
		else
		{
			aimDir += 25f * Game.delta;
		}

		barrelPosition -= barrelPosition * 5f * Game.delta;

		headSprite.Rotation = aimDir;
	}

	public override void Draw()
	{
		barrelMesh.FillColor = false;
		barrelMesh.Color = Color.White;

		barrelMesh.Reset();

		barrelMesh.Translate(TurretPosition);
		barrelMesh.Scale(2.1f);
		barrelMesh.RotateZ(aimDir);
		barrelMesh.Translate(1f + 0.5f * barrelPosition, 0f);

		barrelMesh.Draw();

		if (reloadTimer.PercentageDone > 0f)
		{
			barrelMesh.FillColor = true;
			barrelMesh.Color = new Color(1, 1, 1, reloadTimer.PercentageDone);
			barrelMesh.Draw();
		}

		baseSprite.Draw();
		headSprite.Draw();
	}
}