using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using WeaponEnchantments.Common.Globals;

namespace WeaponEnchantments.Common
{
    public static class InfusionManager
    {
        public const int numVanillaWeaponRarities = 11;
        public const int numRarities = 17;
        public static float[] averageValues = new float[numRarities];
        public static int[] minValues = new int[numRarities];
        public static int[] maxValues = new int[numRarities];
        public static readonly float rarityMultiplier = (float)WEMod.serverConfig.InfusionDamageMultiplier / 1000f;
        public const float minMaxValueMultiplier = 0.25f;

        public static void SetUpVanillaWeaponInfusionPowers()
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
            for(int i = numVanillaWeaponRarities; i < numRarities; i++)
            {
                if (i == 16)
                    maxValues[i] = 2000000;
                else
                    maxValues[i] = 1100000 + 100000 * (i - numVanillaWeaponRarities);
                minValues[i] = maxValues[i - 1];
                averageValues[i] = (minValues[i] + maxValues[i]) / 2;
            }
            string msg = "";
            for(int i = 0; i < numRarities; i++)
            {
                if(i < numVanillaWeaponRarities)
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
        public static float GetWeaponRarity(this Item item)
        {
            bool valueOnly = false;
            int rarity = 0;
            Item sampleItem = ContentSamples.ItemsByType[item.type].Clone();
            if(item.ModItem?.Mod.Name == "CalamityMod")
                valueOnly = true;
            switch (item.Name)
            {
                /*case "Pulse Pistol":
                case "Taser":
                case "Gauss Dagger":
                case "Star Swallower Containment Unit":
                case "Trackig Disk":
                    valueOnly = true;
                    //rarity = 3;
                    break;*/
                case "Arkhalis":
                    rarity = 5;
                    break;
                case "Terragrim":
                    rarity = 3;
                    break;
                case "The Only Thing I Know For Real":
                    rarity = 9;
                    break;
                case "Bury The Light":
                case "Ultima Thule":
                    rarity = 12;
                    break;
                default:
                    rarity = sampleItem.rare;
                    break;
            }
            float valueMultiplier = 0.5f;
			if (valueOnly)
			{
                int i;
                for(i = 0; i < numRarities; i++)
				{
                    float max = maxValues[i];
                    if(maxValues[i] > item.value)
					{
                        float average = averageValues[i];
                        if (averageValues[i] >= item.value)
                        {
                            float min = minValues[i];
                            if (minValues[i] > item.value)
                                i--;
                            break;
                        }
                    }
                }
                rarity = i;
			}
            else if (rarity == 11 && item.value > maxValues[11])
            {
                int i;
                for (i = 12; i < numRarities; i++)
                {
                    if (minValues[i] >= item.value)
                    {
                        i--;
                        break;
                    }
                }
                rarity = i;
            }
            if (rarity > numRarities - 1) rarity = numRarities - 1;
            else if (rarity < 0) rarity = 0;
            int value = sampleItem.value;
            float averageValue = averageValues[rarity];
            float combinedRarity;
            int maxOrMin = value < averageValue ? minValues[rarity] : maxValues[rarity];
            float denom = Math.Abs(averageValue - maxOrMin);
            float valueRarity = valueMultiplier + valueMultiplier * (value - averageValue) / denom;
            Math.Clamp(valueRarity, 0f, 1f);
            if (valueRarity < 0f)
                valueRarity = 0f;
            else if(valueRarity > 1f)
                valueRarity = 1f;
            /*if (combinedRarity > rarity + 2 * valueMultiplier)
                combinedRarity = rarity + 2 * valueMultiplier;
            else if (combinedRarity < rarity - 2 * valueMultiplier)
                combinedRarity = rarity - 2 * valueMultiplier;*/
            combinedRarity = rarity + valueRarity;
            return combinedRarity > 0 ? combinedRarity : 0;
        }//Done
        public static float GetWeaponMultiplier(this Item item, Item consumedItem, out int infusedPower)
        {
            if (consumedItem.IsAir)
            {
                infusedPower = 0;
                return 1f;
            }
            float itemRarity = GetWeaponRarity(item);
            float consumedRarity = GetWeaponRarity(consumedItem);
            infusedPower = (int)Math.Round(consumedRarity * 100f);
            return (float)Math.Pow(rarityMultiplier, consumedRarity - itemRarity);
        }//Done
        public static int GetWeaponInfusionPower(this Item item)
        {
            if (item.G().infusedItemName != "")
                return item.G().infusedPower;
            float notUse = GetWeaponMultiplier(new Item(ItemID.CopperShortsword), item, out int infusedPower);
            return infusedPower;
        }//Done
        public static string GetInfusionItemName(this Item item)
        {
            if (item.G().infusedItemName != "")
                return item.G().infusedItemName;
            else
                return item.Name;
        }//Done
        public static bool TryInfuseItem(this Item item, Item consumedItem, bool reset = false, bool finalize = false)
        {
            bool failedItemFind = false;
            if (!consumedItem.IsAir && consumedItem.G().infusedItemName != "")
            {
                if (TryInfuseItem(item, consumedItem.G().infusedItemName, reset, finalize))
                    return true;
                else
                    failedItemFind = true;
            }
            int infusedPower = 0;
            float damageMultiplier = 1f;
            string consumedItemName = "";
            int infusedArmorSlot = -1;
            if (WEMod.IsWeaponItem(item) && ((WEMod.IsWeaponItem(consumedItem) || consumedItem.IsAir)))
            {
                if (item.GetWeaponInfusionPower() < consumedItem.GetWeaponInfusionPower() || reset)
                {
                    if (failedItemFind)
                    {
                        infusedPower = consumedItem.G().infusedPower;
                        damageMultiplier = consumedItem.G().damageMultiplier;
                        consumedItemName = consumedItem.G().infusedItemName;
                    }
                    else
                    {
                        consumedItemName = consumedItem.Name;
                        damageMultiplier = GetWeaponMultiplier(item, consumedItem, out infusedPower);
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
                            int infusionValueAdded = ContentSamples.ItemsByType[consumedItem.type].value - ContentSamples.ItemsByType[item.type].value;
                            item.G().infusionValueAdded = infusionValueAdded > 0 ? infusionValueAdded : 0;
                        }
                        return true;
                    }
                    else if (finalize)
                        Main.NewText($"Your {item.Name}({item.G().infusedPower}) cannot gain additional power from the offered {consumedItem.Name}({infusedPower}).");
                }
                else if (finalize)
                    Main.NewText($"The Infusion Power of the item being upgraded must be lower than the Infusion Power of the consumed item.");
                return false;
            }//Weapon
            else if (WEMod.IsArmorItem(item) && ((WEMod.IsArmorItem(consumedItem) || consumedItem.IsAir)))
			{
                if (item.GetInfusionArmorSlot(true) != consumedItem.GetInfusionArmorSlot())
                {
					if (failedItemFind)
					{
                        consumedItemName = item.G().infusedItemName;
                        infusedArmorSlot = ContentSamples.ItemsByType[item.type].GetInfusionArmorSlot();
                    }
                    else
                    {
                        consumedItemName = consumedItem.Name;
                        infusedArmorSlot = consumedItem.GetInfusionArmorSlot();
                    }
					if (!finalize)
					{
                        item.UpdateArmorSlot(infusedArmorSlot);
					}
                    else
					{
                        item.G().infusedItemName = consumedItemName;
                        item.G().infusedArmorSlot = infusedArmorSlot;
                        int infusionValueAdded = ContentSamples.ItemsByType[consumedItem.type].value - ContentSamples.ItemsByType[item.type].value;
                        item.G().infusionValueAdded = infusionValueAdded > 0 ? infusionValueAdded : 0;
                    }
                    return true;
                }
                else if (finalize && !failedItemFind)
                    Main.NewText($"The item being upgraded has the same set bonus as the item being consumed and will have no effect.");
                return false;
            }//Armor
            if (finalize && !failedItemFind && (WEMod.IsWeaponItem(item) || WEMod.IsArmorItem(item)))
                Main.NewText($"Infusion is only possitle between items of the same type (Weapon/Armor)");
            return false;
        }//Done
        public static bool TryInfuseItem(this Item item, string infusedItemName, bool reset = false, bool finalize = false)
        {
            for (int itemType = 1; itemType < ItemLoader.ItemCount; itemType++)
            {
                Item foundItem = new Item(itemType);
                if(foundItem.Name == infusedItemName)
                    return TryInfuseItem(item, foundItem, reset, finalize);
            }
            return TryInfuseItem(item, new Item(), reset, finalize);
        }//Done
        public static void GetGlotalItemStats(this Item item, Item infusedItem, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot)
        {
			if (WEMod.IsWeaponItem(item))
            {
                damageMultiplier = GetWeaponMultiplier(item, infusedItem, out infusedPower);
                infusedArmorSlot = -1;
            }
			else
			{
                damageMultiplier = 1f;
                infusedPower = 0;
                infusedArmorSlot = infusedItem.GetInfusionArmorSlot();
            }
        }//Done
        public static bool TryGetGlotalItemStats(this Item item)
        {
            bool returnValue = TryGetGlotalItemStats(item, item.G().infusedItemName, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot);
            if (returnValue)
            {
                item.G().infusedPower = infusedPower;
                item.G().damageMultiplier = damageMultiplier;
                item.G().infusedArmorSlot = infusedArmorSlot;
            }
            return returnValue;
        }//Done
        public static bool TryGetGlotalItemStats(this Item item, string infusedItemName, out int infusedPower, out float damageMultiplier, out int infusedArmorSlot)
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
                    GetGlotalItemStats(item, new Item(type), out infusedPower, out damageMultiplier, out infusedArmorSlot);
                    if (WEMod.IsWeaponItem(item))
                        item.UpdateInfusionDamage(damageMultiplier, false);
                    else if (WEMod.IsArmorItem(item))
                        item.UpdateArmorSlot(infusedArmorSlot);
                    return true;
                }
            }
            infusedPower = 0;
            damageMultiplier = 1f;
            infusedArmorSlot = -1;
            if (WEMod.IsWeaponItem(item))
                item.UpdateInfusionDamage(damageMultiplier, false);
            else if (WEMod.IsArmorItem(item))
                item.UpdateArmorSlot(infusedArmorSlot);
            return false;
        }//Done
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
                    string errorMessage = $"Prevented an issue that would cause your item: {item.S()} to be set to 0 damage.  Please inform andro951";
                    errorMessage.Log();
                    Main.NewText(errorMessage);
                }
            }
        }
        public static void UpdateArmorSlot(this Item item, int infusedArmorSlot)
		{
            Item sampleItem = ContentSamples.ItemsByType[item.type];
            item.headSlot = sampleItem.headSlot;
            item.bodySlot = sampleItem.bodySlot;
            item.legSlot = sampleItem.legSlot;
            if (infusedArmorSlot != -1)
			{
                if (item.headSlot != -1)
                    item.headSlot = infusedArmorSlot;
                else if (item.bodySlot != -1)
                    item.bodySlot = infusedArmorSlot;
                else if (item.legSlot != -1)
                    item.legSlot = infusedArmorSlot;
            }
		}//Done
        public static int GetInfusionArmorSlot(this Item item, bool checkBase = false, bool getCurrent = false)
		{
            if (!getCurrent && item.TryGetGlobalItem(out EnchantedItem iGlobal) && iGlobal.infusedArmorSlot != -1)
                return iGlobal.infusedArmorSlot;
			else
            {
                if (checkBase)
                    return ContentSamples.ItemsByType[item.type].GetInfusionArmorSlot();
                else
                {
                    if (item.headSlot != -1)
                        return item.headSlot;
                    else if (item.bodySlot != -1)
                        return item.bodySlot;
                    else if (item.legSlot != -1)
                        return item.legSlot;
                    else
                        return -1;
                }
            }
        }//Done
    }
}
