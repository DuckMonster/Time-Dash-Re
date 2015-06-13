using OpenTK;
using System;
using TKTools;
using TKTools.Context;
using TKTools.Mathematics;

public class PlayerHud : IDisposable
{
	Player player;

	CircleBar dodgeBar = new CircleBar(2f, 0.2f, 90, 360f);
	CircleBar dashBar = new CircleBar(3f, 1f, 90, 360f);
	CircleBar reloadBar = new CircleBar(4f, 0.8f, 90 + 45, -90f);
	CircleBar rearmBar = new CircleBar(4f, 0.8f, 90 + 45, -90f);

	Mesh ammoMesh = Mesh.CreateFromPrimitive(MeshPrimitive.Quad);

	int ammoUseIndex;
	Timer ammoGainTimer = new Timer(0.4f, true), ammoUseTimer = new Timer(0.4f, true);
	float ammoAlpha = 1f;

	CircleBar healthBar = new CircleBar(5f, 0.4f, -90 - 45, 90);

	float healthAlpha = 0.2f;
	float healthBlinkFactor = 0f;

	Mesh weaponMesh;

	float weaponEquipAnimation = 0f;

	public PlayerHud(Player p)
	{
		this.player = p;

		weaponMesh = Mesh.CreateFromPrimitive(MeshPrimitive.Quad);
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

		if (player.Weapon != null)
		{
			if (player.Weapon.ReloadProgress != -1)
			{
				reloadBar.Progress = player.Weapon.ReloadProgress;
				reloadBar.Logic();
			}
			else if (player.Weapon.RearmProgress != -1 && player.Weapon.FireType == WeaponStats.FireType.SingleTimed)
			{
				rearmBar.Progress = 1f - player.Weapon.RearmProgress;
				rearmBar.Logic();
			}
			else if (player.Weapon.FireType == WeaponStats.FireType.Charge)
			{
				rearmBar.Progress = player.Weapon.Charge;
				rearmBar.Logic();
			}

			if (player.Ammo >= player.MaxAmmo && ammoGainTimer.IsDone && ammoUseTimer.IsDone)
				ammoAlpha -= 4f * Game.delta;
			else
				ammoAlpha += 4f * Game.delta;

			ammoAlpha = MathHelper.Clamp(ammoAlpha, 0f, 1f);
		}

		weaponEquipAnimation += ((float)player.weaponIndex - weaponEquipAnimation) * 18f * Game.delta;
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
			dodgeBar.Draw(player.Position, player.Color);

		if (!player.DashCooldown.IsDone)
			dashBar.Draw(player.Position, player.Color);

		#region Health
		healthBar.Draw(player.Position, new Color(1f, 0f, 0f, healthAlpha));
		#endregion

		#region Ammo
		if (player.Weapon != null)
		{
			int maxAmmo = player.MaxAmmo;
			float dir = 90f / (maxAmmo - 1);

			if (ammoAlpha > 0f && player.Weapon.ReloadProgress == -1)
			{
				ammoMesh.Color = new Color(1f, 1f, 1f, ammoAlpha);

				for (int i = 0; i < player.MaxAmmo; i++)
				{
					float d = 90 + 45 - dir * i;

					if (!ammoGainTimer.IsDone)
					{
						float f = 1f - TKMath.Exp(ammoGainTimer.PercentageDone);

						Color c = new Color(1f, 1f, 1f, f);
						ammoMesh.Color = c;

						ammoMesh.Reset();

						ammoMesh.Translate(player.Position);
						ammoMesh.RotateZ(d);
						ammoMesh.Translate(f * 2f, 0);
						ammoMesh.Scale(0.4f, 0.15f);

						ammoMesh.Draw();
					}
					else
					{
						ammoMesh.Color = new Color(1f, 1f, 1f, (i < player.Ammo ? 1f : 0.2f) * ammoAlpha);

						ammoMesh.Reset();

						ammoMesh.Translate(player.Position);
						ammoMesh.RotateZ(d);
						ammoMesh.Translate(2f, 0f);
						ammoMesh.Scale(0.4f, 0.15f);

						ammoMesh.Draw();
					}
				}
			}

			if (player.Weapon.ReloadProgress != -1)
				reloadBar.Draw(player.Position, Color.White);

			if (player.Weapon.RearmProgress != -1 && player.Weapon.FireType == WeaponStats.FireType.SingleTimed)
				rearmBar.Draw(player.Position, Color.White);

			if (player.Weapon.Charge != 0 && player.Weapon.FireType == WeaponStats.FireType.Charge)
				rearmBar.Draw(player.Position, Color.White);

			if (!ammoUseTimer.IsDone)
			{
				float d = 90 + 45 - dir * ammoUseIndex;
				float f = 1f - TKMath.Exp(ammoUseTimer.PercentageDone, 10);

				Color c = new Color(1f, 1f, 1f, 1f - f);
				ammoMesh.Color = c;

				ammoMesh.Reset();

				ammoMesh.Translate(player.Position);
				ammoMesh.RotateZ(d);
				ammoMesh.Translate(2f, 0);
				ammoMesh.Scale(0.4f, 0.15f);
				ammoMesh.Scale(1 + f * 2f);

				ammoMesh.Draw();
			}
		}
		#endregion
	}

	public void DrawHUD()
	{
		#region Inventory
		DrawWeapon(0, 1f - weaponEquipAnimation);
		DrawWeapon(1, weaponEquipAnimation);
		#endregion
	}

	void DrawWeapon(int index, float animation)
	{
		if (player.inventory[index] == null) return;
		weaponMesh.Texture = Weapon.GetIcon(player.inventory[index].type);
		weaponMesh.Color = new Color(1f, 1f, 1f, 0.3f + 0.7f * animation);

		weaponMesh.Reset();

		weaponMesh.Translate(0, -4f);
		weaponMesh.Scale(index == 0 ? -1 : 1, 1);

		weaponMesh.Translate(0.6f, 0f);
		weaponMesh.Translate(0f, -0.2f + 0.5f * animation);
		weaponMesh.RotateZ(-40f + 90f * animation);
		weaponMesh.Translate(0.3f, 0f);
		weaponMesh.RotateZ(-15f + 30f * animation);
		weaponMesh.Scale(2f);

		weaponMesh.Draw();
	}
}