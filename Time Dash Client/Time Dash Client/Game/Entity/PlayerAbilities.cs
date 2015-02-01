using System;
using System.Collections.Generic;

using OpenTK;
using TKTools;
using System.Diagnostics;

public enum Direction
{
	Up,
	Down,
	Right,
	Left
}

public partial class Player : Actor
{
	public override void Jump()
	{
		//jumpSound.Play();
		base.Jump();
	}

	public void WallJump()
	{
		Vector2 velo = stats.WallJumpVector;

		velocity = new Vector2(velo.X * -WallTouch, velo.Y);

		wallStick = 0f;

		canDoublejump = true;
	}

	#region Dodging
	public void Dodge(DodgeTarget dt)
	{
		position = dt.startPosition;
		Dodge(dt.direction);
	}

	public void Dodge(Direction d)
	{
		dodgeTarget = new DodgeTarget(position, d);
		if (IsLocalPlayer) SendDodge(dodgeTarget);
	}

	public void DodgeStep()
	{
		dodgeTarget.timeTraveled += Game.delta;
		Vector2 dir = dodgeTarget.DirectionVector;

		float speedFactor = TKMath.Exp(Math.Max(0, 0.4f - dodgeTarget.timeTraveled * 5), 2f, 30);

		Vector2 step = dir * stats.DodgeVelocity * speedFactor * Game.delta;

		//Stepping
		if ((dodgeTarget.direction == Direction.Right || dodgeTarget.direction == Direction.Left) && map.GetCollision(this, step))
		{
			float stepSizeFactor = stats.StepSize * step.Length;

			if (!map.GetCollision(this, step + new Vector2(0, stepSizeFactor)))
			{
				int accuracy = 16;
				float currentStep = 0;
				float testStepSize = stepSizeFactor / accuracy;

				for (int i = 0; i < accuracy; i++)
				{
					if (map.GetCollision(this, step + new Vector2(0, currentStep)))
						currentStep += testStepSize;
					else
						break;
				}

				step.Y += currentStep;
			}
		}

		Vector2 collisionPosition;
		bool collision = map.RayTraceCollision(position, position + step, size, out collisionPosition);

		step = collisionPosition - position;

		dodgeTarget.stepLength = speedFactor;
		dodgeTarget.stepAngle = TKMath.GetAngle(step);

		position = collisionPosition;
		if (dodgeTarget.TargetReached(position) || collision)
			DodgeEnd();
	}

	public void DodgeEnd()
	{
		velocity = dodgeTarget.DirectionVector * stats.DodgeEndVelocity;

		dodgeTarget = null;
		map.AddEffect(new EffectRing(position, 4f, 0.5f, Color, map));

		dodgeCooldown.Reset();

		if (!IsOnGround)
			gravityIgnore = dodgeGravityIgnoreTime;

		bufferFrame = true;
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

		map.AddEffect(new EffectRing(position, 2.4f, 0.8f, Color, map));

		if (IsLocalPlayer) SendDash(dashTarget);
	}

	public void DashStep()
	{
		dashTarget.timeTraveled += Game.delta;

		Vector2 direction = (dashTarget.endPosition - position).Normalized();
		float speedFactor = TKMath.Exp(Math.Max(0, 0.9f - dashTarget.timeTraveled * 5), 2f, 20);

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
		map.AddEffect(new EffectSpike(start, end, 2.2f, 0.8f, Color, map));
		map.AddEffect(new EffectRing(end, 6f, 0.8f, Color, map));

		velocity = velo;

		dashTarget = null;

		canDoublejump = true;
		shadow = null;

		//dashSound.Play();
	}
	#endregion
}