using OpenTK;
using System;
using TKTools;
using TKTools.Context;

namespace ShopMenu
{
	public class WeaponButton : Button
	{
		static Vector2 ButtonOrigin
		{
			get { return new Vector2(-4f * Game.AspectRatio, 3f); }
		}

		public delegate void SelectWeapon(WeaponList w);

		WeaponList weaponType;
		
		Sprite weaponSprite;
		SelectWeapon selectWeapon;

		Sprite lockSprite = new Sprite(Art.Load("Res/weapons/lock.png"));

		Sprite scrapSprite = new Sprite(Art.Load("Res/scrap.png"));

		TextBox costBox;

		public override Vector2 Position
		{
			get { return ButtonOrigin + position; }
		}

		protected override float Alpha
		{
			get
			{
				if ((menu as ShopMenu).selectedWeapon == weaponType)
					return 1f;
				else
					return base.Alpha;
			}
		}

		protected override Color Color
		{
			get
			{
				if (!map.LocalPlayer.OwnsWeapon(weaponType))
				{
					if ((map.LocalPlayer as SYPlayer).CollectedScrap >= WeaponStats.GetStats(weaponType).scrapCost)
						return new Color(0f, 0.7f, 0f, Alpha);
					else
						return new Color(0.7f, 0f, 0f, Alpha);
				}
				else return base.Color;
			}
		}

		public WeaponButton(WeaponList weapon, SelectWeapon selectWeapon, Vector2 position, Vector2 size, ShopMenu menu, Map map)
			: base("", position, size, menu, map)
		{
			this.weaponType = weapon;
			this.selectWeapon = selectWeapon;

			weaponSprite = new Sprite(Weapon.GetIcon(weapon));

			lockSprite.FillColor = true;
			lockSprite.Color = new Color(0, 0, 0, 0.5f);

			costBox = new TextBox(120f);
			costBox.SetHeight = size.Y * 0.4f;
			costBox.Text = WeaponStats.GetStats(weapon).scrapCost.ToString();
			costBox.VerticalAlign = TextBox.VerticalAlignment.Center;
			costBox.HorizontalAlign = TextBox.HorizontalAlignment.Left;
		}

		protected override void OnClicked()
		{
			selectWeapon(weaponType);
		}

		public override void Logic()
		{
			base.Logic();
		}

		public override void Draw()
		{
			base.Draw();

			if (map.LocalPlayer.OwnsWeapon(weaponType))
			{
				weaponSprite.Color = Color.White;
				weaponSprite.FillColor = false;
			} else
			{
				weaponSprite.Color = Color.Black;
				weaponSprite.FillColor = true;
			}

			weaponSprite.Draw(Position, size, 10f + Rotation);

			if (!map.LocalPlayer.OwnsWeapon(weaponType))
			{
				lockSprite.Draw(Position, size, Rotation);
				scrapSprite.Draw(Position - new Vector2(size.X * 0.3f, size.Y * 0.4f), size * 0.4f, 20f + Rotation);

				Mesh m = costBox.Mesh;

				m.Reset();

				m.Translate(Position);
				m.Translate(-size.X * 0.3f, -size.Y * 0.4f);
				m.RotateZ(Rotation);

				m.Draw();
			}
		}
	}
}