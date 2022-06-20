using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace WeaponEnchantments.Common
{
    public static class InfusionManager
    {
        public const int numRarities = 11;
        public static float[] averageValues = new float[numRarities];
        public static int[] minValues = new int[numRarities];
        public static int[] maxValues = new int[numRarities];
        public const float rarityMultiplier = 1.25f;

        public static void SetUpVanilla()
        {
            Dictionary<string, List<int[]>> weaponsDict = GetItemDict(0);
            int[] total = new int[numRarities];
            int[] count = new int[numRarities];
            foreach (int[] stats in weaponsDict["Terraria"])
            {
                int rarity = stats[0];
                int value = stats[1];
                total[rarity] += value;
                count[rarity]++;
                if(minValues[rarity] > value || minValues[rarity] == 0)
                    minValues[rarity] = value;
                if(maxValues[rarity] < value)
                    maxValues[rarity] = value;
            }
            string msg = "";
            for(int i = 0; i < numRarities; i++)
            {
                averageValues[i] = (float)total[i]/(float)count[i];
                msg += $"rarity: {i} average: {averageValues[i]} min: {minValues[i]} max: {maxValues[i]}\n";
            }
            //msg.Log();
        }
        private static Dictionary<string, List<int[]>> GetItemDict(byte mode)
        {
            Dictionary<string, List<int[]>> itemsDict = new Dictionary<string, List<int[]>>();
            //string msg = "";
            for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++)
            {
                Item item = new Item(itemType);
                if (item != null)
                {
                    if (!item.consumable && item.axe < 1 && item.pick < 1 && item.hammer < 1)
                    {
                        string modName = item.ModItem != null ? item.ModItem.Mod.Name : "Terraria";
                        if (mode == 0 && WEMod.IsWeaponItem(item) || mode == 1 && WEMod.IsArmorItem(item) || mode == 2 && WEMod.IsArmorItem(item))
                        {
                            //msg += item.Name;
                            int[] itemStats = { item.rare, item.value, item.damage };
                            if (!itemsDict.ContainsKey(modName))
                                itemsDict.Add(modName, new List<int[]>());
                            itemsDict[modName].Add(itemStats);
                            //for (int i = 0; i < itemStats.Length; i++)
                            //{
                            //    msg += $",{itemStats[i]}";
                            //}
                            //msg += "\n";
                        }
                    }
                }
            }
            //msg.Log();
            return itemsDict;
        }
        public static float GetRarity(this Item item)
        {
            Item sampleItem = ContentSamples.ItemsByType[item.type].Clone();
            float valueMultiplier = 0.5f;
            int rarity = sampleItem.rare;
            int value = sampleItem.value;
            float averageValue = averageValues[rarity];
            float combinedRarity;
            int maxOrMin = value < averageValue ? minValues[rarity] : maxValues[rarity];
            float denom = Math.Abs(averageValue - maxOrMin);
            combinedRarity = valueMultiplier + rarity + valueMultiplier * (value - averageValue) / denom;
            if (combinedRarity > rarity + 2 * valueMultiplier)
                combinedRarity = rarity + 2 * valueMultiplier;
            else if (combinedRarity < rarity - 2 * valueMultiplier)
                combinedRarity = rarity - 2 * valueMultiplier;
            return combinedRarity;
        }
        public static float GetMultiplier(this Item item, Item consumedItem, out int infusedPower)
        {
            if (consumedItem.IsAir)
            {
                infusedPower = 0;
                return 1f;
            }
            float itemRarity = GetRarity(item);
            float consumedRarity = GetRarity(consumedItem);
            infusedPower = (int)Math.Round(consumedRarity * 100f);
            return (float)Math.Pow(rarityMultiplier, consumedRarity - itemRarity);
        }
        public static int GetInfusionPower(this Item item)
        {
            if (item.G().infusedItemName != "")
                return item.G().infusedPower;
            float notUse = GetMultiplier(new Item(ItemID.CopperShortsword), item, out int infusedPower);
            return infusedPower;
        }
        public static string GetInfusionItemName(this Item item)
        {
            if (item.G().infusedItemName != "")
                return item.G().infusedItemName;
            else
                return item.Name;
        }
        public static bool TryInfuseItem(this Item item, Item consumedItem, bool reset = false, bool finalize = false)
        {
            if (WEMod.IsWeaponItem(item) && WEMod.IsWeaponItem(consumedItem))
            {
                bool failedItemFind = false;
                if (consumedItem.G().infusedItemName != "")
                {
                    if (TryInfuseItem(item, consumedItem.G().infusedItemName, reset, finalize))
                        return true;
                    else
                        failedItemFind = true;
                }
                Item sampleItem = ContentSamples.ItemsByType[item.type].Clone();
                if (sampleItem.rare < consumedItem.rare || reset)
                {
                    int infusedPower;
                    float damageMultiplier;
                    string consumedItemName;
                    if (failedItemFind)
                    {
                        infusedPower = consumedItem.G().infusedPower;
                        damageMultiplier = consumedItem.G().damageMultiplier;
                        consumedItemName = consumedItem.G().infusedItemName;
                    }
                    else
                    {
                        consumedItemName = consumedItem.Name;
                        damageMultiplier = GetMultiplier(item, consumedItem, out infusedPower);
                    }
                    if (item.G().infusedPower < infusedPower || reset)
                    {
                        if (!finalize)
                        {
                            item.UpdateInfusionDamage(damageMultiplier);
                        }
                        else
                        {
                            item.G().infusedPower = infusedPower;
                            item.G().damageMultiplier = damageMultiplier;
                            item.G().infusedItemName = consumedItemName;
                            item.value += ContentSamples.ItemsByType[consumedItem.type].value - sampleItem.value;
                            item.rare = consumedItem.rare;
                        }
                        return true;
                    }
                    else if (finalize)
                        Main.NewText($"Your {item.Name}({item.G().infusedPower}) cannot gain additional power from the offered {consumedItem.Name}({infusedPower}).");
                }
                else if (finalize)
                    Main.NewText($"The base rarity of the item being upgraded must be lower than the rarity of the consumed item.");
            }
            return false;
        }
        public static bool TryInfuseItem(this Item item, string infusedItemName, bool reset = false, bool finalize = false)
        {
            for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++)
            {
                Item foundItem = new Item(itemType);
                if(foundItem.Name == infusedItemName)
                    return TryInfuseItem(item, foundItem, reset, finalize);
            }
            return TryInfuseItem(item, new Item(), reset, finalize);
        }
        public static void GetGlotalItemStats(this Item item, Item infusedItem, out int infusedPower, out float damageMultiplier)
        {
            damageMultiplier = GetMultiplier(item, infusedItem, out infusedPower);
        }
        public static bool TryGetGlotalItemStats(this Item item)
        {
            bool returnValue = TryGetGlotalItemStats(item, item.G().infusedItemName, out int infusedPower, out float damageMultiplier);
            if (returnValue)
            {
                item.G().infusedPower = infusedPower;
                item.G().damageMultiplier = damageMultiplier;
            }
            return returnValue;
        }
        public static bool TryGetGlotalItemStats(this Item item, string infusedItemName, out int infusedPower, out float damageMultiplier)
        {
            if (infusedItemName != "")
            {
                int type = 0;
                for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++)
                {
                    Item foundItem = new Item(itemType);
                    if (foundItem.Name == infusedItemName)
                    {
                        type = itemType;
                        break;
                    }
                }
                if (type > 0)
                {
                    GetGlotalItemStats(item, new Item(type), out infusedPower, out damageMultiplier);
                    item.UpdateInfusionDamage(damageMultiplier, false);
                    return true;
                }
            }
            infusedPower = 0;
            damageMultiplier = 1f;
            item.UpdateInfusionDamage(damageMultiplier, false);
            return false;
        }
        public static void UpdateInfusionDamage(this Item item, float damageMultiplier, bool updateStats = true)
        {
            if(damageMultiplier != 1f || item.G().statModifiers.ContainsKey("damage"))
            {
                if(damageMultiplier > 0f)
                {
                    if (item.G().statModifiers.ContainsKey("damage"))
                    {
                        item.G().statModifiers["damage"] = new StatModifier(1f, damageMultiplier);
                    }
                    else
                    {
                        item.G().statModifiers.Add("damage", new StatModifier(1f, damageMultiplier));
                    }
                    if (updateStats)
                        Main.LocalPlayer.G().UpdateItemStats(ref item);
                }
                else
                {
                    ($"Prevented an issue that would cause your item: {item.S()} to be set to 0 damage.  Please inform andro951").Log();
                }
            }
        }
    }
}
