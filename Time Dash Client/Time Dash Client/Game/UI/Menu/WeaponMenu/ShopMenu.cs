using OpenTK;
using System;
using TKTools;

namespace ShopMenu
{
	public class ShopMenu : Menu
	{
		public WeaponList? selectedWeapon = null;
		Mesh weaponMesh = Mesh.OrthoBox;
		float weaponRotation = 0f;

		BuyButton buyButton;

		Mesh lockMesh = Mesh.OrthoBox,
			scrapMesh = Mesh.OrthoBox,
			costMesh = Mesh.OrthoBox;

		TextBox costBox;

		Vector2 previewWeaponPosition = new Vector2(4, 2);

		public ShopMenu(Map map)
			: base(new Vector2(18f, 10f), map)
		{
			int nmbrOfWeapons = Enum.GetNames(typeof(WeaponList)).Length;

			float padding = 0.2f;

			Vector2 buttonSize = new Vector2(1.25f, 1.25f);
			Vector2 buttonStart = (size / 2 - buttonSize / 2 - new Vector2(padding, padding)) * new Vector2(-1, 1);

			for (int i=0; i<nmbrOfWeapons; i++)
			{
				float x = (i % 5) * (buttonSize.X + padding);
				float y = (i / 5) * (-buttonSize.Y - padding);

				AddButton(new WeaponButton(
					(WeaponList)(i % nmbrOfWeapons),
					(w) => { selectedWeapon = w; costBox.Text = WeaponStats.GetStats(w).scrapCost.ToString(); } ,
					buttonStart + new Vector2(x, y), buttonSize, 
					this, map));
			}

			buyButton = new BuyButton(() => map.LocalPlayer.SendBuyWeapon(selectedWeapon.Value), previewWeaponPosition + new Vector2(0, -4), new Vector2(3f, 1f), this, map);

			lockMesh.Texture = Art.Load("Res/weapons/lock.png");
			lockMesh.FillColor = true;
			lockMesh.Color = new Color(0f, 0f, 0f, 0.5f);

			scrapMesh.Texture = Art.Load("Res/scrap.png");

			costBox = new TextBox(new System.Drawing.Font("Adobe Song Std L", 200f));
			costBox.VerticalAlign = TextBox.VerticalAlignment.Center;
			costBox.UpdateRate = 0;
			costBox.SetHeight = 3f;
        }

		public override void Logic()
		{
			base.Logic();

			if (selectedWeapon != null)
				buyButton.Logic();

			weaponRotation += 40f * Game.delta;
		}

		public override void Draw()
		{
			base.Draw();

			if (selectedWeapon != null)
			{
				weaponMesh.Texture = WeaponButton.GetWeaponIcon(selectedWeapon.Value);
				if (map.LocalPlayer.OwnsWeapon(selectedWeapon.Value))
				{
					weaponMesh.FillColor = false;
					weaponMesh.Color = Color.White;
				} else
				{
					weaponMesh.FillColor = true;
					weaponMesh.Color = Color.Black;
				}

				weaponMesh.Reset();

				weaponMesh.Translate(previewWeaponPosition);
				weaponMesh.Scale(5f);
				weaponMesh.Rotate(weaponRotation);

				weaponMesh.Draw();

				if (!map.LocalPlayer.OwnsWeapon(selectedWeapon.Value))
				{
					lockMesh.Reset();

					lockMesh.Translate(previewWeaponPosition);
					lockMesh.Scale(3f);

					lockMesh.Draw();

					scrapMesh.Reset();

					scrapMesh.Translate(previewWeaponPosition);
					scrapMesh.Translate(-0.5f, -0.2f);
					scrapMesh.Rotate(10f);
					scrapMesh.Scale(1.2f);

					scrapMesh.Draw();

					costMesh.Texture = costBox.Texture;
					costMesh.UV = costBox.UV;
					costMesh.Vertices = costBox.Vertices;

					costMesh.Reset();

					costMesh.Translate(previewWeaponPosition);
					costMesh.Translate(-0.3f, 0f);

					costMesh.Draw();

					buyButton.Draw();
				}
			}
		}
	}
}