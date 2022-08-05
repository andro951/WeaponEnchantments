using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.UI;

namespace WeaponEnchantments.Common.Globals
{
	public class AllItemsGlobalItem : GlobalItem
	{
		public override bool InstancePerEntity => true;

		public override bool CanUseItem(Item item, Player player) {
			WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();

			//Prevent using items when hoving over enchanting table ui
			if (wePlayer.usingEnchantingTable && WeaponEnchantmentUI.preventItemUse)
				return false;

			return true;
		}
	}
}
