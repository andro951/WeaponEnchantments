﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.Items.Enchantments;
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using androLib.Common.Utility;
using androLib;
using VacuumOreBag.Items;

namespace WeaponEnchantments.Common
{
    public class OldItemManager
    {
        public static byte versionUpdate;
        private enum OldItemContext {
            firstWordNames,
            searchWordNames,
            wholeNameReplaceWithItem,
            wholeNameReplaceWithCoins
        }
        private static Dictionary<string, string> firstWordNames = new Dictionary<string, string> { 
            { "Critical", "CriticalStrikeChance" }, 
            { "Scale", "Size" }, 
            { "ManaCost", "ReducedManaUsage" },
            { "Mana", "ReducedManaUsage" }, 
            { "StatDefense", "Defense" },
            { "Splitting", "Multishot"},
            { "ShootSpeed", "ProjectileVelocity"},
            { "Speed", "AttackSpeed" },
            { "Control", "MobilityControl" },
            { "MoveSpeed", "MovementSpeed" },
	        { "PhaseJump", "SolarDash" },
            { "ArmorPenetration", "PercentArmorPenetration" },
			{ "ShadowFlame", "Shadowflame" }
		};
        private static Dictionary<string, int> searchWordNames = new Dictionary<string, int> {
            { "SuperRare", 3 },
            { "UltraRare", 4 }
        };
        private static List<string> firstWordReplaceEnchantmentWithCoins = new List<string>() {
            
		};
        private static Dictionary<string, int> firstWordReplaceEnchantmentWithItem = new Dictionary<string, int>() {
            { "CatastrophicRelease", ItemID.None }
        };
        private static Dictionary<string, int> wholeNameReplaceWithItem = new Dictionary<string, int> { 
            { "ContainmentFragment", ItemID.GoldBar }, 
            { "Stabilizer", 177 }, 
            { "SuperiorStabilizer", 999 },
            { "OreBag", ModContent.ItemType<OreBag>() }
        };
        private static Dictionary<string, int> wholeNameReplaceWithCoins = new Dictionary<string, int>() {
            
		};
        public static void ReplaceAllOldItems() {

			#region Debug

			if (LogMethods.debugging) ($"\\/ReplaceAllOldItems()").Log_WE();

            #endregion

            int i = 0;
            foreach (Chest chest in Main.chest) {
                if (chest != null) {
                    if(LogMethods.debugging) ($"chest: {i}").Log_WE();
                    ReplaceOldItems(chest.item);
                }
                i++;
            }

            #region Debug

            if (LogMethods.debugging) ($"/\\ReplaceAllOldItems()").Log_WE();

            #endregion
        }
        public static void ReplaceAllPlayerOldItems(Player player) {

			#region Debug

			if (LogMethods.debugging) ($"\\/ReplaceAllPlayerOldItems(player: {player.S()})").Log_WE();

            #endregion

            //"armor".Log();
            //ReplaceOldItems(player.GetWEPlayer().GetEquipArmor(true), player, 91);
            ReplaceOldItems(player.armor, player, 91);

            int modSlotCount = player.GetModPlayer<ModAccessorySlotPlayer>().SlotCount;
            var loader = LoaderManager.Get<AccessorySlotLoader>();
            for (int num = 0; num < modSlotCount; num++) {
                if (loader.ModdedIsItemSlotUnlockedAndUsable(num, player)) {
                    Item accessoryClone = loader.Get(num).FunctionalItem.Clone();
                    if (!accessoryClone.NullOrAir()) {
                        ReplaceOldItem(ref accessoryClone, player);
                        loader.Get(num).FunctionalItem = accessoryClone;
				    }

                    Item vanityClone = loader.Get(num).VanityItem.Clone();
                    if (!vanityClone.NullOrAir()) {
                        ReplaceOldItem(ref vanityClone, player);
                        loader.Get(num).VanityItem = vanityClone;
				    }
                }
            }

            //"inventory".Log();
            ReplaceOldItems(player.inventory, player);

            //"bank1".Log();
            ReplaceOldItems(player.bank.item, player);

            //"bank2".Log();
            ReplaceOldItems(player.bank2.item, player);

            //"bank3".Log();
            ReplaceOldItems(player.bank3.item, player);

            //"bank4".Log();
            ReplaceOldItems(player.bank4.item, player);

            if (player.TryGetWEPlayer(out WEPlayer wePlayer)) {
                foreach (Item[] storageInventory in StorageManager.AllItems) {
                    ReplaceOldItems(storageInventory, player);
                }

				ReplaceOldItems(wePlayer.enchantingTableEssence, player);
				ReplaceOldItems(wePlayer.enchantmentStorageItems, player);
			}

			#region Debug

			if (LogMethods.debugging) ($"/\\ReplaceAllPlayerOldItems(player: {player.S()})").Log_WE();

			#endregion
		}
		private static void ReplaceOldItems(Item[] inventory, Player player = null, int itemSlotNumber = 0) {
            if (inventory == null)
                return;

            #region Debug

            if (LogMethods.debugging) ($"\\/ReplaceOldItems(inventory, player: {player.S()}, itemSlotNumber: {itemSlotNumber})").Log_WE();

			#endregion

			for (int i = 0; i < inventory.Length; i++) {
                 ReplaceOldItem(ref inventory[i], player);
            }

            #region Debug

            if (LogMethods.debugging) ($"/\\ReplaceOldItems(inventory, player: {player.S()}, itemSlotNumber: {itemSlotNumber})").Log_WE();

			#endregion
		}
		public static void ReplaceOldItem(ref Item item, Player player = null) {
            if (item != null && !item.IsAir) {

                #region Debug

                if (LogMethods.debugging) ($"\\/ReplaceOldItem(item: {item.S()}, player: {player.S()})").Log_WE();

				#endregion

				if (item.ModItem is UnloadedItem unloadedItem) {
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

                //Transfer and delete EnchantedItem data
                if (versionUpdate < 1) {
                    FieldInfo fieldInfo = typeof(Item).GetField("_globals", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo dataFieldInfo = typeof(UnloadedGlobalItem).GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
                    string modName = "WeaponEnchantments";
                    string className = "EnchantedItem";

                    if (fieldInfo.GetValue(item) is GlobalItem[] globalItems && globalItems.Length != 0) {
                        if (item.TryGetEnchantedItemSearchAll(out EnchantedItem foundEnchantedItem)) {
                            foreach (GlobalItem g in globalItems.Where(i => i is UnloadedGlobalItem)) {
                                if (dataFieldInfo.GetValue(g) is IList<TagCompound> tagList) {
                                    foreach (TagCompound tagCompound in tagList) {
                                        string mod = tagCompound.Get<string>("mod");
                                        if (mod == modName) {
                                            string name = tagCompound.Get<string>("name");
                                            if (name == className) {
                                                TagCompound dataTag = tagCompound.Get<TagCompound>("data");
                                                foundEnchantedItem.LoadData(item, dataTag);
                                                foundEnchantedItem.SaveData(item, dataTag);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (fieldInfo.GetValue(item) is GlobalItem[] newGlobalItemsArray) {
                            List<GlobalItem> newGlobalItems = newGlobalItemsArray.ToList();
                            int count = newGlobalItems.Count;
                            for (int i = newGlobalItems.Count - 1; i >= 0; i--) {
                                if (newGlobalItems[i] is UnloadedGlobalItem unloadedGlobalItem) {
                                    if (dataFieldInfo.GetValue(unloadedGlobalItem) is IList<TagCompound> unloadedTagList) {
                                        foreach (TagCompound tagCompound in unloadedTagList) {
                                            //$"item: {item.S()}, tagCompound: {tagCompound}".Log();
                                            string mod = tagCompound.Get<string>("mod");
                                            if (mod == modName) {
                                                string name = tagCompound.Get<string>("name");
                                                if (name == className) {
                                                    newGlobalItems.RemoveAt(i);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (newGlobalItems.Count < count) {
                                fieldInfo.SetValue(item, newGlobalItems.ToArray());
								GameMessageTextID.RemovedEnchantedItemData.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S(), count, newGlobalItems.Count }).Log_WE();// $"Removed EnchantedItem data from item: {item.S()}, count: {count}, newCount: {newGlobalItems.Count}".Log_WE();
                            }
                        }
                    }
                }

                if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
                    for (int i = 0; i < EnchantingTableUI.MaxEnchantmentSlots; i++) {
                        Item enchantmentItem = enchantedItem.enchantments[i];
                        if (enchantmentItem.ModItem is UnloadedItem) {
                            ReplaceOldItem(ref enchantmentItem, player);
                        }
                    }

                    if (player != null)
                        item.CheckRemoveEnchantments(player);

                    enchantedItem.SetupGlobals(item);
                }

                #region Debug

                if (LogMethods.debugging) ($"/\\ReplaceOldItem(item: {item.S()}, player: {player.S()})").Log_WE();

				#endregion
			}
		}
        private static bool TryReplaceItem(ref Item item, Dictionary<string, int> dict, OldItemContext context) {
            string name = ((UnloadedItem)item.ModItem).ItemName;
            string key = null;
            foreach (string k in dict.Keys) {
                switch (context) {
                    case OldItemContext.searchWordNames:
                        int index = name.IndexOf(k);
                        if (index > -1) {
                            key = name.Substring(0, index) + tierNames[dict[k]];
                            int afterIndex = index + k.Length - 1;
                            if (afterIndex < name.Length - 1)
                                key += name.Substring(afterIndex);//Not Tested
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
                    case OldItemContext.searchWordNames:
                        foreach (ModItem modItem in ModContent.GetInstance<WEMod>().GetContent<ModItem>()) {
                            if (modItem.Name == key) {
                                ReplaceItem(ref item, modItem.Item.type);

                                return true;
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
                                    if (typeOffset == -1) {
                                        foreach(string s in searchWordNames.Keys) {
                                            if (name.IndexOf(s) > -1) {
                                                typeOffset = searchWordNames[s];
											}
										}
									}

                                    if (typeOffset > -1) {
                                        ReplaceItem(ref item, enchantment.Item.type + typeOffset);
                                    }
									else {
                                        $"{GameMessageTextID.FailedReplaceOldItem.ToString().Lang_WE(L_ID1.GameMessages)} {name}".LogNT_WE(ChatMessagesIDs.AlwaysShowFailedToReplaceOldItem);
									}

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
                Main.NewText(GameMessageTextID.ItemRemovedFromWeaponEnchantments.ToString().Lang_WE(L_ID1.GameMessages, new object[]{ unloadedItemName }));// $"{unloadedItemName} has been removed from Weapon Enchantments.");
                return true;
            }

            Main.NewText(GameMessageTextID.ItemRemovedReiceveCompensation.ToString().Lang_WE(L_ID1.GameMessages, new object[] { unloadedItemName, itemToSpawn.S() }));//$"{unloadedItemName} has been removed from Weapon Enchantments.  You've recieved {itemToSpawn.S()} as compensation.");
			Main.LocalPlayer.QuickSpawnItem(null, itemToSpawn, stack);

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
				GameMessageTextID.FailedReplaceWithCoins.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S() }).LogNT_WE(ChatMessagesIDs.FailedGetEnchantmentValueByName);//$"Failed to replace item: {item.S()} with coins".LogNT_WE(ChatMessagesIDs.FailedGetEnchantmentValueByName);
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
				androLib.Common.Utility.AndroUtilityMethods.ReplaceItemWithCoins(ref item, total);

                GameMessageTextID.ItemRemovedRecieveCoins.ToString().Lang_WE(L_ID1.GameMessages, new object[] { unloadedItemName }).Log_WE();// ($"{unloadedItemName} has been removed from Weapon Enchantments.  You have recieved Coins equal to its sell price.").Log_WE();
            }
            else {
                item = new Item(type, stack);
				GameMessageTextID.ItemRemovedRelacedWithItem.ToString().Lang_WE(L_ID1.GameMessages, new object[] { unloadedItemName, ContentSamples.ItemsByType[type].S() }).Log_WE();// ($"{unloadedItemName} has been removed from Weapon Enchantments.  It has been replaced with {ContentSamples.ItemsByType[type].S()}").Log_WE();
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
