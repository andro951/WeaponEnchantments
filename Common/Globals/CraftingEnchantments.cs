using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common.Globals
{
	public class CraftingEnchantments : GlobalItem
	{
		public override bool InstancePerEntity => true;

		public override bool AppliesToEntity(Item entity, bool lateInstantiation) {
            //Bars created from uncrafting Containments
			switch (entity.type) {
                case ItemID.SilverBar:
                case ItemID.TungstenBar:
                case ItemID.GoldBar:
                case ItemID.PlatinumBar:
                case ItemID.DemoniteBar:
                case ItemID.CrimtaneBar:
                    return true;
			}

			if(entity.ModItem == null)
                return false;

            ModItem modItem = entity.ModItem;
            return modItem is Enchantment;
		}

		public override void OnCreate(Item item, ItemCreationContext context) {
            if(context is RecipeCreationContext recipeCreationContext) {
                foreach (Item consumedItem in recipeCreationContext.ConsumedItems) {
                    if (consumedItem.ModItem is Enchantment) {
                        int size = ((Enchantment)consumedItem.ModItem).EnchantmentTier;
                        if (size < 2) {
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ContainmentItem.IDs[size], 1);
                        }
                        else if (size == 3) {
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ItemID.Topaz, 2);
                        }
                    }
                    else if (consumedItem.ModItem is ContainmentItem containment) {
                        if (containment.size == 2 && item.type == ContainmentItem.barIDs[0, 2])
                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ItemID.Topaz, 4);
                    }
                }
            }
        }
	}
}
