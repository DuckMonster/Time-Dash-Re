using OpenTK;
using OpenTK.Input;
using EZUDP;
using TKTools;

public partial class Player
{
	DodgeTarget dodgeTargetBuffer;
	DashTarget dashTargetBuffer;

	public int playerID;
	public string playerName;

	public bool IsLocalPlayer
	{
		get
		{
			return Map.myID == playerID;
		}
	}

	bool receivedServerPosition = false;
	Vector2 serverPosition = Vector2.Zero;

	public void ReceiveInput(byte k)
	{
		inputData.DecodeFlag(k);
	}
	public void ReceiveInput(Vector2 position, Vector2 velocity, byte k)
	{
		if (!IsDodgeing && !IsDashing)
		{
			this.position = position;
			this.velocity = velocity;
		}

		inputData.DecodeFlag(k);
	}

	public void ReceiveJump(Vector2 position)
	{
		if (dashTarget != null)
			DashEnd();
		if (dodgeTarget != null)
			DodgeEnd();

		this.position = position;

		if (IsOnGround) Jump();
		else if (WallTouch != 0) WallJump();
		else if (canDoublejump)
		{
			Jump();
			canDoublejump = false;
		}
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
	}

	public void ReceiveServerPosition(Vector2 position)
	{
		receivedServerPosition = true;
		serverPosition = position;
	}

	public void ReceiveHit(float dmg, Player attacker, float dir, MessageBuffer msg)
	{
		Map.AddEffect(new EffectPlayerHit(this, dir, dmg, Map));

		if ((HitType)msg.ReadByte() == HitType.Bullet)
		{
			int id = msg.ReadByte();
			attacker.RemoveProjectile(id);
		}

		Hit(dmg);
	}

	public void ReceiveDodge(Vector2 start, Direction dir)
	{
		dodgeTargetBuffer = new DodgeTarget(start, dir);
	}

	public void ReceiveDodgeCollision(byte player, Vector2 myPos, Vector2 colPos)
	{
		Player p = Map.playerList[player];

		position = myPos;
		p.position = colPos;

		//DodgeEnd(p);
		//p.DodgeEnd(this);

		Map.AddEffect(new EffectCollision(this, p, Map));
	}

	public void ReceiveDashCollision(byte player, Vector2 myPos, Vector2 colPos)
	{
		Player p = Map.playerList[player];

		position = myPos;
		p.position = colPos;

		DashEnd(p);
		p.DashEnd(this);

		Map.AddEffect(new EffectCollision(this, p, Map));
	}

	public void ReceiveDash(Vector2 start, Vector2 target)
	{
		dashTargetBuffer = new DashTarget(start, target);
	}

	public void ReceiveShoot(Vector2 position, Vector2 target)
	{
		this.position = position;
		Shoot(target);
	}

	public void ReceiveShoot(Vector2 position, Vector2 target, float charge)
	{
		this.position = position;
		weapon.Charge = charge;

		Shoot(target);
	}

	public void ReceiveReload()
	{
		weapon.Reload();
	}

	void SendInput()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInput);
		msg.WriteVector(position);
		msg.WriteVector(velocity);
		msg.WriteByte(inputData.GetFlag());

		Game.client.Send(msg);
	}

	void SendInputPure()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerInputPure);
		msg.WriteByte(inputData.GetFlag());

		Game.client.Send(msg);
	}

	void SendJump()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerJump);
		msg.WriteVector(position);

		Game.client.Send(msg);
	}

	void SendPosition()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerPosition);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		Game.client.Send(msg);
	}

	void SendLand()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerLand);
		msg.WriteVector(position);
		msg.WriteVector(velocity);

		Game.client.Send(msg);
	}

	void SendDodge(DodgeTarget target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDodge);
		msg.WriteVector(target.startPosition);
		msg.WriteByte((byte)target.direction);

		Game.client.Send(msg);
	}

	void SendDash(DashTarget target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerDash);
		msg.WriteVector(target.startPosition);
		msg.WriteVector(target.endPosition);

		Game.client.Send(msg);
	}

	void SendShoot(Vector2 target)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerShoot);

		msg.WriteVector(position);
		msg.WriteVector(target);

		Game.client.Send(msg);
	}

	void SendShoot(Vector2 target, float charge)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerShoot);

		msg.WriteVector(position);
		msg.WriteVector(target);
		msg.WriteFloat(charge);

		Game.client.Send(msg);
	}

	void SendEquipWeapon(int id)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerEquipWeapon);
		msg.WriteByte(id);

		Game.client.Send(msg);
	}

	void SendReload()
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.PlayerReload);

		Game.client.Send(msg);
	}
}