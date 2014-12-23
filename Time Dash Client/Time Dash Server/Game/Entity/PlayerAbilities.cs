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
		dashTarget = new DashTarget(position, target);
		dashCooldown.Reset();

		if (!IsOnGround)
		{
			gravityIgnore = dashGravityIgnoreTime;
			airDashNmbr--;
		}
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

		//Check for dashing players
		List<Player> playerCol = map.RayTrace(position, stepTarget, size, this);
		if (playerCol.Count > 0 && playerCol[0].IsDashing)
		{
			DashEnd(playerCol[0]);
			playerCol[0].DashEnd(this);

			SendDashCollisionToPlayer(playerCol[0], map.playerList);

			return;
		}

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
	}
	#endregion

	#region Warping
	public void CreateShadow()
	{
		if (shadow == null)
		{
			shadow = new PlayerShadow(this);
		}
	}

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

		//Check for dashing players
		List<Player> playerCol = map.RayTrace(position, stepTarget, size, this);
		if (playerCol.Count > 0)
		{
			foreach (Player p in playerCol)
			{
				if (p.IsWarping)
				{
					WarpEnd(playerCol[0]);
					playerCol[0].WarpEnd(this);

					SendWarpCollisionToPlayer(playerCol[0], map.playerList);

					return;
				}
				else
				{
					p.Hit();
				}
			}
		} 

		position += stepSize;
	}

	public void WarpEnd()
	{
		WarpEnd(warpTarget.startPosition, warpTarget.endPosition, (warpTarget.endPosition - warpTarget.startPosition).Normalized() * stats.WarpEndVelocity);
	}

	public void WarpEnd(Player p)
	{
		velocity = ((position - p.position).Normalized() + new Vector2(0, 1)).Normalized() * 20f;
		warpTarget = null;
	}

	public void WarpEnd(Vector2 start, Vector2 end, Vector2 velo)
	{
		position = end;
		velocity = velo;

		warpTarget = null;

		canDoublejump = true;
		airDashNmbr = stats.AirDashMax;
	}
	#endregion
}