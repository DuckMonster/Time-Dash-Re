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
		airDodgeNmbr = stats.AirDodgeMax;
	}

	#region Dodging
	int airDodgeNmbr = 1;

	public void Dodge(int dir)
	{
		if (!CanDodge) return;
		Dodge(position + new Vector2(stats.DodgeLength * dir, 0));
	}

	public void DodgeVertical(int dir)
	{
		if (!CanDodge) return;
		Dodge(position + new Vector2(0, stats.DodgeLength * dir));
	}

	public void Dodge(DodgeTarget t)
	{
		if (!CanDodge) return;
		position = t.startPosition;
		Dodge(t.endPosition);
	}

	public void Dodge(Vector2 target)
	{
		map.RayTraceCollision(position, target, size, out target); //Raytrace
		if ((target - position).Length < 0.1f) return;

		dodgeTarget = new DodgeTarget(position, target);

		dodgeCooldown.Reset();

		if (!IsOnGround)
		{
			gravityIgnore = dodgeGravityIgnoreTime;
			airDodgeNmbr--;
		}
		if (IsLocalPlayer) SendDodge(dodgeTarget);
	}

	public void DodgeStep()
	{
		dodgeTarget.timeTraveled += Game.delta;

		Vector2 dir = (dodgeTarget.endPosition - position).Normalized();
		float speedFactor = TKMath.Exp(Math.Max(0, 0.4f - dodgeTarget.timeTraveled * 5), 2f, 30);
		Vector2 stepSize = dir * stats.DodgeVelocity * speedFactor * Game.delta;

		//If youre there, just end it
		if (stepSize.Length > (dodgeTarget.endPosition - position).Length || (dodgeTarget.endPosition - position).Length <= 0.1f)
		{
			DodgeEnd();
			return;
		}

		Vector2 stepTarget = position + stepSize;

		//Add line effect
		map.AddEffect(new EffectLine(position, stepTarget,
				dodgeTarget.lastStep * 0.4f, speedFactor * 0.4f, 0.8f, Color, map));

		position = stepTarget;

		dodgeTarget.lastStep = speedFactor;
	}

	public void DodgeEnd(Player p)
	{
		velocity = ((position - p.position).Normalized() + new Vector2(0, 1)).Normalized() * 20f;
		dodgeTarget = null;
	}

	public void DodgeEnd()
	{
		position = dodgeTarget.endPosition;
		velocity = (dodgeTarget.endPosition - dodgeTarget.startPosition).Normalized() * stats.DodgeEndVelocity;
		dodgeTarget = null;

		map.AddEffect(new EffectRing(position, 4f, 0.5f, Color, map));

		//if (IsLocalPlayer) SendPosition();
	}
	#endregion

	#region Dashing
	public void CreateShadow()
	{
		if (shadow == null)
		{
			shadow = new PlayerShadow(this, mesh);
		}
	}

	public void Dash(DashTarget t)
	{
		if (Disabled || (t.endPosition - position).Length < 0.1f) return;

		position = t.startPosition;
		Dash(t.endPosition);
	}

	public void Dash(Vector2 target)
	{
		if (Disabled || (target - position).Length < 0.1f) return;

		dashTarget = new DashTarget(position, target);
		dashCooldown.Reset();

		if (IsLocalPlayer) SendDash(dashTarget);
	}

	public void DashStep()
	{
		dashTarget.timeTraveled += Game.delta;

		Vector2 direction = (dashTarget.endPosition - position).Normalized();
		float speedFactor = TKMath.Exp(Math.Max(0, 0.4f - dashTarget.timeTraveled), 2f, 20);

		Vector2 stepSize = direction * speedFactor * stats.DashVelocity * Game.delta;

		if (stepSize.Length > (dashTarget.endPosition - position).Length)
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
		DashEnd(dashTarget.startPosition, dashTarget.endPosition, (dashTarget.endPosition - dashTarget.startPosition).Normalized() * stats.DashEndVelocity);
	}

	public void DashEnd(Player p)
	{
		Vector2 velo = ((position - p.position).Normalized() + new Vector2(0, 1)).Normalized() * 20f;
		DashEnd(dashTarget.startPosition, position, velo);
	}

	public void DashEnd(Vector2 start, Vector2 end, Vector2 velo)
	{
		position = end;
		map.AddEffect(new EffectSpike(start, end, 1.5f, 0.8f, Color, map));
		map.AddEffect(new EffectRing(end, 6f, 0.8f, Color, map));

		velocity = velo;

		dashTarget = null;

		canDoublejump = true;
		shadow = null;
	}
	#endregion
}