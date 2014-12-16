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
		map.RayTraceCollision(position, target, size, out target); //Raytrace
		dashTarget = new DashTarget(position, target);
		dashCooldown.Reset();

		if (!IsOnGround) gravityIgnore = dashGravityIgnoreTime;
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

		position = stepTarget;

		dashTarget.lastStep = speedFactor;
	}

	public void DashEnd()
	{
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
		velocity = velo;

		warpTarget = null;
	}
	#endregion
}