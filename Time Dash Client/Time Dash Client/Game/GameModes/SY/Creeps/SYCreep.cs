using EZUDP;
using OpenTK;
using TKTools;

public class SYCreep : Actor
{
	protected new SYMap Map
	{
		get
		{
			return (SYMap)base.Map;
		}
	}

	public int id;

	public SYCreep(int id, Vector2 position, Vector2 velocity, Map map)
		:base(position, map)
	{
		this.id = id;
		this.velocity = velocity;
	}

	public void ReceivePosition(Vector2 position, Vector2 velocity)
	{
		this.position = position;
		this.velocity = velocity;
	}

	public void ReceiveHit(float damage, float dir, int actorID, MessageBuffer msg)
	{
		Map.AddEffect(new EffectEnemyHit(this, dir, damage, Map));

		HitType type = (HitType)msg.ReadByte();

		switch(type)
		{
			case HitType.Bullet:
				int id = msg.ReadByte();
				Map.playerList[actorID].ProjectileHit(this, id);
				break;

			case HitType.NPC:
				break;
		}

		Hit(damage);
	}

	public override void Logic()
	{
		//base.Logic();
	}

	public override void Draw()
	{
		if (!IsAlive) return;

		mesh.Color = Color.Red;
		mesh.Reset();

		mesh.Translate(position);
		mesh.Scale(0.8f, 1f);

		mesh.Draw();
	}
}