using androLib;
using MagicStorage;
using MagicStorage.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.Tiles;
using WeaponEnchantments.UI;

namespace WeaponEnchantments.ModIntegration
{
    [ExtendsFromMod(AndroMod.magicStorageName)]
    public class WEEnvironmentModule : EnvironmentModule
    {
        public override string Name => "Enchanting Table Tile (Andros Storage for items)";
		public override void ModifyCraftingZones(EnvironmentSandbox sandbox, ref CraftingInformation information) {
            int highestTableTierUsed = Main.LocalPlayer.GetWEPlayer().highestTableTierUsed;

            for (int tier = highestTableTierUsed; tier >= 0; tier--) {
				int tableTier = EnchantingTableTile.GetTableTypeByTier(tier);
				information.adjTiles[tableTier] = true;
			}
		}
		public override void OnConsumeItemForRecipe(EnvironmentSandbox sandbox, Item item, int stack) {
            if (item.maxStack > 1 && item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantmentedItem) && enchantmentedItem.Modified) {
				TEStorageHeart tEStorageHeart = global::MagicStorage.StoragePlayer.LocalPlayer.GetStorageHeart();
				IEnumerable<Item> storageItems = tEStorageHeart.GetStoredItems();
                MagicStorageIntegration.JustCraftedStackableItem = item.TryResetSameEnchantedItem(storageItems, out _);

                List<Item> storageItemsList = storageItems.ToList();
                for (int i = 0; i < storageItemsList.Count; i++) {
                    for (int o = i + 1; o < storageItemsList.Count; o++) {
                        Item item1 = storageItemsList[i];
                        Item item2 = storageItemsList[o];
                        if (item1.type != item2.type || item1.maxStack <= 1 || !item1.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem1) || !enchantedItem1.Modified)
                            continue;

                        if (!item1.IsSameEnchantedItem(item2))
                            continue;

                        if (!item2.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem2))
                            continue;

                        if (item1.stack == 1) {
                            enchantedItem1.ResetGlobals(item1);
                        }
                        else if (item2.stack == 1) {
                            enchantedItem2.ResetGlobals(item2);
                        }
                    }
                }
            }
		}
	}
}
