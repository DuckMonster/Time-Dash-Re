using OpenTK;

namespace ShopMenu
{
	public class EquipButton : Button
	{
		public delegate void EquipWeapon(int inventoryID);

		int inventoryID;
		EquipWeapon onEquip;

		public EquipButton(int inventoryID, EquipWeapon onEquip, Vector2 position, Vector2 size, ShopMenu menu, Map map)
			: base("", position, size, menu, map)
		{
			this.onEquip = onEquip;
			this.inventoryID = inventoryID;

			textBox.Text = "Equip\n" + (inventoryID == 0 ? "A" : "B");

		}

		protected override void OnClicked()
		{
			onEquip(inventoryID);
		}
	}
}