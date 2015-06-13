using OpenTK;
using System;
using TKTools;
using TKTools.Context;

namespace ShopMenu
{
	public class ShopMenu : Menu
	{
		public WeaponList? selectedWeapon = null;
		Sprite weaponSprite = new Sprite();
		float weaponRotation = 0f;

		BuyButton buyButton;

		Sprite lockSprite = new Sprite(Art.Load("Res/weapons/lock.png")),
			scrapSprite = new Sprite(Art.Load("Res/scrap.png"));

		TextBox costBox;

		Vector2 previewWeaponPosition = new Vector2(2.4f, 2);

		EquipButton[] equipButtons = new EquipButton[2];

		Vector2 PreviewPosition
		{
			get { return previewWeaponPosition * new Vector2(Game.AspectRatio, 1f); }
		}

		public ShopMenu(Map map)
			: base(new Vector2(18f, 10f), map)
		{
			int nmbrOfWeapons = Enum.GetNames(typeof(WeaponList)).Length;

			float padding = 0.2f;

			Vector2 buttonSize = new Vector2(1.25f, 1.25f);

			for (int i=0; i<nmbrOfWeapons; i++)
			{
				float x = (i % 5) * (buttonSize.X + padding);
				float y = (i / 5) * (-buttonSize.Y - padding);

				AddButton(new WeaponButton(
					(WeaponList)(i % nmbrOfWeapons),
					(w) => { selectedWeapon = w; costBox.Text = WeaponStats.GetStats(w).scrapCost.ToString(); } ,
					new Vector2(x, y), buttonSize, 
					this, map));
			}

			equipButtons[0] = new EquipButton(0, (id) => map.LocalPlayer.SendInventory(id, selectedWeapon.Value), previewWeaponPosition - new Vector2(4f, 2f), new Vector2(1.2f, 1.2f), this, map);
			equipButtons[1] = new EquipButton(1, (id) => map.LocalPlayer.SendInventory(id, selectedWeapon.Value), previewWeaponPosition - new Vector2(-4f, 2f), new Vector2(1.2f, 1.2f), this, map);

			buyButton = new BuyButton(() => map.LocalPlayer.SendBuyWeapon(selectedWeapon.Value), previewWeaponPosition + new Vector2(0, -4), new Vector2(3f, 1f), this, map);

			lockSprite.FillColor = true;
			lockSprite.Color = new Color(0f, 0f, 0f, 0.8f);

			costBox = new TextBox(new System.Drawing.Font("Adobe Song Std L", 200f));
			costBox.VerticalAlign = TextBox.VerticalAlignment.Center;
			costBox.UpdateRate = 0;
			costBox.SetHeight = 3f;
			costBox.Color = SYScrap.HUDColor;
        }

		public override void Logic()
		{
			base.Logic();

			if (selectedWeapon != null)
			{
				buyButton.Position = PreviewPosition + new Vector2(0f, -2.5f);
				equipButtons[0].Position = PreviewPosition + new Vector2(-2f, -2.5f);
				equipButtons[1].Position = PreviewPosition + new Vector2(2f, -2.5f);

				buyButton.Logic();
				equipButtons[0].Logic();
				equipButtons[1].Logic();
			}

			weaponRotation += 40f * Game.delta;
		}

		public override void Draw()
		{
			base.Draw();

			if (selectedWeapon != null)
			{
				weaponSprite.Texture = Weapon.GetIcon(selectedWeapon.Value);
				if (map.LocalPlayer.OwnsWeapon(selectedWeapon.Value))
				{
					weaponSprite.FillColor = false;
					weaponSprite.Color = Color.White;
				} else
				{
					weaponSprite.FillColor = true;
					weaponSprite.Color = Color.Black;
				}

				weaponSprite.Draw(PreviewPosition, 4f, weaponRotation);

				if (!map.LocalPlayer.OwnsWeapon(selectedWeapon.Value))
				{
					lockSprite.Draw(PreviewPosition, 2f, 0f);
					scrapSprite.Draw(PreviewPosition - new Vector2(0.3f, 0.2f), 1.2f, 10f);

					Mesh m = costBox.Mesh;

					m.Reset();

					m.Translate(PreviewPosition - new Vector2(0.3f, 0f));
					m.Draw();

					buyButton.Draw();
				} else
				{
					equipButtons[0].Draw();
					equipButtons[1].Draw();
				}
			}
		}
	}
}