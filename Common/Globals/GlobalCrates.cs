using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Common.Configs;

namespace WeaponEnchantments.Common.Globals
{
    public class GlobalCrates : GlobalItem
    {
        public static SortedDictionary<int, List<WeightedPair>> crateDrops = new SortedDictionary<int, List<WeightedPair>>();
        /*
        // No need for premature optimizations - the VM has no idea what an "enum" is, every enum is just stored as its underlying type (usually int).
        public static SortedDictionary<int, ChestID> crateToChestIDs = new SortedDictionary<int, ChestID>() {
				// Basic
                { ItemID.WoodenCrate, ChestID.Chest },
                { ItemID.WoodenCrateHard, ChestID.Chest },
                { ItemID.IronCrate, ChestID.LivingWoodChest }, // No chest corresponding to living chest.
                { ItemID.IronCrateHard, ChestID.LivingWoodChest },
                { ItemID.GoldenCrate, ChestID.GoldChest }, // There's actually no crate dropping normal golden chest loot - obsidian crate drops things from lava-specific chests, but that's it.
                { ItemID.GoldenCrateHard, ChestID.GoldChest }, // Normally it's not an issue since golden chest loot is, frankly, everywhere and you can fish for multiple variants of the most useful item. This is more for compatibility with old worlds.

                { ItemID.LockBox, ChestID.GoldChestLocked }, // Lockboxes: no hardmode variant
                { ItemID.ObsidianLockbox, ChestID.ShadowChest }, // Would be nice to have biome lockboxes, but out of scope.

                { ItemID.JungleFishingCrate, ChestID.IvyChest },
                { ItemID.JungleFishingCrateHard, ChestID.IvyChest },
                { ItemID.FrozenCrate, ChestID.FrozenChest },
                { ItemID.FrozenCrateHard, ChestID.FrozenChest },
                { ItemID.FloatingIslandFishingCrate, ChestID.SkywareChest },
                { ItemID.FloatingIslandFishingCrateHard, ChestID.SkywareChest },
                { ItemID.OasisCrate, ChestID.SandStoneChest },
                { ItemID.OasisCrateHard, ChestID.SandStoneChest },
                { ItemID.OceanCrate, ChestID.WaterChest },
                { ItemID.OceanCrateHard, ChestID.WaterChest }
		};
        */
		//private const int epsilon = 1000;
		
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot) {

            //if (!crateToChestIDs.ContainsKey(item.type))
            //    return;

            #region Debug

            if (LogMethods.debugging && item.ModItem != null) {
                string crateName = item.ModItem.Name;
                crateName.Log();
            }

            #endregion

            //ChestID chestID = crateToChestIDs[item.type];

            //WEModSystem.GetChestLoot(chestID, out List<int> itemDrops, out float chance);

            if (crateDrops.ContainsKey(item.type)) {
                float chance = GetCrateEnchantmentDropChance(item.type);

                itemLoot.Add(new OneFromWeightedOptionsNotScaledWithLuckDropRule(chance, crateDrops[item.type]));
            }
            
            //itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuckWithX(epsilon, (int)(chance * epsilon), itemDrops.ToArray()));
        }

        public static float GetCrateEnchantmentDropChance(int id) {
            float chance;
			switch (id) {
                case ItemID.WoodenCrate:
                case ItemID.IronCrate:
                case ItemID.GoldenCrate:
                    chance = (float)(id - ItemID.WoodenCrate + 1f) / 20f;
                    break;
                case ItemID.WoodenCrateHard:
                case ItemID.IronCrateHard:
                case ItemID.GoldenCrateHard:
                    chance = (float)(id - ItemID.WoodenCrateHard + 1f) / 10f;
                    break;
                case ItemID.LockBox:
                    chance = 0.5f;
                    break;
                case ItemID.ObsidianLockbox:
                    chance = 1f;
                    break;
                case < ItemID.WoodenCrateHard:
                case ItemID.FrozenCrate:
                case ItemID.OasisCrate:
                case ItemID.LavaCrate:
                case ItemID.OceanCrate:
                    chance = 0.15f;
                    break;
                default:
                    chance = 0.25f;
                    break;
			}

            chance *= ConfigValues.CrateDropChance;

            if (crateDrops[id].Count == 1)
                chance *= crateDrops[id][0].Weight;

            return chance;
        }
    }
}
