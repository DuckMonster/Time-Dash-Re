using System;
using System.Collections.Generic;

using OpenTK;
using TKTools;
using System.Diagnostics;

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
		airDashNmbr = stats.AirDashMax;
	}

	#region Dashing
	int airDashNmbr = 1;

	public void Dash(int dir)
	{
		if (!CanDash) return;
		Dash(position + new Vector2(stats.DashLength * dir, 0));
	}

	public void DashVertical(int dir)
	{
		if (!CanDash) return;
		Dash(position + new Vector2(0, stats.DashLength * dir));
	}

	public void Dash(DashTarget t)
	{
		if (!CanDash) return;
		position = t.startPosition;
		Dash(t.endPosition);
	}

	public void Dash(Vector2 target)
	{
		map.RayTraceCollision(position, target, size, out target); //Raytrace
		if ((target - position).Length < 0.1f) return;

		dashTarget = new DashTarget(position, target);

		dashCooldown.Reset();

		if (!IsOnGround)
		{
			gravityIgnore = dashGravityIgnoreTime;
			airDashNmbr--;
		}
		if (IsLocalPlayer) SendDash(dashTarget);
	}

	public void DashStep()
	{
		dashTarget.timeTraveled += Game.delta;

		Vector2 dir = (dashTarget.endPosition - position).Normalized();
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
				dashTarget.lastStep * 0.4f, speedFactor * 0.4f, 0.8f, MyColor, map));

		position = stepTarget;

		dashTarget.lastStep = speedFactor;
	}

	public void DashEnd(Player p)
	{
		velocity = ((position - p.position).Normalized() + new Vector2(0, 1)).Normalized() * 20f;
		dashTarget = null;
	}

	public void DashEnd()
	{
		position = dashTarget.endPosition;
		velocity = (dashTarget.endPosition - dashTarget.startPosition).Normalized() * stats.DashEndVelocity;
		dashTarget = null;

		map.AddEffect(new EffectRing(position, 4f, 0.5f, MyColor, map));

		//if (IsLocalPlayer) SendPosition();
	}
	#endregion

	#region Warping
	public void CreateShadow()
	{
		if (shadow == null)
		{
			shadow = new PlayerShadow(this, mesh);
		}
	}

	public void Warp(WarpTarget t)
	{
		if (Disabled || (t.endPosition - position).Length < 0.1f) return;

		position = t.startPosition;
		Warp(t.endPosition);
	}

	public void Warp(Vector2 target)
	{
		if (Disabled || (target - position).Length < 0.1f) return;

		warpTarget = new WarpTarget(position, target);
		warpCooldown.Reset();

		if (IsLocalPlayer) SendWarp(warpTarget);
	}

	public void WarpStep()
	{
		warpTarget.timeTraveled += Game.delta;

		Vector2 direction = (warpTarget.endPosition - position).Normalized();
		float speedFactor = TKMath.Exp(Math.Max(0, 0.4f - warpTarget.timeTraveled), 2f, 20);

		Vector2 stepSize = direction * speedFactor * stats.WarpVelocity * Game.delta;

		if (stepSize.Length > (warpTarget.endPosition - position).Length)
		{
			WarpEnd();
			return;
		}

		Vector2 stepTarget = position + stepSize;

		position = stepTarget;
		warpTarget.lastStep = speedFactor;
	}

	public void WarpEnd()
	{
		WarpEnd(warpTarget.startPosition, warpTarget.endPosition, (warpTarget.endPosition - warpTarget.startPosition).Normalized() * stats.WarpEndVelocity);
	}

	public void WarpEnd(Player p)
	{
		Vector2 velo = ((position - p.position).Normalized() + new Vector2(0, 1)).Normalized() * 20f;
		WarpEnd(warpTarget.startPosition, position, velo);
	}

	public void WarpEnd(Vector2 start, Vector2 end, Vector2 velo)
	{
		position = end;
		map.AddEffect(new EffectSpike(start, end, 1f, 0.8f, MyColor, map));
		map.AddEffect(new EffectRing(end, 4f, 1f, MyColor, map));

		velocity = velo;

		warpTarget = null;

		canDoublejump = true;
		shadow = null;
	}
	#endregion
}