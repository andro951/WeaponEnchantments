using Microsoft.CodeAnalysis.Operations;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static WeaponEnchantments.Common.Utility.LogModSystem;

namespace WeaponEnchantments.Common
{
    public static class InfusionManager
    {
        public const int numVanillaWeaponRarities = 11;
        public const int numRarities = 15;
        public static float[] averageValues = new float[numRarities];
        public static int[] minValues = new int[numRarities];
        public static int[] maxValues = new int[numRarities];
        public static int[] calamityAverageValues = new int[numRarities];
        public static int[] calamityMinValues = new int[numRarities];
		//                                                 0     1      2      3      4       5       6       7       8       9       10       11       12       13       14       15       16       17
		public static int[] calamityMaxValues = new int[] { 23000, 52000, 87000, 128000, 175000, 240000, 360000, 480000, 600000, 800000, 1000000, 1200000, 1400000, 2000000, 3000000 };
		//public static int[] calamityMaxValues = new int[] {5000, 10000, 20000, 40000, 120000, 240000, 360000, 480000, 600000, 800000, 1000000, 1100000, 1200000, 1300000, 1400000, 1500000, 2000000, 2500000};
		public const float minMaxValueMultiplier = 0.25f;

        public static void SetUpVanillaWeaponInfusionPowers() {
            Dictionary<string, List<int[]>> weaponsDict = GetItemDict(GetItemDictModeID.Weapon);
            int[] total = new int[numRarities];
            int[] count = new int[numRarities];
            
            //For each vanilla item
            foreach (int[] stats in weaponsDict["Terraria"]) {
                int rarity = stats[0];
                rarity.Clamp(0, numRarities - 1);

                int value = stats[1];
                total[rarity] += value;
                count[rarity]++;

                //Min value
                if(minValues[rarity] > value || minValues[rarity] == 0)
                    minValues[rarity] = value;

                //Max value
                if(maxValues[rarity] < value)
                    maxValues[rarity] = value;
            }

            //Rarity 0-10 averages
            for (int i = 0; i < numRarities; i++) {
                if (i < numVanillaWeaponRarities)
                    averageValues[i] = (float)total[i] / (float)count[i];
            }

            //Rarities 11-17
            for (int i = numVanillaWeaponRarities; i < numRarities; i++) {
                if (i >= 16) {
                    maxValues[i] = 2000000 + 500000 * (i - 16);
                }
                else {
                    maxValues[i] = 1100000 + 100000 * (i - numVanillaWeaponRarities);
                }

                minValues[i] = maxValues[i - 1];
                averageValues[i] = (minValues[i] + maxValues[i]) / 2;
            }

            //Calamity min and max (used for all modded items)
            for (int i = 0; i < numRarities; i++) {
                if (i == 0) {
                    calamityMinValues[i] = 0;
                }
				else {
                    calamityMinValues[i] = calamityMaxValues[i - 1];
                }

                calamityAverageValues[i] = (calamityMinValues[i] + calamityMaxValues[i]) / 2;
			}
        }
        public static void LogAllInfusionPowers() {
			//Print list of items
			if (PrintListOfItems[GetItemDictModeID.Weapon]) {
				GetItemDict(GetItemDictModeID.Weapon, postSetupPrintList: true);

				string msg = "\nRarity, Average, Min, Max";
				for (int i = 0; i < numRarities; i++) {
					msg += $"\n{i}, {averageValues[i]}, {minValues[i]}, {maxValues[i]}";
				}

				msg.Log();
			}
		}
        private struct ItemDetails {
            public Item Item;
            public float Rarity;
            public float ValueRarity;
            public ItemDetails(Item item, float rarity, float valueRarity) {
                Item = item;
                Rarity = rarity;
                ValueRarity = valueRarity;
			}
		}
        private static Dictionary<string, List<int[]>> GetItemDict(byte mode, bool postSetupPrintList = false) {
            bool printList = PrintListOfItems[mode];

            Dictionary<string, List<int[]>> itemsDict = new Dictionary<string, List<int[]>>();
            SortedDictionary<int, SortedDictionary<string, ItemDetails>> infusionPowers = new SortedDictionary<int, SortedDictionary<string, ItemDetails>>();
            string msg = "";
            for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++) {
                Item item = ContentSamples.ItemsByType[itemType];
                if (item != null) {
                    if (item.netID == ItemID.Count)//Skip April Fools Joke
						continue;

                    string modName = item.ModItem != null ? item.ModItem.Mod.Name : "Terraria";
                    bool weaponList = mode == GetItemDictModeID.Weapon && EnchantedItemStaticMethods.IsWeaponItem(item);
                    bool armorList = mode == GetItemDictModeID.Armor && EnchantedItemStaticMethods.IsArmorItem(item);
                    bool accessory = mode == GetItemDictModeID.Accessory && EnchantedItemStaticMethods.IsAccessoryItem(item);
                    if ( weaponList || armorList || accessory) {
                        int[] itemStats = { item.rare, item.value, item.damage };
                        //if (Debugger.IsAttached && item.rare >= numRarities && modName != "CalamityMod") $"Item above max supported rarities detected: {item.S()}, rare: {item.rare}, value: {item.value}.  It will be treated as rarity {numRarities - 1} for Infusion.".LogSimple();

                        if (!itemsDict.ContainsKey(modName))
                            itemsDict.Add(modName, new List<int[]>());

                        itemsDict[modName].Add(itemStats);

						if (printList && postSetupPrintList) {
                            Item clone = item.Clone();
							float rarity = GetAdjustedItemRarity(clone);
							float valueRarity = GetValueRarity(clone, rarity);
							int infusionPower = clone.GetWeaponInfusionPower();
                            ItemDetails itemDetails = new ItemDetails(clone, rarity, valueRarity);
                            if (!infusionPowers.ContainsKey(infusionPower)) {
                                infusionPowers.Add(infusionPower, new SortedDictionary<string, ItemDetails>() { { clone.Name, itemDetails } });
                            }
							else {
                                if (infusionPowers[infusionPower].ContainsKey(clone.Name)) {
                                    ItemDetails currentItemDetails = infusionPowers[infusionPower][clone.Name];
                                    Item currentItem = currentItemDetails.Item;
									int currentInfusionPower = currentItem.GetWeaponInfusionPower();
									($"infusionPowers[{infusionPower}] already contains key({clone.Name}).\n" +
                                        $"Current = {GetDataString(currentInfusionPower, currentItem.Name, currentItemDetails)}\n" +
                                        $"New = {GetDataString(infusionPower, clone.Name, itemDetails)}").LogSimple();
                                }
                                else {
									infusionPowers[infusionPower].Add(clone.Name, itemDetails);
								}
							}
                        }
                    }
                }
            }

