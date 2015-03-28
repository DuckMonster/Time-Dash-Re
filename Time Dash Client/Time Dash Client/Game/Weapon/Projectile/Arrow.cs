using OpenTK;
using System.Collections.Generic;
using TKTools;

public class Arrow : Projectile
{
	public class ArrowTrace : Effect
	{
		Mesh mesh;
		Timer effectTimer = new Timer(0.8f, false);

		public ArrowTrace(Vector2 start, Vector2 end, float startsize, float endsize, Map map)
			: base(map)
		{
			mesh = new Mesh(OpenTK.Graphics.OpenGL.PrimitiveType.Quads);
			mesh.Vertices = new Vector2[] {
				new Vector2(0, 0.5f * startsize),
				new Vector2(1, 0.5f * endsize),
				new Vector2(1, -0.5f * endsize),
				new Vector2(0, -0.5f * startsize)
			};

			mesh.UV = mesh.Vertices;

			mesh.Reset();

			mesh.Translate(start);
			mesh.Rotate(TKMath.GetAngle(start, end));
			mesh.Scale((end - start).Length, 0.1f);
		}

		public override void Logic()
		{
			if (effectTimer.IsDone)
				Remove();

			effectTimer.Logic();
		}

		public override void Draw()
		{
			mesh.Color = new Color(1, 1, 1, 1f - effectTimer.PercentageDone);
			mesh.Draw();
		}
	}

	float charge;
	float sizeOffset = 0f;

	public Arrow(Actor owner, int id, Vector2 position, Vector2 target, float charge, Map map)
		: base(owner, id, position, map)
	{
		this.charge = charge;

		size = new Vector2(0.2f, 0.2f);
		velocity = (target - position).Normalized() * (10 + 120f * charge);
	}

	public override void Logic()
	{
		if (!Active) return;

		float startsize = (0.5f + 0.5f * charge) * sizeOffset;

		if (sizeOffset < 1f)
		{
			sizeOffset += 40f * charge * Game.delta;
			if (sizeOffset > 1f) sizeOffset = 1f;
		}

		float endsize = (0.5f + 0.5f * charge) * sizeOffset;

		velocity.Y -= Stats.defaultStats.Gravity * 1f * Game.delta;

		Vector2 stepVector = velocity * Game.delta;

		Vector2 collidePos;
		bool coll = Map.RayTraceCollision(position, position + stepVector, size, out collidePos);

		if (!coll)
		{
			List<Actor> p = Map.RayTraceActor<Actor>(position, position + stepVector, size, owner);
			if (p.Count > 0) collidePos = position;
		}

		Map.AddEffect(new ArrowTrace(position, collidePos, startsize, endsize, Map));

		position = collidePos;

		if (coll)
			Hit();

		//base.Logic();
	}

	public override void Hit()
	{
		EffectCone.CreateSmokeCone(position, TKMath.GetAngle(velocity), 0.4f + 0.6f * charge, 0.8f, 3, 1, Map);
		Map.AddEffect(new EffectRing(position, 1.6f + charge * 1.5f, 0.5f + 0.5f * charge, Color.White, Map));

		base.Hit();
	}

	public override void Draw()
	{
		if (!Active) return;
	}
}