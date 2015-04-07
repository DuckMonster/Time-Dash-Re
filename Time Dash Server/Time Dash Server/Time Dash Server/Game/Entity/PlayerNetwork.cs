using EZUDP;
using OpenTK;

public partial class Player : Actor
{
	public static bool SEND_SERVER_POSITION = false;

	DodgeTarget dodgeTargetBuffer;
	DashTarget dashTargetBuffer;

	public void ReceiveInput(byte k)
	{
		inputData.DecodeFlag(k);
		SendInputPureToPlayer(Map.playerList);
	}
	public void ReceiveInput(Vector2 position, Vector2 velocity, byte k)
	{
		if (!IsDodging && !IsDashing)
			SetPosition(position, velocity);

		inputData.DecodeFlag(k);
		SendInputToPlayer(Map.playerList);
	}

	public void SetPosition(Vector2 position, Vector2 velocity)
	{
		Vector2 oldPos = position;

		if ((position - oldPos).Length > 0.4f)
			SendPositionToPlayerForce(this);
		else
		{
			this.Position = position;
			this.velocity = velocity;
		}
	}

	public void ReceiveJump(Vector2 position)
	{
		if (dashTarget != null)
			DashEnd();
		if (dodgeTarget != null)
			DodgeEnd();

		this.Position = position;

		if (IsOnGround) Jump();
		else if (WallTouch != 0) WallJump();
		else if (canDoublejump)
		{
			Jump();
			canDoublejump = false;
		}

		SendJumpToPlayer(Map.playerList);
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		SetPosition(position, velocity);

		if (dodgeTarget != null) dodgeTarget = null;
		SendPositionToPlayer(Map.playerList);
	}

	public void ReceiveLand(Vector2 position, Vector2 velocity)
	{
		SetPosition(position, velocity);
		Land();
	}

	public void ReceiveDodge(Vector2 start, Direction direction)
	{
		dodgeTargetBuffer = new DodgeTarget(start, direction);

		SendDodgeToPlayer(start, direction, Map.playerList);
	}

	public void ReceiveDash(Vector2 start, Vector2 target)
	{
		dashTargetBuffer = new DashTarget(start, target);

		SendDashToPlayer(start, target, Map.playerList);
	}

	public void ReceiveShoot(Vector2 position, Vector2 target)
	{
		this.Position = position;

		Shoot(target);
	}

	public void ReceiveShoot(Vector2 position, Vector2 target, float charge)
	{
		this.Position = position;
		Weapon.Charge = charge;

		Shoot(target);
	}

	public void ReceiveSwapWeapon()
	{
		int origin = weaponIndex;
		do
		{
			weaponIndex = (weaponIndex + 1) % inventory.Length;
			if (weaponIndex == origin) break;
		} while (inventory[weaponIndex] == null);

        SendEquipWeaponToPlayer(weaponIndex, Map.playerList);
	}

	public void ReceiveReload()
	{
		Weapon.Reload();
		SendReloadToPlayer(Map.playerList);
	}

