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

	public void Dash(Vector2 target)
	{
		if (Disabled) return;

		dashTarget = new DashTarget(position, target);
		dashCooldown.Reset();

		if (!IsOnGround) gravityIgnore = dashGravityIgnoreTime;
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

			List<Player> playerList = map.RayTrace(position, position + diffStepVector * Game.delta, size, this);
			foreach (Player p in playerList)
			{
				if (!p.Disabled)
				{
					p.Disabled = true;

					if (lastDirection.X != 0)
					{
						Vector2 hitVelo = TKMath.GetAngleVector(90f - 55f * lastDirection.X) * stats.DashEndVelocity;

						p.velocity = hitVelo;
						p.SendPositionToPlayer(map.playerList);
					}
					else
					{
						p.velocity = new Vector2(0, stats.DashEndVelocity);
						p.SendPositionToPlayer(map.playerList);
					}
				}
			}

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
		velocity = (dashTarget.endPosition - dashTarget.startPosition).Normalized() * stats.DashEndVelocity;

		dashTarget = null;
	}

	public void Warp(Vector2 target)
	{
		if (Disabled) return;

		warpTarget = new WarpTarget(position, target);
		warpCooldown.Reset();
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
				else
				{
					p.Hit();
				}
			}

			if (warpTarget == null) return;

			position += diffStepVector * Game.delta;
			warpTarget.distance += Game.delta;
		}
		else
		{
			WarpEnd();
		}
	}

	public void WarpEnd()
	{
		WarpEnd(warpTarget.endPosition, (warpTarget.endPosition - warpTarget.startPosition).Normalized() * stats.WarpEndVelocity);
	}

	public void WarpEnd(Player p)
	{
		WarpEnd(p.position, (position - p.position).Normalized() * stats.WarpEndVelocity);
	}

	public void WarpEnd(Vector2 pos, Vector2 velo)
	{
		position = pos;
		velocity = velo;
		warpTarget = null;
	}
}