using EZUDP;
using EZUDP.Server;
using OpenTK;

public class CTFPlayer : Player
{
	CTFFlag holdingFlag;

	public bool HoldingFlag
	{
		get
		{
			return holdingFlag != null;
		}
	}

	public CTFPlayer(int id, string name, Client client, Vector2 position, Map m)
		: base(id, name, client, position, m)
	{
	}

	public override void Hit(float dmg)
	{
		base.Hit(dmg);
	}

	public override void Die()
	{
		if (holdingFlag != null) holdingFlag.Drop();
		base.Die();
	}

	public void GrabFlag(CTFFlag flag)
	{
		holdingFlag = flag;
		flag.holder = this;

		SendFlagGrabToPlayer(Map.playerList);
	}

	public void DropFlag()
	{
		if (holdingFlag != null)
			holdingFlag.Drop();
	}

	public void SendFlagGrabToPlayer(params Player[] players)
	{
		MessageBuffer msg = new MessageBuffer();

		msg.WriteShort((short)Protocol.MapArgument);
		msg.WriteShort((short)Protocol_CTF.FlagGrabbed);
		msg.WriteByte(id);

		SendMessageToPlayer(msg, false, players);
	}
}