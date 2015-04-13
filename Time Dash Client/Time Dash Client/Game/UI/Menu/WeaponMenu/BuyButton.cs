using OpenTK;
using TKTools;

namespace ShopMenu
{
	public class BuyButton : Button
	{
		public delegate void OnBuy();

		OnBuy onBuy;

		public override bool Enabled
		{
			get
			{
				if ((menu as ShopMenu).selectedWeapon == null) return false;
				return ((map.LocalPlayer as SYPlayer).CollectedScrap >= WeaponStats.GetStats((menu as ShopMenu).selectedWeapon.Value).scrapCost);
			}
		}

		public BuyButton(OnBuy onBuy, Vector2 position, Vector2 size, Menu menu, Map map)
			: base("Buy", position, size, menu, map)
		{
			this.onBuy = onBuy;
			textBox.UpdateRate = 0;
		}

		protected override Color Color
		{
			get
			{
				if (Enabled) return base.Color;
				else return new Color(0, 0, 0, 0.2f);
			}
		}

		protected override void OnClicked()
		{
			onBuy();
		}

		public override void Logic()
		{
			base.Logic();

			textBox.Text = Enabled ? "Buy" : "Not enough scrap";
		}
	}
}