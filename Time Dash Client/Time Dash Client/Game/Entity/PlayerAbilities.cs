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

	public void Dash(DashTarget t)
	{
		if (Disabled) return;
		position = t.startPosition;
		Dash(t.endPosition);
	}

	public void Dash(Vector2 target)
	{
		map.RayTraceCollision(position, target, size, out target); //Raytrace
		dashTarget = new DashTarget(position, target);
		dashCooldown.Reset();

		if (!IsOnGround) gravityIgnore = dashGravityIgnoreTime;
		if (IsLocalPlayer) SendDash(dashTarget);
	}

	public void DashStep()
	{
		dashTarget.timeTraveled += Game.delta;

		Vector2 dir = dashTarget.Direction;

		float speedFactor = TKMath.Exp(Math.Max(0, 0.4f - dashTarget.timeTraveled * 5), 2f, 30);

		Vector2 stepSize = dir * stats.DashVelocity * speedFactor * Game.delta;

		//If youre there, just end it
		if (stepSize.Length > (dashTarget.endPosition - position).Length || (dashTarget.endPosition - position).Length <= 0.1f)
		{
			DashEnd();
			return;
		}

		Vector2 stepTarget = position + stepSize;

		//Add line effect
		map.AddEffect(new EffectLine(position, stepTarget,
				dashTarget.lastStep * 0.4f, speedFactor * 0.4f, 0.8f, map));

		position = stepTarget;

		dashTarget.lastStep = speedFactor;
	}

	public void DashEnd()
	{
		//map.AddEffect(new EffectSpike(dashTarget.startPosition, position, 1f, 0.8f, map));
		map.AddEffect(new EffectRing(position, 4f, 0.5f, map));

		velocity = (dashTarget.endPosition - dashTarget.startPosition).Normalized() * stats.DashEndVelocity;

		dashTarget = null;
	}
	#endregion

	#region Warping
	public void Warp(WarpTarget t)
	{
		if (Disabled) return;

		position = t.startPosition;
		Warp(t.endPosition);
	}

	public void Warp(Vector2 target)
	{
		if (Disabled) return;

		warpTarget = new WarpTarget(position, target);
		warpCooldown.Reset();

		if (IsLocalPlayer) SendWarp(warpTarget);
	}

	public void WarpStep()
	{
		warpTarget.timeTraveled += Game.delta;

		Vector2 direction = warpTarget.Direction;
		float speedFactor = TKMath.Exp(Math.Max(0, 0.4f - warpTarget.timeTraveled), 2f, 20);

		Vector2 stepSize = direction * speedFactor * stats.WarpVelocity * Game.delta;

		if (stepSize.Length > (warpTarget.endPosition - position).Length)
		{
			WarpEnd();
			return;
		}

		position += stepSize;
		warpTarget.lastStep = speedFactor;
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