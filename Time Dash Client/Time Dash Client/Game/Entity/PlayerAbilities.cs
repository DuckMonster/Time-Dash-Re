using System;
using System.Collections.Generic;

using OpenTK;
using TKTools;

public partial class Player : Actor
{
	public override void Jump()
	{
		base.Jump();
	}

	public void WallJump()
	{
		Vector2 velo = stats.WallJumpVector;

		velocity = new Vector2(velo.X * -WallTouch, velo.Y);

		wallStick = 0f;

		canDoublejump = true;
	}

	#region Dashing
	public void Dash(int dir)
	{
		if (Disabled) return;
		Dash(position + new Vector2(stats.DashLength * dir, 0));
	}

	public void DashVertical(int dir)
	{
		if (Disabled) return;
		Dash(position + new Vector2(0, stats.DashLength * dir));
	}

	public void Dash(Vector2 target)
	{
		dashTarget = new DashTarget(position, target);
		dashCooldown.Reset();

		if (!IsOnGround) gravityIgnore = dashGravityIgnoreTime;

		if (IsLocalPlayer) SendDash(dashTarget);
	}

	public void DashStep()
	{
		Vector2 diffVector = dashTarget.endPosition - position;
		Vector2 directionVector = diffVector.Normalized();

		float factor = TKMath.Exp(Math.Max(0, 0.4f - dashTarget.distance), 1.5f, 30);

		Vector2 diffStepVector = directionVector * stats.DashVelocity * factor;

		if (diffStepVector.Length * Game.delta < diffVector.Length)
		{
			Vector2 collisionVec;
			bool collision = map.RayTraceCollision(position, position + diffStepVector * Game.delta, size, out collisionVec);

			map.AddEffect(new EffectLine(position, collisionVec,
				dashTarget.lastStep / stats.WarpVelocity, diffStepVector.Length / stats.WarpVelocity, 0.2f, map));
			position = collisionVec;

			dashTarget.distance += Game.delta;
			dashTarget.lastStep = diffStepVector.Length;

			if (collision)
				DashEnd();
		}
		else
		{
			DashEnd();
		}
	}

	public void DashEnd()
	{
		map.AddEffect(new EffectSpike(dashTarget.startPosition, position, 1f, 0.8f, map));
		map.AddEffect(new EffectRing(position, 4f, 1f, map));

		velocity = (dashTarget.endPosition - dashTarget.startPosition).Normalized() * stats.DashEndVelocity;

		dashTarget = null;
	}
	#endregion

	#region Warping
	public void Warp(Vector2 target)
	{
		if (Disabled) return;

		warpTarget = new WarpTarget(position, target);
		warpCooldown.Reset();

		if (IsLocalPlayer) SendWarp(warpTarget);
	}

	public void WarpStep()
	{
		Vector2 diffVector = warpTarget.endPosition - position;
		Vector2 directionVector = diffVector.Normalized();

		float factor = TKMath.Exp(Math.Max(0, 0.4f - warpTarget.distance), 2f, 20);

		Vector2 diffStepVector = directionVector * stats.WarpVelocity * factor;

		if (diffStepVector.Length * Game.delta < diffVector.Length)
		{
			List<Player> playerList = map.RayTrace(position, position + diffStepVector * Game.delta, size, this);
			foreach (Player p in playerList)
			{
				if (p.IsWarping)
				{
					WarpEnd(p);
					p.WarpEnd(this);
				}
			}

			if (warpTarget == null) return;

			map.AddEffect(new EffectLine(position, position + diffStepVector * Game.delta,
				warpTarget.lastStep / stats.WarpVelocity, diffStepVector.Length / stats.WarpVelocity, 0.2f, map));
			position += diffStepVector * Game.delta;

			warpTarget.distance += Game.delta;

			warpTarget.lastStep = diffStepVector.Length;
		}
		else
		{
			WarpEnd();
		}
	}

	public void WarpEnd()
	{
		WarpEnd(warpTarget.startPosition, warpTarget.endPosition, (warpTarget.endPosition - warpTarget.startPosition).Normalized() * stats.WarpEndVelocity);
	}

	public void WarpEnd(Player p)
	{
		WarpEnd(warpTarget.startPosition, p.position, (position - p.position).Normalized() * stats.WarpEndVelocity);
	}

	public void WarpEnd(Vector2 start, Vector2 end, Vector2 velo)
	{
		position = end;
		map.AddEffect(new EffectSpike(start, end, 1f, 0.8f, map));
		map.AddEffect(new EffectRing(end, 4f, 1f, map));

		velocity = velo;

		warpTarget = null;
	}
	#endregion
}