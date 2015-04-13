using OpenTK;
using System;
using TKTools;

namespace ShopMenu
{
	public class WeaponButton : Button
	{
		static Texture[] weaponIcons;
		public static Texture GetWeaponIcon(WeaponList weapon)
		{
			if (weaponIcons == null)
			{
				weaponIcons = new Texture[Enum.GetNames(typeof(WeaponList)).Length];
				weaponIcons[(int)WeaponList.Pistol] = Art.Load("Res/weapons/pistol.png");
				weaponIcons[(int)WeaponList.Rifle] = Art.Load("Res/weapons/rifle.png");
				weaponIcons[(int)WeaponList.GrenadeLauncher] = Art.Load("Res/weapons/grenadeLauncher.png");
				weaponIcons[(int)WeaponList.Bow] = Art.Load("Res/weapons/bow.png");
			}

			return weaponIcons[(int)weapon];
		}

		public delegate void SelectWeapon(WeaponList w);

		WeaponList weaponType;
		
		Mesh weaponMesh;
		SelectWeapon selectWeapon;

		Mesh lockMesh = Mesh.OrthoBox;

		Mesh scrapMesh = Mesh.OrthoBox,
			costMesh = Mesh.OrthoBox;

		TextBox costBox;

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

			weaponMesh = Mesh.OrthoBox;
			weaponMesh.Texture = GetWeaponIcon(weapon);

			lockMesh.Texture = Art.Load("Res/weapons/lock.png");
			lockMesh.Color = new Color(0, 0, 0, 0.5f);
			lockMesh.FillColor = true;

			scrapMesh.Texture = Art.Load("Res/scrap.png");

			costBox = new TextBox(120f);
			costBox.SetHeight = size.Y * 0.4f;
			costBox.Text = WeaponStats.GetStats(weapon).scrapCost.ToString();
			costBox.VerticalAlign = TextBox.VerticalAlignment.Center;
			costBox.HorizontalAlign = TextBox.HorizontalAlignment.Left;

			costMesh.Texture = costBox.Texture;
			costMesh.Vertices = costBox.Vertices;
			costMesh.UV = costBox.UV;
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
				weaponMesh.Color = Color.White;
				weaponMesh.FillColor = false;
			} else
			{
				weaponMesh.Color = Color.Black;
				weaponMesh.FillColor = true;
			}

			weaponMesh.Reset();

			weaponMesh.Translate(position);
			weaponMesh.Scale(size);
			weaponMesh.Rotate(10f + Rotation);

			weaponMesh.Draw();

			if (!map.LocalPlayer.OwnsWeapon(weaponType))
			{
				lockMesh.Reset();

				lockMesh.Translate(position);
				lockMesh.Scale(size);
				lockMesh.Rotate(Rotation);

				lockMesh.Draw();


				scrapMesh.Reset();

				scrapMesh.Translate(position);
				scrapMesh.Translate(-size.X * 0.3f, -size.Y * 0.4f);
				scrapMesh.Scale(size * 0.4f);
				scrapMesh.Rotate(20f + Rotation);

				scrapMesh.Draw();


				costMesh.Reset();

				costMesh.Translate(position);
				costMesh.Translate(-size.X * 0.3f, -size.Y * 0.4f);
				costMesh.Rotate(Rotation);

				costMesh.Draw();
			}
		}
	}
}