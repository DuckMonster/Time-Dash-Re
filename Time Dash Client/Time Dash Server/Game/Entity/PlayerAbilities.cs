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
	Left,
	UpRight,
	DownRight,
	UpLeft,
	DownLeft
}

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

	#region Dodgeing
	public void Dodge(DodgeTarget dt)
	{
		position = dt.startPosition;
		Dodge(dt.direction);
	}

	public void Dodge(Direction d)
	{
		dodgeTarget = new DodgeTarget(position, d);
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

		dodgeCooldown.Reset();

		gravityIgnore = dodgeGravityIgnoreTime;
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
		float speedFactor = TKMath.Exp(Math.Max(0, 0.9f - dashTarget.timeTraveled * 5), 2f, 20);

		Vector2 stepSize = direction * speedFactor * stats.DashVelocity * Game.delta;

		if (stepSize.Length > (dashTarget.endPosition - position).Length)
		{
			DashEnd();
			return;
		}

		Vector2 stepTarget = position + stepSize;

		//Check for players
		List<Player> playerCol = map.RayTracePlayer(position, stepTarget, size * 1.8f, this);
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
					p.Die();
					OnKill(p);
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
	}
	#endregion

	Bullet[] bulletList = new Bullet[10];
	int bulletIndex = 0;

	public void Shoot(Vector2 target)
	{
		bulletList[bulletIndex] = new Bullet(this, target, map);
		bulletList[bulletIndex].Logic();
		bulletIndex = (bulletIndex + 1) % bulletList.Length;

		SendShootToPlayer(position, target, map.playerList);
	}

	public static Direction GetInputDirection(PlayerInput input)
	{
		Vector2 vec = Vector2.Zero;

		if (input[PlayerKey.Right]) vec.X += 1;
		if (input[PlayerKey.Left]) vec.X -= 1;
		if (input[PlayerKey.Up]) vec.Y += 1;
		if (input[PlayerKey.Down]) vec.Y -= 1;

		if (vec == new Vector2(1, 0)) return Direction.Right;
		if (vec == new Vector2(-1, 0)) return Direction.Left;
		if (vec == new Vector2(0, 1)) return Direction.Up;
		if (vec == new Vector2(0, -1)) return Direction.Down;
		if (vec == new Vector2(1, 1)) return Direction.UpRight;
		if (vec == new Vector2(-1, 1)) return Direction.UpLeft;
		if (vec == new Vector2(1, -1)) return Direction.DownRight;
		if (vec == new Vector2(-1, -1)) return Direction.DownLeft;

		return Direction.Right;
	}

	public static Vector2 GetDirectionVector(Direction dir)
	{
		switch (dir)
		{
			case Direction.Right: return new Vector2(1, 0);
			case Direction.Left: return new Vector2(-1, 0);
			case Direction.Up: return new Vector2(0, 1);
			case Direction.Down: return new Vector2(0, -1);
			case Direction.UpRight: return new Vector2(1, 1).Normalized();
			case Direction.UpLeft: return new Vector2(-1, 1).Normalized();
			case Direction.DownRight: return new Vector2(1, -1).Normalized();
			case Direction.DownLeft: return new Vector2(-1, -1).Normalized();
			default: return new Vector2(0, 0);
		}
	}
}