﻿using System;
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
	public class DashTarget
	{
		public float timeTraveled = 0;
		public float lastStep = 0;

		public float angle;

		public Vector2 startPosition;
		public Vector2 endPosition;

		public Vector2 Direction
		{
			get
			{
				return (endPosition - startPosition).Normalized();
			}
		}

		public DashTarget(Vector2 a, Vector2 b)
		{
			startPosition = a;
			endPosition = b;
			angle = TKMath.GetAngle(a, b);
		}
	}
	public class DodgeTarget
	{
		public float timeTraveled = 0;

		public float stepLength = 0;
		public float stepAngle = 0;

		public Vector2 startPosition;
		public Direction direction;

		public float frameDirection = 0f;

		public Vector2 DirectionVector
		{
			get
			{
				switch (direction)
				{
					case Direction.Down: return new Vector2(0, -1);
					case Direction.Up: return new Vector2(0, 1);
					case Direction.Right: return new Vector2(1, 0);
					case Direction.Left: return new Vector2(-1, 0);
				}

				return new Vector2(1, 0);
			}
		}

		public DodgeTarget(Vector2 start, Direction dir)
		{
			startPosition = start;
			direction = dir;
		}

		public bool TargetReached(Vector2 pos)
		{
			float posLen = 0;

			switch (direction)
			{
				case Direction.Right:
				case Direction.Left:
					posLen = Math.Abs(pos.X - startPosition.X);
					break;

				case Direction.Up:
				case Direction.Down:
					posLen = Math.Abs(pos.Y - startPosition.Y);
					break;
			}

			return (posLen >= Stats.defaultStats.DodgeLength);
		}
	}

	public override void Jump()
	{
		base.Jump();

		gravityIgnore = 0f;
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
		if ((dodgeTarget.direction == Direction.Right || dodgeTarget.direction == Direction.Left) && Map.GetCollision(this, step))
		{
			float stepSizeFactor = stats.StepSize * step.Length;

			if (!Map.GetCollision(this, step + new Vector2(0, stepSizeFactor)))
			{
				int accuracy = 16;
				float currentStep = 0;
				float testStepSize = stepSizeFactor / accuracy;

				for (int i = 0; i < accuracy; i++)
				{
					if (Map.GetCollision(this, step + new Vector2(0, currentStep)))
						currentStep += testStepSize;
					else
						break;
				}

				step.Y += currentStep;
			}
		}

		Vector2 collisionPosition;
		bool collision = Map.RayTraceCollision(position, position + step, size, out collisionPosition);

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
		position = t.startPosition;
		Dash(t.endPosition);
	}

	public void Dash(Vector2 target)
	{
		dashTarget = new DashTarget(position, target);
		dashCooldown.Reset();
	}

	public void DashStep()
	{
		dashTarget.timeTraveled += Game.delta;

		Vector2 direction = dashTarget.Direction;
		float speedFactor = TKMath.Exp(Math.Max(0, 0.4f - dashTarget.timeTraveled * 10), 2f, 20);

		Vector2 stepSize = direction * speedFactor * stats.DashVelocity * Game.delta;

		if (stepSize.Length > (dashTarget.endPosition - position).Length)
		{
			DashEnd();
			return;
		}

		Vector2 stepTarget = position + stepSize;

		//Check for players
		List<Player> actorCol = Map.RayTraceActor<Player>(position, stepTarget, size * 1.8f, this);
		if (actorCol.Count > 0)
		{
			foreach (Player p in actorCol)
			{
				if (p.IsDashing)
				{
					DashEnd((Player)actorCol[0]);
					((Player)actorCol[0]).DashEnd(this);

					SendDashCollisionToPlayer((Player)actorCol[0], Map.playerList);

					return;
				}
				else if (!p.IsDodging && !AlliedWith(p))
				{
					p.Hit(p.MaxAmmo, TKMath.GetAngle(direction), this);
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

	Projectile[] projectileList = new Projectile[10];
	int projectileIndex = 0;

	public void Shoot(Vector2 target)
	{
		if (weapon.FireType == WeaponStats.FireType.Charge)
			SendShootToPlayer(position, target, weapon.Charge, Map.playerList);
		else
			SendShootToPlayer(position, target, Map.playerList);

		projectileList[projectileIndex] = weapon.CreateProjectile(target, projectileIndex);
		projectileList[projectileIndex].Logic();
		projectileIndex = (projectileIndex + 1) % projectileList.Length;

		weapon.OnShoot();
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