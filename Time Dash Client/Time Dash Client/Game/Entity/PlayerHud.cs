using OpenTK;
using System;
using TKTools;

public class PlayerHud : IDisposable
{
	Player player;

	CircleBar dodgeBar = new CircleBar(2f, 0.2f, 90, 360f);
	CircleBar dashBar = new CircleBar(3f, 1f, 90, 360f);
	CircleBar reloadBar = new CircleBar(4f, 0.8f, 90 + 45, -90f);
	CircleBar rearmBar = new CircleBar(4f, 0.8f, 90 + 45, -90f);

	Mesh ammoMesh = Mesh.Box;

	int ammoUseIndex;
	Timer ammoGainTimer = new Timer(0.4f, true), ammoUseTimer = new Timer(0.4f, true);
	float ammoAlpha = 1f;

	CircleBar healthBar = new CircleBar(5f, 0.4f, -90 - 45, 90);

	float healthAlpha = 0.2f;
	float healthBlinkFactor = 0f;

	public PlayerHud(Player p)
	{
		this.player = p;
	}

	public void Dispose()
	{
		dodgeBar.Dispose();
		dashBar.Dispose();
	}

	public void Logic()
	{
		if (!player.DodgeCooldown.IsDone)
			dodgeBar.Progress = 1f - player.DodgeCooldown.PercentageDone;
		if (!player.DashCooldown.IsDone)
			dashBar.Progress = 1f - player.DashCooldown.PercentageDone;

		dodgeBar.Logic();
		dashBar.Logic();

		ammoGainTimer.Logic();
		ammoUseTimer.Logic();

		if (player.health > 1f)
		{
			if (healthAlpha > 0.2f)
			{
				healthAlpha -= 0.4f * Game.delta;
				if (healthAlpha <= 0.2f) healthAlpha = 0.2f;
			}
		}
		else
		{
			healthBlinkFactor += 5f * Game.delta;
			healthAlpha = ((float)Math.Sin(healthBlinkFactor) + 1f) / 2;
		}

		healthBar.Progress = player.health / player.MaxHealth;
		healthBar.Logic();

		if (player.weapon.ReloadProgress != -1)
		{
			reloadBar.Progress = player.weapon.ReloadProgress;
			reloadBar.Logic();
		}
		else if (player.weapon.RearmProgress != -1 && player.weapon.FireType == WeaponStats.FireType.SingleTimed)
		{
			rearmBar.Progress = 1f - player.weapon.RearmProgress;
			rearmBar.Logic();
		}
		else if (player.weapon.Charge != 0 && player.weapon.FireType == WeaponStats.FireType.Charge)
		{
			rearmBar.Progress = player.weapon.Charge;
			rearmBar.Logic();
		}

		if (player.Ammo >= player.MaxAmmo && ammoGainTimer.IsDone && ammoUseTimer.IsDone)
			ammoAlpha -= 4f * Game.delta;
		else
			ammoAlpha += 4f * Game.delta;

		ammoAlpha = MathHelper.Clamp(ammoAlpha, 0f, 1f);
	}

	public void OnReload()
	{
		ammoGainTimer.Reset();
	}

	public void UseAmmo(int index)
	{
		ammoUseIndex = index;
		ammoUseTimer.Reset();

		ammoGainTimer.IsDone = true;
	}

	public void Hit()
	{
		healthAlpha = 1f;
	}

	public void Draw()
	{
		if (!player.DodgeCooldown.IsDone)
			dodgeBar.Draw(player.position, player.Color);

		if (!player.DashCooldown.IsDone)
			dashBar.Draw(player.position, player.Color);

		#region Health
		healthBar.Draw(player.position, new Color(1f, 0f, 0f, healthAlpha));
		#endregion

		#region Ammo
		int maxAmmo = player.MaxAmmo;
		float dir = 90f / (maxAmmo - 1);

		if (ammoAlpha > 0f && player.weapon.ReloadProgress == -1)
		{
			ammoMesh.Color = new Color(1f, 1f, 1f, ammoAlpha);

			for (int i = 0; i < player.MaxAmmo; i++)
			{
				float d = 90 + 45 - dir * i;

				if (!ammoGainTimer.IsDone)
				{
					float f = 1f - TKMath.Exp(ammoGainTimer.PercentageDone, 10);

					Color c = new Color(1f, 1f, 1f, f);
					ammoMesh.Color = c;

					ammoMesh.Reset();

					ammoMesh.Translate(player.position);
					ammoMesh.Rotate(d);
					ammoMesh.Translate(f * 2f, 0);
					ammoMesh.Scale(0.4f, 0.15f);

					ammoMesh.Draw();
				}
				else
				{
					ammoMesh.Color = new Color(1f, 1f, 1f, (i < player.Ammo ? 1f : 0.2f) * ammoAlpha);

					ammoMesh.Reset();

					ammoMesh.Translate(player.position);
					ammoMesh.Rotate(d);
					ammoMesh.Translate(2f, 0f);
					ammoMesh.Scale(0.4f, 0.15f);

					ammoMesh.Draw();
				}
			}
		}

		if (player.weapon.ReloadProgress != -1)
			reloadBar.Draw(player.position, Color.White);

		if (player.weapon.RearmProgress != -1 && player.weapon.FireType == WeaponStats.FireType.SingleTimed)
			rearmBar.Draw(player.position, Color.White);

		if (player.weapon.Charge != 0 && player.weapon.FireType == WeaponStats.FireType.Charge)
			rearmBar.Draw(player.position, Color.White);

		if (!ammoUseTimer.IsDone)
		{
			float d = 90 + 45 - dir * ammoUseIndex;
			float f = 1f - TKMath.Exp(ammoUseTimer.PercentageDone, 10);

			Color c = new Color(1f, 1f, 1f, 1f - f);
			ammoMesh.Color = c;

			ammoMesh.Reset();

			ammoMesh.Translate(player.position);
			ammoMesh.Rotate(d);
			ammoMesh.Translate(2f, 0);
			ammoMesh.Scale(0.4f, 0.15f);
			ammoMesh.Scale(1 + f * 2f);

			ammoMesh.Draw();
		}
		#endregion
	}
}