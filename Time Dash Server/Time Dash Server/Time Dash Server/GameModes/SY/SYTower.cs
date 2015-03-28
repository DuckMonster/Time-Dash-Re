using EZUDP;
using OpenTK;
using System;
using System.Collections.Generic;
using TKTools;

public class SYTower : Actor
{
	static readonly float TurretHeight = 5f;

	public readonly int id;

	SYTowerPoint point;

	Player target;
	float aimDir = 0f;

	Timer playerSearchTimer = new Timer(0.5f, true);
	Timer shootTimer = new Timer(0.5f, false);

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
		set { base.Position = value; }
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
		base.Hit(dmg, dir, proj, force);

		SendHitToPlayer(dmg, dir, proj, Map.playerList);
	}

	public override bool CollidesWith(Vector2 pos, float radius)
	{
		Vector2 tp = TurretPosition;

		Vector2 checkpos = new Vector2(
			MathHelper.Clamp(pos.X, tp.X - Size.X / 2, tp.X + Size.X / 2),
			MathHelper.Clamp(pos.Y, tp.Y - Size.Y / 2, tp.Y + Size.Y / 2));

		return (pos - checkpos).Length <= radius;
	}

	void SetTarget(Player p)
	{
		target = p;
		SendTargetToPlayer(Map.playerList);
	}

	void Shoot()
	{
		if (target == null) return;

		Projectile p = new Bullet(this, TurretPosition, target.Position, 0.5f, Map);

		SendShootToPlayer(target.Position, p, Map.playerList);
		shootTimer.Reset();
	}

	public override void Logic()
	{
		if (target != null)
		{
			Vector2 outPos;

			if ((target.Position - Position).Length > 20f || Map.RayTraceCollision(TurretPosition, target.Position, Vector2.Zero, out outPos))
			{
				SetTarget(null);
			}
			else
			{
				float targetDir = TKMath.GetAngle(TurretPosition, target.Position);

				float dif = Math.Abs(targetDir - aimDir);

				if (dif + 360 < dif)
					targetDir += 360;
				if (Math.Abs(dif - 360) < dif)
					targetDir -= 360;

				aimDir += (targetDir - aimDir) * 4f * Game.delta;
				aimDir = TKMath.Mod(aimDir, -180, 180);

				shootTimer.Logic();

				//if (shootTimer.IsDone)
				//	Shoot();
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
		msg.WriteByte(point.id);

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

		return msg;
	}
}