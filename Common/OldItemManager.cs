using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using static WeaponEnchantments.Common.EnchantingRarity;

namespace WeaponEnchantments.Common
{
    public class OldItemManager
    {
        private enum OldItemContext {
            firstWordNames,
            searchWordNames,
            wholeNameReplaceWithItem,
            wholeNameReplaceWithCoins
        }
        private static Dictionary<string, string> firstWordNames = new Dictionary<string, string> { 
            { "Critical", "CriticalStrikeChance" }, 
            { "Size", "Scale" }, 
            { "ManaCost", "Mana" }, 
            { "StatDefense", "Defense" }, 
            { "Splitting", "Multishot"} };
        private static Dictionary<string, int> searchWordNames = new Dictionary<string, int> { 
            { "SuperRare", 3 }, 
            { "UltraRare", 4 }, 
            { "Rare", 2 } };
        private static List<string> firstWordReplaceEnchantmentWithCoins = new List<string>() {
            { "PhaseJump" }
		};
        private static Dictionary<string, int> firstWordReplaceEnchantmentWithItem = new Dictionary<string, int>() {
            { "CatastrophicRelease", ItemID.None }
        };
        private static Dictionary<string, int> wholeNameReplaceWithItem = new Dictionary<string, int> { 
            { "ContainmentFragment", ItemID.GoldBar }, 
            {"Stabilizer", 177}, 
            {"SuperiorStabilizer", 999} };
        private static Dictionary<string, int> wholeNameReplaceWithCoins = new Dictionary<string, int>() {
            
		};
        public static void ReplaceAllOldItems() {

			#region Debug

			if (LogMethods.debugging) ($"\\/ReplaceAllOldItems()").Log();

            #endregion

            int i = 0;
            foreach (Chest chest in Main.chest) {
                if (chest != null) {
                    if(LogMethods.debugging) ($"chest: {i}").Log();
                    ReplaceOldItems(chest.item);
                }
                i++;
            }

            #region Debug

            if (LogMethods.debugging) ($"/\\ReplaceAllOldItems()").Log();

            #endregion
        }
        public static void ReplaceAllPlayerOldItems(Player player) {

			#region Debug

			if (LogMethods.debugging) ($"\\/ReplaceAllPlayerOldItems(player: {player.S()})").Log();

            #endregion

            //"armor".Log();
            ReplaceOldItems(player.GetWEPlayer().GetEquipArmor(true), player, 91);
            //"inventory".Log();
            ReplaceOldItems(player.inventory, player);
            //"bank1".Log();
            ReplaceOldItems(player.bank.item, player, 50, -2);
            //"bank2".Log();
            ReplaceOldItems(player.bank2.item, player, 50, -3);
            //"bank3".Log();
            ReplaceOldItems(player.bank3.item, player, 50, -4);
            //"bank4".Log();
            ReplaceOldItems(player.bank4.item, player, 50, -5);

			#region Debug

			if (LogMethods.debugging) ($"/\\ReplaceAllPlayerOldItems(player: {player.S()})").Log();

			#endregion
		}
		private static void ReplaceOldItems(Item[] inventory, Player player = null, int itemSlotNumber = 0, int bank = -1) {

            #region Debug

            if (LogMethods.debugging) ($"\\/ReplaceOldItems(inventory, player: {player.S()}, itemSlotNumber: {itemSlotNumber}, bank: {bank})").Log();

			#endregion

			for (int i = 0; i < inventory.Length; i++) {
                 ReplaceOldItem(ref inventory[i], player, itemSlotNumber + i, bank);
            }

            #region Debug

            if (LogMethods.debugging) ($"/\\ReplaceOldItems(inventory, player: {player.S()}, itemSlotNumber: {itemSlotNumber}, bank: {bank})").Log();

			#endregion
		}
		public static void ReplaceOldItem(ref Item item, Player player = null, int itemSlotNumber = 0, int bank = -1) {
            if(item != null && !item.IsAir) {

                #region Debug

                if (LogMethods.debugging) ($"\\/ReplaceOldItem(item: {item.S()}, player: {player.S()}, itemSlotNumber: {itemSlotNumber}, bank: {bank})").Log();

				#endregion

				if (item.ModItem is UnloadedItem) {
                    bool replaced = false;
                    if (!replaced)
                        replaced = TryReplaceEnchantmentWithItem(ref item);

                    if (!replaced)
                        replaced = TryReplaceEnchantmentWithCoins(ref item);

                    if (!replaced)
                        replaced = TryReplaceItem(ref item, firstWordNames, OldItemContext.firstWordNames);

                    if (!replaced)
                        replaced = TryReplaceItem(ref item, searchWordNames, OldItemContext.searchWordNames);

                    if (!replaced)
                        replaced = TryReplaceItem(ref item, wholeNameReplaceWithItem, OldItemContext.wholeNameReplaceWithItem);

                    if (!replaced)
                        TryReplaceItem(ref item, wholeNameReplaceWithCoins, OldItemContext.wholeNameReplaceWithCoins);
                }

                if (item.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                    for (int i = 0; i < EnchantingTable.maxEnchantments; i++) {
                        Item enchantmentItem = iGlobal.enchantments[i];
                        if (enchantmentItem.ModItem is UnloadedItem) {
                            ReplaceOldItem(ref enchantmentItem, player);
                        }
                    }

                    if (player != null) {
                        item.CheckRemoveEnchantments(player);

                        item.SetupGlobals();
                    }
                }

                #region Debug

                if (LogMethods.debugging) ($"/\\ReplaceOldItem(item: {item.S()}, player: {player.S()}, itemSlotNumber: {itemSlotNumber}, bank: {bank})").Log();

				#endregion
			}
		}
        private static bool TryReplaceItem(ref Item item, Dictionary<string, int> dict, OldItemContext context) {
            string name = ((UnloadedItem)item.ModItem).ItemName;
            string key = null;
            foreach (string k in dict.Keys) {
                switch (context) {
                    case OldItemContext.searchWordNames://Not tested
                        int index = name.IndexOf(k);
                        if (index > -1) {
                            key = k;
                            name = name.Substring(0, index - 1) + tierNames[dict[key]] + name.Substring(index);
                        }

                        break;
                    default:
                        if(k == name)
                            key = k;

                        break;
                }

                if (key != null)
                    break;
            }

            //firstWordNames
            if (key != null) {
                switch (context) {
                    case OldItemContext.searchWordNames://Not tested
                        foreach (ModItem modItem in ModContent.GetInstance<WEMod>().GetContent<ModItem>()) {
                            if (modItem is Enchantment enchantment) {
                                if(enchantment.Name == name) {
                                    ReplaceItem(ref item, enchantment.Item.type);

                                    return true;
                                }
                            }
                        }

                        break;
                    case OldItemContext.wholeNameReplaceWithItem:
                        ReplaceItem(ref item, dict[key]);

                        return true;
                    case OldItemContext.wholeNameReplaceWithCoins:
                        ReplaceItem(ref item, dict[key], true);

                        return true;
                }
            }

            return false;
        }
        private static bool TryReplaceItem(ref Item item, Dictionary<string, string> dict, OldItemContext context) {
            string name = ((UnloadedItem)item.ModItem).ItemName;
            string key = null;
            foreach (string k in dict.Keys) {
                switch (context) {
                    case OldItemContext.firstWordNames:
                        if (name.Length >= k.Length) {
                            string keyCheck = name.Substring(0, k.Length);
                            if (keyCheck == k) {
                                key = k;
                            }
                        }

                        break;
                }

                if (key != null)
                    break;
            }

            //firstWordNames
            if (key != null) {
                switch (context) {
                    case OldItemContext.firstWordNames:
                        foreach (ModItem modItem in ModContent.GetInstance<WEMod>().GetContent<ModItem>()) {
                            if (modItem is Enchantment enchantment) {
                                if (enchantment.EnchantmentTypeName == dict[key]) {
                                    int typeOffset = GetTierNumberFromName(name);
                                    ReplaceItem(ref item, enchantment.Item.type + typeOffset);

                                    return true;
                                }
                            }
                        }

                        break;
                }
            }

            return false;
        }
        private static bool TryReplaceEnchantmentWithItem(ref Item item) {
            string unloadedItemName = ((UnloadedItem)item.ModItem).ItemName;
            string key = null;
            foreach(string replaceItemName in firstWordReplaceEnchantmentWithItem.Keys) {
                if(unloadedItemName.Length >= replaceItemName.Length) {
                    int index = unloadedItemName.IndexOf(replaceItemName);
                    if (index > -1) {
                        key = replaceItemName;
                        break;
                    }
                }
            }

            if (key == null)
                return false;

            item = new Item();
            int newItemType = firstWordReplaceEnchantmentWithItem[key];
            if (newItemType == 0)
                return true;

            Item itemToSpawn = new Item(newItemType);
            int valueItemToSpawn = itemToSpawn.value;
            int valueEnchantment = GetEnchantmentValueByName(unloadedItemName);
            int stack = valueEnchantment / valueItemToSpawn;
            if (stack <= 0) {
                Main.NewText($"{unloadedItemName} has been removed from Weapon Enchantments.");
                return true;
            }

            Main.NewText($"{unloadedItemName} has been removed from Weapon Enchantments.  You've recieved {itemToSpawn.S()} as compensation.");
            Main.LocalPlayer.QuickSpawnClonedItem(null, itemToSpawn, stack);

            return true;
        }
        private static bool TryReplaceEnchantmentWithCoins(ref Item item) {
            string unloadedItemName = ((UnloadedItem)item.ModItem).ItemName;
            string key = null;
            foreach (string replaceItemName in firstWordReplaceEnchantmentWithCoins) {
                if (unloadedItemName.Length >= replaceItemName.Length) {
                    int index = unloadedItemName.IndexOf(replaceItemName);
                    if (index > -1) {
                        key = replaceItemName;
                        break;
                    }
                }
            }

            if (key == null)
                return false;

            int value = GetEnchantmentValueByName(unloadedItemName);
            if(value > 0) {
                ReplaceItem(ref item, value, true);
            }
			else {
                $"Failed to replace item: {item.S()} with coins".LogNT(ChatMessagesIDs.FailedGetEnchantmentValueByName);
			}

            return true;
        }
        public static void ReplaceItem(ref Item item, int type, bool replaceWithCoins = false, bool sellPrice = true) {
            string unloadedItemName = ((UnloadedItem)item.ModItem).ItemName;
            int stack = item.stack;
            if(type == 999) {
                stack = stack / 4 + (stack % 4 > 0 ? 1 : 0);
            }

            item.TurnToAir();
            if (replaceWithCoins) {
                int total = type * stack;
                if (sellPrice)
                    total /= 5;

                //type is coins when replaceWithCoins is true
                UtilityMethods.ReplaceItemWithCoins(ref item, total);

                ($"{unloadedItemName} has been removed from Weapon Enchantments.  You have recieved Coins equal to its sell price.").Log();
            }
            else {
                item = new Item(type, stack);
                ($"{unloadedItemName} has been removed from Weapon Enchantments.  It has been replaced with {ContentSamples.ItemsByType[type].S()}").Log();
            }
        }
        private static int GetEnchantmentValueByName(string name) {
            int tier = GetTierNumberFromName(name);
            int damageEnchantmentBasicType = ModContent.ItemType<DamageEnchantmentBasic>();
            int value = ContentSamples.ItemsByType[damageEnchantmentBasicType + tier].value;

            return value;
        }
    }
}