	public void SendExistanceToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetExistanceMessage(), false, players);
		SendPositionToPlayer(players);

		for (int i = 0; i < inventory.Length; i++)
			if (inventory[i] != null)
				SendInventoryToPlayer(i, inventory[i].type, players);

		if (Weapon != null)
			SendEquipWeaponToPlayer(weaponIndex, players);
	}

	public void SendLeaveToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetLeaveMessage(), false, players);
	}

	public void SendInputToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetInputMessage(), true, players);
	}

	public void SendInputPureToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetInputPureMessage(), true, players);
	}

	public void SendJumpToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetJumpMessage(), true, players);
	}

	public void SendPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), true, players);
	}

	public void SendPositionToPlayerForce(params Player[] players)
	{
		SendMessageToPlayer(GetPositionMessage(), false, players);
	}

	public void SendHitToPlayer(float dmg, float dir, params Player[] players)
	{
		SendMessageToPlayer(GetHitMessage(dmg, dir), false, players);
	}

	public void SendHitToPlayer(float dmg, float dir, Projectile proj, params Player[] players)
	{
		SendMessageToPlayer(GetHitMessage(dmg, dir, proj), false, players);
	}

	public void SendKillToPlayer(Player target, params Player[] players)
	{
		SendMessageToPlayer(GetKillMessage(target), false, players);
	}

	public void SendDieToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetDieMessage(), false, players);
	}

	public void SendRespawnToPlayer(Vector2 pos, params Player[] players)
	{
		SendMessageToPlayer(GetRespawnMessage(pos), false, players);
	}

	public void SendDisableToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetDisableMessage(), false, players);
	}

	public void SendDodgeToPlayer(Vector2 start, Direction dir, params Player[] players)
	{
		SendMessageToPlayer(GetDodgeMessage(start, dir), true, players);
	}

	public void SendDashToPlayer(Vector2 start, Vector2 target, params Player[] players)
	{
		SendMessageToPlayer(GetDashMessage(start, target), true, players);
	}

	public void SendShootToPlayer(Vector2 position, Vector2 hitpos, int projID, params Player[] players)
	{
		SendMessageToPlayer(GetShootMessage(position, hitpos, projID), false, players);
	}

	public void SendShootToPlayer(Vector2 position, Vector2 hitpos, int projID, float charge, params Player[] players)
	{
		SendMessageToPlayer(GetShootMessage(position, hitpos, projID, charge), false, players);
	}

	public void SendEquipWeaponToPlayer(int inventoryID, params Player[] players)
	{
		SendMessageToPlayer(GetEquipWeaponMessage(inventoryID), false, players);
	}

	public void SendInventoryToPlayer(int inventoryID, WeaponList weapon, params Player[] players)
	{
		SendMessageToPlayer(GetInventoryMessage(inventoryID, weapon), false, players);
	}

	public void SendReloadToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetReloadMessage(), true, players);
	}

	public void SendDodgeCollisionToPlayer(Player p, params Player[] players)
	{
		SendMessageToPlayer(GetDodgeCollisionMessage(p), false, players);
	}

	public void SendDashCollisionToPlayer(Player p, params Player[] players)
	{
		SendMessageToPlayer(GetDashCollisionMessage(p), false, players);
	}

	public void SendServerPositionToPlayer(params Player[] players)
	{
		SendMessageToPlayer(GetServerPositionMessage(), false, players);
	}

	protected void SendMessageToPlayer(MessageBuffer msg, bool excludeSelf, params Player[] players)
	{
		foreach (Player p in players) if (p != null && !(excludeSelf && p == this)) p.client.Send(msg);
	}

	MessageBuffer GetExistanceMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerJoin);
		msg.WriteByte(id);
		msg.WriteString(playerName);

		return msg;
	}

	MessageBuffer GetLeaveMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerLeave);
		msg.WriteByte(id);

		return msg;
	}

	MessageBuffer GetInputMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInput);
		msg.WriteByte(id);
		msg.WriteVector(Position);
		msg.WriteVector(velocity);
		msg.WriteByte(inputData.GetFlag());

		return msg;
	}

	MessageBuffer GetInputPureMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInputPure);
		msg.WriteByte(id);
		msg.WriteByte(inputData.GetFlag());

		return msg;
	}

	MessageBuffer GetJumpMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerJump);
		msg.WriteByte(id);
		msg.WriteVector(Position);

		return msg;
	}

	MessageBuffer GetPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
		msg.WriteByte(id);
		msg.WriteVector(Position);
		msg.WriteVector(velocity);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg, float dir)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerHit);
		msg.WriteByte(id);

		msg.WriteFloat(dmg);
		msg.WriteFloat(dir);

		msg.WriteByte((byte)HitType.Dash);

		return msg;
	}

	MessageBuffer GetHitMessage(float dmg, float dir, Projectile proj)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerHit);
		msg.WriteByte(id);

		msg.WriteFloat(dmg);
		msg.WriteFloat(dir);

		msg.WriteByte((byte)HitType.Projectile);
		msg.WriteByte(proj.id);
		msg.WriteVector(proj.Position);

		return msg;
	}

	MessageBuffer GetKillMessage(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerKill);
		msg.WriteByte(id);
		msg.WriteByte(p.id);

		return msg;
	}

	MessageBuffer GetDieMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDie);
		msg.WriteByte(id);
		msg.WriteVector(Position);

		return msg;
	}

	MessageBuffer GetRespawnMessage(Vector2 pos)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerRespawn);
		msg.WriteByte(id);
		msg.WriteVector(pos);

		return msg;
	}

	MessageBuffer GetDisableMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDisable);
		msg.WriteByte(id);

		return msg;
	}

	MessageBuffer GetDodgeMessage(Vector2 start, Direction dir)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDodge);
		msg.WriteByte(id);

		msg.WriteVector(start);
		msg.WriteByte((byte)dir);

		return msg;
	}

	MessageBuffer GetDashMessage(Vector2 start, Vector2 target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDash);
		msg.WriteByte(id);

		msg.WriteVector(start);
		msg.WriteVector(target);

		return msg;
	}

	MessageBuffer GetShootMessage(Vector2 position, Vector2 hitPosition, int projID)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerShoot);
		msg.WriteByte(id);

		msg.WriteVector(position);
		msg.WriteVector(hitPosition);

		msg.WriteByte(projID);

		return msg;
	}

	MessageBuffer GetShootMessage(Vector2 position, Vector2 hitPosition, int projID, float charge)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerShoot);
		msg.WriteByte(id);

		msg.WriteVector(position);
		msg.WriteVector(hitPosition);

		msg.WriteByte(projID);
		msg.WriteFloat(charge);

		return msg;
	}

	MessageBuffer GetEquipWeaponMessage(int inventoryID)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerEquipWeapon);
		msg.WriteByte(id);
		msg.WriteByte(inventoryID);

		return msg;
	}

	MessageBuffer GetInventoryMessage(int inventoryID, WeaponList weapon)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInventory);
		msg.WriteByte(id);

		msg.WriteByte(inventoryID);
		msg.WriteByte((byte)weapon);

		return msg;
	}

	MessageBuffer GetReloadMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerReload);
		msg.WriteByte(id);

		return msg;
	}

	MessageBuffer GetDodgeCollisionMessage(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDodgeCollision);
		msg.WriteByte(id);
		msg.WriteByte(p.id);
		msg.WriteVector(Position);
		msg.WriteVector(p.Position);

		return msg;
	}

	MessageBuffer GetDashCollisionMessage(Player p)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDashCollision);
		msg.WriteByte(id);
		msg.WriteByte(p.id);
		msg.WriteVector(Position);
		msg.WriteVector(p.Position);

		return msg;
	}

	MessageBuffer GetServerPositionMessage()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.ServerPosition);
		msg.WriteByte(id);

		msg.WriteVector(Position);

		return msg;
	}
}