using EZUDP;
using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;

public class SYTower : Actor
{
	static readonly float TurretHeight = 5f;

	new SYMap Map
	{
		get { return base.Map as SYMap; }
	}

	public readonly int id;

	SYTowerPoint point;

	Player target;
	float aimDir = 0f;

	Timer playerSearchTimer = new Timer(0.5f, true);

	Timer reloadTimer = new Timer(1.2f, false);
	Timer shootTimer = new Timer(0.2f, false);

	int ammo = 0;

	public Vector2 TurretPosition
	{
		get
		{
			return position + new Vector2(0, TurretHeight);
		}
	}

	public override float MaxHealth
	{
		get
		{
			return 20f;
		}
	}

	public override Vector2 Position
	{
		get { return TurretPosition; }
		set { base.Position = value; }
	}

	public SYTower(int id, Vector2 position, Map map)
		:this(id, null, position, map)
	{
	}
	public SYTower(int id, SYTowerPoint point, Vector2 position, Map map)
		:base(position, map)
	{
		this.point = point;
		this.id = id;

		SendExistanceToPlayer(map.playerList);

		Size = new Vector2(2.5f, 2.5f);
	}

	public override void Hit(float dmg, float dir, Projectile proj, float force = 0)
	{
		SendHitToPlayer(dmg, dir, proj, Map.playerList);
		base.Hit(dmg, dir, proj, force);
	}

	public override void Die()
	{
		base.Die();
		SendDieToPlayer(Map.playerList);

		if (point != null) point.Reset();
		Map.TowerDestroyed(this);
	}

	public override bool CollidesWith(Vector2 pos, float radius)
	{
		Vector2 tp = TurretPosition;
		return (pos - tp).Length <= (radius + size.X/2);
	}

	void SetTarget(Player p)
	{
		target = p;
		SendTargetToPlayer(Map.playerList);
	}

	void Shoot()
	{
		if (target == null) return;

		Vector2 dirVec = TKMath.GetAngleVector(aimDir);
		Projectile p = new TowerBullet(this, TurretPosition + dirVec * 1.2f, TurretPosition + dirVec * 5f, 2f, Map);

		SendShootToPlayer(TurretPosition + dirVec * 5f, p, Map.playerList);
		ammo--;
	}

	public override void Logic()
	{
		if (target != null)
		{
			Vector2 outPos;

			if (!target.IsAlive || (target.Position - Position).Length > 20f || Map.RayTraceCollision(TurretPosition, target.Position, Vector2.Zero, out outPos))
			{
				SetTarget(null);
			}
			else
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
				} else
				{
					shootTimer.Logic();

					if (shootTimer.IsDone)
					{
						Shoot();
						shootTimer.Reset();
					}
				}
			}
		} else
		{
			aimDir += 25f * Game.delta;

			playerSearchTimer.Logic();

			if (playerSearchTimer.IsDone)
			{
				List<Player> playerList = Map.GetActorRadius<Player>(TurretPosition, 20f);
				foreach(Player p in playerList)
				{
					Vector2 outPos;

					bool coll = Map.RayTraceCollision(TurretPosition, p.Position, Vector2.Zero, out outPos);
					if (!coll)
					{
						SetTarget(p);
						break;
					}
				}

				playerSearchTimer.Reset();
			}
		}
	}

	public void SendExistanceToPlayer(params Player[] player)
	{
		SendMessageToPlayer(GetExistanceMessage(), player);
	}

	public void SendRotationToPlayer(params Player[] player)
	{
		SendMessageToPlayer(GetRotationMessage(), player);
	}

	public void SendTargetToPlayer(params Player[] player)
	{
		SendMessageToPlayer(GetTargetMessage(), player);
	}

	public void SendShootToPlayer(Vector2 target, Projectile p, params Player[] player)
	{
		SendMessageToPlayer(GetShootMessage(target, p), player);
	}

	public void SendHitToPlayer(float dmg, float dir, Projectile p, params Player[] player)
	{
		SendMessageToPlayer(GetHitMessage(dmg, dir, p), player);
	}

	public void SendDieToPlayer(params Player[] player)
	{
		SendMessageToPlayer(GetDieMessage(), player);
	}

	void SendMessageToPlayer(MessageBuffer msg, params Player[] player)
	{
		foreach (Player p in player) if (p != null) p.client.Send(msg);
	}

	MessageBuffer GetExistanceMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.TowerExistance);

		msg.WriteByte(id);
		if (point != null)
			msg.WriteByte(point.id);
		else
			msg.WriteVector(position);

		return msg;
	}

	MessageBuffer GetRotationMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.TowerRotation);

		msg.WriteByte(id);
		msg.WriteFloat(aimDir);

		return msg;
	}

	MessageBuffer GetTargetMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.TowerTarget);

		msg.WriteByte(id);
		msg.WriteByte(target == null ? -1 : target.id);

		return msg;
	}

	MessageBuffer GetShootMessage(Vector2 target, Projectile p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.TowerShoot);

		msg.WriteByte(id);
		msg.WriteVector(target);
		msg.WriteByte(p.id);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg, float dir, Projectile p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.TowerHit);

		msg.WriteByte(id);

		msg.WriteFloat(dmg);
		msg.WriteFloat(dir);

		msg.WriteByte(p.id);
		msg.WriteVector(p.Position);

		return msg;
	}

	MessageBuffer GetDieMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_SY.TowerDie);

		msg.WriteByte(id);

		return msg;
	}
}