            if(printList && postSetupPrintList) {
                if (mode == GetItemDictModeID.Weapon) {
                    msg += "\nMod, Weapon, Infusion Power, Value Rarity, Rarity, Original Rarity, Value, Item ID, Damage, Use Time, DPS";
                    foreach (int infusionPower in infusionPowers.Keys) {
                        foreach(string name in infusionPowers[infusionPower].Keys) {
                            msg += $"\n{GetDataString(infusionPower, name, infusionPowers[infusionPower][name])}";
                        }
                    }
                }
                    //Print list of items
                    msg.Log();
            }
            
            return itemsDict;
        }
        private static string GetDataString(int infusionPower, string name, ItemDetails itemDetails) {
			Item item = itemDetails.Item;
			string mod = item.ModItem?.Mod.Name;
			if (mod == null)
				mod = "Terraria";

			int damage = item.damage;
			int useTime = item.useTime;
			float dps = (float)damage * 60f / (float)useTime;

			return $"{mod}, {name}, {infusionPower}, {itemDetails.ValueRarity}, {itemDetails.Rarity}, {item.rare}, {item.value}, {item.type}, {damage}, {useTime}, {dps}";
		}
        public static bool UseCalamityValuesOnly(Item item) {
			string sampleItemModName = item.ModItem?.Mod.Name;
			switch (sampleItemModName) {
				case null:
				case "StarsAbove":
					return false;
				default:
					return true;
			}
		}
		private static int GetInfusionPowerFromRarityAndValue(this Item weapon) {
            Item sampleWeapon = ContentSamples.ItemsByType[weapon.type];
            float rarity = GetAdjustedItemRarity(sampleWeapon);
            float valueRarity = GetValueRarity(sampleWeapon, rarity);
            float combinedRarity = rarity + valueRarity;

            return combinedRarity > 0f ? (int)Math.Round(combinedRarity * 100f) : 0;
        }
        public static float GetAdjustedItemRarity(Item sampleItem) {
            bool useCalamityValuesOnly = UseCalamityValuesOnly(sampleItem);
            float rarity = sampleItem.rare;
            int sampleValue = sampleItem.value;
            if (sampleItem.type <= ItemID.Count) {
				if (rarity > numRarities - 1) {
					rarity = numRarities - 1;
				}
				else if (rarity < 0) {
					rarity = 0;
				}
			}
            else {
				if (useCalamityValuesOnly) {
					int i;
					for (i = 0; i < numRarities; i++) {
						float max = calamityMaxValues[i];
						if (max >= sampleValue) {
							float min = calamityMinValues[i];
							if (min >= sampleValue)
								i--;

							break;
						}
					}

					rarity = i;
				}
				else if (rarity >= 11 && sampleItem.value > maxValues[11]) {
					int i;
					for (i = 12; i < numRarities; i++) {
						float min = minValues[i];
						if (min >= sampleItem.value) {
							i--;

							break;
						}
					}

					rarity = i;
				}

				if (rarity > numRarities - 1) {
					rarity = numRarities - 1;
				}
				else if (rarity < 0) {
					rarity = 0;
				}
			}

			switch (sampleItem.Name) {
				case "Primary Zenith"://Primary Zenith
					rarity = 0f;
					break;
			}

			return rarity;
        }
        public static float GetValueRarity(Item sampleItem, float rarity, bool usingBaseRarity = false) {
            int sampleValue = sampleItem.value;
            float valueMultiplier = 0.5f;
            bool useCalamityValuesOnly = UseCalamityValuesOnly(sampleItem);

			int rarityInt = (int)rarity;
            if (rarityInt < 0) {
                rarityInt = 0;
			}
			else if (rarityInt >= numRarities) {
                rarityInt = numRarities - 1;
			}

            float averageValue = useCalamityValuesOnly ? calamityAverageValues[rarityInt] : averageValues[rarityInt];
            int maxOrMin;
            if (sampleValue < averageValue) {
                if (useCalamityValuesOnly) {
                    maxOrMin = calamityMinValues[rarityInt];
                }
                else {
                    maxOrMin = minValues[rarityInt];
                }
            }
            else {
                if (useCalamityValuesOnly) {
                    maxOrMin = calamityMaxValues[rarityInt];
                }
                else {
                    maxOrMin = maxValues[rarityInt];
                }
            }

            float denom = Math.Abs(averageValue - maxOrMin);
            float valueRarity = valueMultiplier + valueMultiplier * (sampleValue - averageValue) / denom;
            if ((valueRarity >= 1f || valueRarity <= 0f) && !usingBaseRarity && !useCalamityValuesOnly && rarity != (float)sampleItem.rare) {
                //Get it's base valueRarity
                valueRarity = GetValueRarity(sampleItem, sampleItem.rare, true);
            }
            else {
				valueRarity.Clamp(0f, 1f);
			}

            return valueRarity;
        }
        public static float GetWeaponMultiplier(this Item item, Item consumedItem, out int infusedPower) {
            if (consumedItem.IsAir) {
                infusedPower = 0;
                return 1f;
            }

            int weaponInfusionPower = GetBaseInfusionPower(item);
            int consumedWeaponInfusionPower = GetBaseInfusionPower(consumedItem);
            infusedPower = consumedWeaponInfusionPower;

			return GetWeaponMultiplier(weaponInfusionPower, consumedWeaponInfusionPower);
		}
		public static float GetWeaponMultiplier(this Item item, int consumedWeaponInfusionPower) {
			int weaponInfusionPower = GetBaseInfusionPower(item);
            return GetWeaponMultiplier(weaponInfusionPower, consumedWeaponInfusionPower);
		}
        private static float GetWeaponMultiplier(int weaponInfusionPower, int consumedWeaponInfusionPower) {
			float multiplier = (float)Math.Pow(InfusionDamageMultiplier, (consumedWeaponInfusionPower - weaponInfusionPower) / 100f);

			return multiplier > 1f || WEMod.clientConfig.AllowInfusingToLowerPower ? multiplier : 1f;
		}
		public static int GetWeaponInfusionPower(this Item item) {
			if (item.TryGetEnchantedItem(out EnchantedWeapon enchantedWeapon) && enchantedWeapon.infusedItemName != "") {
				return enchantedWeapon.InfusionPower;
			}
			else {
				return GetBaseInfusionPower(item);
			}
		}
		public static int GetWeaponInfusionPower(this EnchantedWeapon enchantedWeapon) {
            if (enchantedWeapon.infusedItemName != "") {
                return enchantedWeapon.InfusionPower;
            }
            else {
                return GetBaseInfusionPower(enchantedWeapon.Item);
            }
        }
        private static int GetBaseInfusionPower(Item weapon) {
			if (!InfusionProgression.TryGetBaseInfusionPower(weapon, out int infusionPower))
				infusionPower = GetInfusionPowerFromRarityAndValue(weapon);

			return infusionPower;
		}
        public static string GetInfusionItemName(this Item item) {
            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && enchantedItem.infusedItemName != "") {
                return enchantedItem.infusedItemName;
            }
			else {
                return item.Name;
            }
        }
        public static bool TryInfuseItem(this Item item, Item consumedItem, bool reset = false, bool finalize = false) {
            bool failedItemFind = false;
            if (consumedItem.TryGetEnchantedItemSearchAll(out EnchantedItem consumedEnchantedItem) && consumedEnchantedItem.infusedItemName != "") {
                if (TryInfuseItem(item, consumedEnchantedItem.infusedItemName, reset, finalize)) {
                    return true;
                }
				else {
                    failedItemFind = true;
                }
            }

            if(!item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
                $"Failied to infuse item: {item.S()} with consumedItem: {consumedItem.S()}".LogNT(ChatMessagesIDs.FailedInfuseItem);
                return false;
			}
            
            int infusedPower;
            float damageMultiplier;
            string consumedItemName;
            int infusedArmorSlot;
            if (enchantedItem is EnchantedWeapon enchantedWeapon && (consumedEnchantedItem is EnchantedWeapon || consumedItem.IsAir)) {
                //Weapon
                if (item.GetWeaponInfusionPower() < consumedItem.GetWeaponInfusionPower() || WEMod.clientConfig.AllowInfusingToLowerPower || reset) {
                    if (failedItemFind) {
                        infusedPower = consumedEnchantedItem is EnchantedWeapon consumedEnchantedWeapon ? consumedEnchantedWeapon.InfusionPower : -1;
                        consumedItemName = consumedEnchantedItem.infusedItemName;
                        damageMultiplier = enchantedWeapon.infusionDamageMultiplier;
                    }
                    else {
                        consumedItemName = consumedItem.Name;
						damageMultiplier = GetWeaponMultiplier(item, consumedItem, out infusedPower);
                    }

                    if (enchantedWeapon.InfusionPower < infusedPower || WEMod.clientConfig.AllowInfusingToLowerPower || reset) {
                        if (!finalize) {
                            enchantedWeapon.infusionDamageMultiplier = damageMultiplier;
                        }
                        else {
                            enchantedWeapon.infusionDamageMultiplier = damageMultiplier;
                            enchantedWeapon.InfusionPower = infusedPower;
                            enchantedWeapon.infusedItemName = consumedItemName;
                            int infusionValueAdded = ContentSamples.ItemsByType[consumedItem.type].value - ContentSamples.ItemsByType[item.type].value;
                            enchantedWeapon.InfusionValueAdded = infusionValueAdded > 0 ? infusionValueAdded : 0;
                        }

                        return true;
                    }
                    else if (finalize) {
                        Main.NewText($"Your {item.Name}({enchantedWeapon.InfusionPower}) cannot gain additional power from the offered {consumedItem.Name}({infusedPower}).");
                    }
                }
                else if (finalize) {
                    Main.NewText($"The Infusion Power of the item being upgraded must be lower than the Infusion Power of the consumed item.");
                }

                return false;
            }
            else if (enchantedItem is EnchantedArmor enchantedArmor && (consumedEnchantedItem is EnchantedArmor || consumedItem.IsAir)) {
                //Armor
                if (item.GetSlotIndex() == consumedItem.GetSlotIndex() || reset) {
                    if (item.GetInfusionArmorSlot(true) != consumedItem.GetInfusionArmorSlot()) {
                        if (failedItemFind) {
                            consumedItemName = enchantedArmor.infusedItemName;
                            infusedArmorSlot = ContentSamples.ItemsByType[item.type].GetInfusionArmorSlot();
                        }
                        else {
                            consumedItemName = consumedItem.Name;
                            infusedArmorSlot = consumedItem.GetInfusionArmorSlot();
                        }

                        if (!finalize) {
                            item.UpdateArmorSlot(infusedArmorSlot);
                        }
                        else {
                            enchantedArmor.infusedItemName = consumedItemName;
                            enchantedArmor.infusedArmorSlot = infusedArmorSlot;
                            enchantedArmor.infusedItem = new Item(consumedItem.type);
                            int infusionValueAdded = ContentSamples.ItemsByType[consumedItem.type].value - ContentSamples.ItemsByType[item.type].value;
                            enchantedArmor.InfusionValueAdded = infusionValueAdded > 0 ? infusionValueAdded : 0;
                        }

                        return true;
                    }
                    else if (finalize && !failedItemFind) {
                        Main.NewText($"The item being upgraded has the same set bonus as the item being consumed and will have no effect.");
                    }

                    return false;
                }
                else if (finalize && !failedItemFind) {
                    Main.NewText($"You cannot infuse armor of different types such as a helmet and body.");
                }

                return false;
            }
            if (finalize && !failedItemFind && (EnchantedItemStaticMethods.IsWeaponItem(item) || EnchantedItemStaticMethods.IsArmorItem(item))) {
                Main.NewText($"Infusion is only possible between items of the same type (Weapon/Armor)");
            }

            return false;
        }
        public static bool TryInfuseItem(this Item item, string infusedItemName, bool reset = false, bool finalize = false) {
            for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++) {
                Item foundItem = new Item(itemType);
                if(foundItem.Name == infusedItemName)
                    return TryInfuseItem(item, foundItem, reset, finalize);
            }

            return TryInfuseItem(item, new Item(), reset, finalize);
        }
        public static void GetGlotalItemStats(this Item item, Item infusedItem, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot) {
			if (EnchantedItemStaticMethods.IsWeaponItem(item)) {
                damageMultiplier = GetWeaponMultiplier(item, infusedItem, out infusedPower);
                infusedArmorSlot = -1;
            }
			else {
                damageMultiplier = 1f;
                infusedPower = 0;
                infusedArmorSlot = infusedItem.GetInfusionArmorSlot();
            }
        }
        public static bool TryGetInfusionStats(this EnchantedItem enchantedItem) {
            if (enchantedItem == null)
                return false;

            bool succededGettingStats = TryGetInfusionStats(enchantedItem, enchantedItem.infusedItemName, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot, out Item infusedItem);
            if (succededGettingStats) {
                if (enchantedItem is EnchantedWeapon enchantedWeapon) {
					enchantedWeapon.InfusionPower = infusedPower;
					enchantedWeapon.infusionDamageMultiplier = damageMultiplier;
                }
                else if (enchantedItem is EnchantedArmor enchantedArmor) {
                    enchantedArmor.infusedArmorSlot = infusedArmorSlot;
                    enchantedArmor.infusedItem = infusedItem;
                }
            }
            else if (enchantedItem is EnchantedWeapon enchantedWeapon) {
                //Damage Multiplier (If failed to Get Global Item Stats)
                enchantedWeapon.infusionDamageMultiplier = enchantedItem.Item.GetWeaponMultiplier(enchantedWeapon.InfusionPower);
            }
                

            return succededGettingStats;
        }
        public static bool TryGetInfusionStats(this EnchantedItem enchantedItem, string infusedItemName, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot, out Item infusedItem) {
            infusedPower = 0;
            damageMultiplier = 1f;
            infusedArmorSlot = -1;
            infusedItem = null;

            if (infusedItemName != "") {
                int type = 0;
                for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++) {
                    Item foundItem = ContentSamples.ItemsByType[itemType];
                    if (foundItem.Name == infusedItemName) {
                        type = itemType;
                        infusedItem = new Item(itemType);
                        break;
                    }
                }

                if (type > 0) {
                    GetGlotalItemStats(enchantedItem.Item, new Item(type), out infusedPower, out damageMultiplier, out infusedArmorSlot);
                    if (enchantedItem is EnchantedWeapon enchantedWeapon) {
                        //item.UpdateInfusionDamage(damageMultiplier, false);
                        enchantedWeapon.infusionDamageMultiplier = damageMultiplier;
                    }
                    else if (enchantedItem is EnchantedArmor enchantedArmor2) {
                        enchantedArmor2.Item.UpdateArmorSlot(infusedArmorSlot);
                    }

                    return true;
                }
            }

            if (enchantedItem is EnchantedArmor enchantedArmor) {
                enchantedArmor.Item.UpdateArmorSlot(infusedArmorSlot);
            }
            
            return false;
        }
        public static void UpdateArmorSlot(this Item item, int infusedArmorSlot) {
            if (WEMod.serverConfig.DisableArmorInfusion)
                return;

            Item sampleItem = ContentSamples.ItemsByType[item.type];
            item.headSlot = sampleItem.headSlot;
            item.bodySlot = sampleItem.bodySlot;
            item.legSlot = sampleItem.legSlot;
            if (infusedArmorSlot != -1) {
                if (item.headSlot != -1) {
                    item.headSlot = infusedArmorSlot;
                }
                else if (item.bodySlot != -1) {
                    item.bodySlot = infusedArmorSlot;
                }
                else if (item.legSlot != -1) {
                    item.legSlot = infusedArmorSlot;
                }
            }
		}
        public static int GetInfusionArmorSlot(this Item item, bool checkBase = false, bool getCurrent = false) {
            if (!getCurrent && item.TryGetEnchantedArmor(out EnchantedArmor enchantedItem) && enchantedItem.infusedArmorSlot != -1) {
                return enchantedItem.infusedArmorSlot;
            }
			else
            {
                if (checkBase) {
                    return ContentSamples.ItemsByType[item.type].GetInfusionArmorSlot();
                }
                else
                {
                    if (item.headSlot != -1) {
                        return item.headSlot;
                    }
                    else if (item.bodySlot != -1) {
                        return item.bodySlot;
                    }
                    else if (item.legSlot != -1) {
                        return item.legSlot;
                    }
					else {
                        return -1;
                    }
                }
            }
        }
        public static int GetSlotIndex(this Item item) {
            Item SampleItem = ContentSamples.ItemsByType[(item.type)];
            if (SampleItem.headSlot != -1) {
                return 0;
            }
            else if (SampleItem.bodySlot != -1) {
                return 1;
            }
            else if (SampleItem.legSlot != -1) {
                return 2;
            }
			else {
                return -1;
            }
        }
    }

    public static class InfusionStaticClasses {
        public static bool InfusionAllowed(this Item item, out bool configAllowed) {
            bool weapon = EnchantedItemStaticMethods.IsWeaponItem(item);
            bool armor = EnchantedItemStaticMethods.IsArmorItem(item);
            bool WeaponAndWeaponInfusionAllowed = weapon && WEMod.serverConfig.InfusionDamageMultiplier > 1000;
			bool ArmorAndArmorInfusionAllowed = armor && !WEMod.serverConfig.DisableArmorInfusion;
			configAllowed = WeaponAndWeaponInfusionAllowed || ArmorAndArmorInfusionAllowed;

			return weapon || armor;
		}
	}
}
