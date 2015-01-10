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

	#region Dodgeing
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
		dodgeTarget = new DodgeTarget(position, target);
		dodgeCooldown.Reset();

		if (!IsOnGround)
		{
			gravityIgnore = dodgeGravityIgnoreTime;
			airDodgeNmbr--;
		}
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

		//Check for dodgeing players
		List<Player> playerCol = map.RayTracePlayer(position, stepTarget, size, this);
		if (playerCol.Count > 0 && playerCol[0].IsDodging)
		{
			DodgeEnd(playerCol[0]);
			playerCol[0].DodgeEnd(this);

			SendDodgeCollisionToPlayer(playerCol[0], map.playerList);

			return;
		}

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
	}
	#endregion

	#region Dashing
	public void CreateShadow()
	{
		if (shadow == null)
		{
			shadow = new PlayerShadow(this);
		}
	}

	public void Dash(DashTarget t)
	{
		if (Disabled) return;

		position = t.startPosition;
		Dash(t.endPosition);
	}

	public void Dash(Vector2 target)
	{
		if (Disabled) return;

		dashTarget = new DashTarget(position, target);
		dashCooldown.Reset();
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

		//Check for players
		List<Player> playerCol = map.RayTracePlayer(position, stepTarget, size, this);
		if (playerCol.Count > 0)
		{
			foreach (Player p in playerCol)
			{
				if (p.IsDashing)
				{
					DashEnd(playerCol[0]);
					playerCol[0].DashEnd(this);

					SendDashCollisionToPlayer(playerCol[0], map.playerList);

					return;
				}
				else if (!p.IsDodging && !AlliedWith(p))
				{
					Kill(p);
				}
			}
		} 

		position += stepSize;
	}

	public void DashEnd()
	{
		DashEnd(dashTarget.startPosition, dashTarget.endPosition, (dashTarget.endPosition - dashTarget.startPosition).Normalized() * stats.DashEndVelocity);
	}

	public void DashEnd(Player p)
	{
		velocity = ((position - p.position).Normalized() + new Vector2(0, 1)).Normalized() * 20f;
		dashTarget = null;
	}

	public void DashEnd(Vector2 start, Vector2 end, Vector2 velo)
	{
		position = end;
		velocity = velo;

		dashTarget = null;

		canDoublejump = true;
		airDodgeNmbr = stats.AirDodgeMax;
	}
	#endregion
}