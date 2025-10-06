using androLib.Common.Utility;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Globals
{
	public class EnchantmentEssenceGlobal : GlobalItem {
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
			return !entity.NullOrAir() && entity.ModItem != null && entity.ModItem is EnchantmentEssence || entity.type == ModContent.ItemType<CursedEssence>();
		}

		public override void GrabRange(Item item, Player player, ref int grabRange) {
			grabRange *= WEMod.serverConfig.EssenceGrabRange;
		}
	}
}
