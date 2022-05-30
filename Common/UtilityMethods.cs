using System.Collections.Generic;
using Terraria;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments;
using WeaponEnchantments.Items;
using System.Reflection;
using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common
{
    public static class UtilityMethods
    {
        ///<summary>
        ///Gets (EnchantedItem : GlobalItem)
        ///</summary>
        public static EnchantedItem G(this Item item) => item.GetGlobalItem<EnchantedItem>();
        ///<summary>
        ///Gets this item's enchantemnt at index i.  Gets (AllForOneEnchantmentBasic)item.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem
        ///</summary>
        public static AllForOneEnchantmentBasic E(this Item item, int i) => (AllForOneEnchantmentBasic)item.GetGlobalItem<EnchantedItem>().enchantments[i].ModItem;
        ///<summary>
        ///Gets item in the enchanting table itemslot.  Gets wePlayer.enchantingTableUI.itemSlot[i].Item
        ///</summary>
        public static Item I(this WEPlayer wePlayer, int i = 0) => wePlayer.enchantingTableUI.itemSlot[i].Item;
        ///<summary>
        ///Gets enchantment in the enchanting table in enchantment slot i.  Gets wePlayer.enchantingTableUI.enchantmentSlot[i].Item
        ///</summary>
        public static Item E(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlot[i].Item;
        ///<summary>
        ///Gets essence in the enchanting table in essence slot i.  
        ///</summary>
        public static Item Es(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.essenceSlot[i].Item;
        
        private static readonly char[] upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static bool IsUpper(this char c)
        {
            foreach (char upper in upperCase)
            {
                if (upper == c)
                    return true;
            }
            return false;
        }
        public static List<string> SplitString(this string s)
        {
            List<string> list = new List<string>();
            int start = 0;
            int end = 0;
            for (int i = 1; i < s.Length; i++)
            {
                if (s[i].IsUpper())
                {
                    end = i - 1;
                    list.Add(s.Substring(start, end - start + 1));
                    start = end + 1;
                }
                else if (i == s.Length - 1)
                {
                    end = i;
                    list.Add(s.Substring(start, end - start + 1));
                }
            }
            return list;
        }
        public static string AddSpaces(string s)
        {
            int start = 0;
            int end = 0;
            string finalString = "";
            for(int i = 1; i < s.Length; i++)
            {
                if (s[i].IsUpper())
                {
                    end = i - 1;
                    finalString += s.Substring(start, end - start + 1) + " ";
                    start = end + 1;
                }
                else if(i == s.Length - 1)
                {
                    end = i;
                    finalString += s.Substring(start, end - start + 1);
                }
            }
            return finalString;
        }
        public static string ToFieldName(string s)
        {
            if (s.Length > 0)
            {
                string[] apla = { "abcdefghijklmnopqrstuvwxyz", "ABCDEFGHIJKLMNOPQRSTUVWKYZ" };
                if(s[0].IsUpper())
                    for(int i = 0; i < apla[0].Length; i++)
                    {
                        if(s[0] == apla[1][i])
                        {
                            char c = apla[0][i];
                            return c + s.Substring(1);
                        }
                    }
            }
            return s;
        }
        public static string RemoveProjectileName(this string s)
        {
            int i = s.IndexOf("ProjectileName.");
            return i == 0 ? s.Substring(15) : s;
        }
        public static int CheckMatches(this List<string> l1, List<string> l2)
        {
            int matches = 0;
            foreach(string s in l1)
            {
                foreach(string s2 in l2)
                {
                    if(s2.IndexOf(s) > -1)
                    {
                        matches++;
                    }
                }
            }
            return matches;
        }
        public static bool IsSameEnchantedItem(this Item item1, Item item2)
        {
            if(!item1.IsAir && !item2.IsAir)
            {
                if (item1.TryGetGlobalItem(out EnchantedItem global1))
                {
                    if (item2.TryGetGlobalItem(out EnchantedItem global2))
                    {
                        if (item1.type == item2.type && global1.experience == global2.experience && global1.powerBoosterInstalled == global2.powerBoosterInstalled && item1.value == item2.value && item1.prefix == item2.prefix)
                        {
                            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                            {
                                if (global1.enchantments[i].type != global2.enchantments[i].type)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static float GetReductionFactor(int hp)
        {
            float factor = hp < 7000 ? hp / 1000f + 1f : 8f;
            return factor;
        }
        public static void RemoveUntilPositive(this Item item)
        {
            int netMode = Main.netMode;
            int gameMode = Main.GameMode;
            if (!item.IsAir)
            {
                if (WEMod.IsEnchantable(item))
                {
                    if (item.TryGetGlobalItem(out EnchantedItem iGlobal))
                    {
                        if (iGlobal.GetLevelsAvailable() < 0)
                        {
                            for (int k = EnchantingTable.maxEnchantments - 1; k >= 0 && iGlobal.GetLevelsAvailable() < 0; k--)
                            {
                                if (!iGlobal.enchantments[k].IsAir)
                                {
                                    item.GetGlobalItem<EnchantedItem>().enchantments[k] = Main.LocalPlayer.GetItem(Main.myPlayer, iGlobal.enchantments[k], GetItemSettings.LootAllSettings);
                                }
                                if (!iGlobal.enchantments[k].IsAir)
                                {
                                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), iGlobal.enchantments[k]);
                                    iGlobal.enchantments[k] = new Item();
                                }
                            }
                            Main.NewText("Your " + item.Name + "' level is too low to use that many enchantments.");
                        }//Check too many enchantments on item
                    }
                }
            }
        }
        public static void EditBoolField(this Dictionary<string, int> dictionary, string name, bool remove)
        {
            int num;
            if (!dictionary.ContainsKey(name))
            {
                dictionary.Add(name, 1);
            }
            else
            {
                num = dictionary[name];
                dictionary[name] += (remove ? -1 : 1);
                num = dictionary[name];
            }
            num = dictionary[name];
            if (dictionary[name] < 1)
            {
                dictionary.Remove(name);
            }
        }
        public static void UpdateEnchantment(this Item item, AllForOneEnchantmentBasic enchantment, int slotNum, bool remove = false)
        {
            EnchantedItem iGlobal = item.GetGlobalItem<EnchantedItem>();
            if(enchantment != null)
            {
                int i = 0;
                if (enchantment.PotionBuff > -1)
                {
                    if (iGlobal.potionBuffs.ContainsKey(enchantment.PotionBuff))
                    {
                        iGlobal.potionBuffs[enchantment.PotionBuff] += (remove ? -1 : 1);
                        if (iGlobal.potionBuffs[enchantment.PotionBuff] < 1)
                            iGlobal.potionBuffs.Remove(enchantment.PotionBuff);
                    }
                    else
                    {
                        iGlobal.potionBuffs.Add(enchantment.PotionBuff, 1);
                    }
                }
                foreach (EStat eStat in enchantment.EStats)
                {
                    float add = eStat.Additive * (remove ? -1f : 1f);
                    float mult = remove ? 1 / eStat.Multiplicative : eStat.Multiplicative;
                    float flat = eStat.Flat * (remove ? -1f : 1f);
                    StatModifier statModifier = new StatModifier(1f + add, mult, flat);
                    if (!iGlobal.statMultipliers.ContainsKey(eStat.StatName))
                    {
                        iGlobal.statMultipliers.Add(eStat.StatName, statModifier);
                    }
                    else
                    {
                        iGlobal.statMultipliers[eStat.StatName] = iGlobal.statMultipliers[eStat.StatName].CombineWith(statModifier);
                        if (iGlobal.statMultipliers[eStat.StatName].Additive == 1f && iGlobal.statMultipliers[eStat.StatName].Multiplicative == 1f)
                        {
                            iGlobal.statMultipliers.Remove(eStat.StatName);
                        }
                    }
                }
                if (enchantment.EStats.Count < 1 && enchantment.PotionBuff == -1)
                {
                    foreach (StaticStatStruct staticStat in enchantment.StaticStats)
                    {
                        bool found = false;
                        foreach (FieldInfo field in item.GetType().GetFields())
                        {
                            string name = field.Name;
                            if (name == staticStat.Name)
                            {
                                Type fieldType = field.FieldType;
                                float value;
                                if (fieldType == typeof(float))
                                {
                                    value = (float)field.GetValue(item);
                                }
                                else if (fieldType == typeof(int))
                                {
                                    int valueInt = (int)field.GetValue(item);
                                    value = (float)valueInt;
                                }
                                staticStat.UpdateStat(ref item, name, remove, fieldType == typeof(bool), staticStat.PreventBoolStat);
                                found = true;
                                break;
                            }
                            //ModContent.GetInstance<WEMod>().Logger.Info("item field " + i.ToString() + ": " + name);
                            i++;
                        }
                        i = 0;
                        if (!found)
                        {
                            foreach (PropertyInfo property in item.GetType().GetProperties())
                            {
                                string name = property.Name;
                                if (name == staticStat.Name)
                                {
                                    Type propertyType = property.PropertyType;
                                    float value;
                                    if (propertyType == typeof(float))
                                    {
                                        value = (float)property.GetValue(item, null);
                                        
                                    }
                                    else if (propertyType == typeof(int))
                                    {
                                        int valueInt = (int)property.GetValue(item, null);
                                        value = (float)valueInt;
                                    }
                                    staticStat.UpdateStat(ref item, name, remove, propertyType == typeof(bool), staticStat.PreventBoolStat, true);
                                    found = true;
                                    break;
                                }
                                //ModContent.GetInstance<WEMod>().Logger.Info("item property " + i.ToString() + ": " + name);
                                i++;
                            }
                        }
                    }
                }
                enchantment.statsSet = true;
            }
            //iGlobal.statsSet[slotNum] = true;
        }
        public static void SpawnCoins(int coins, bool delay = false)
        {
            int coinType = ItemID.PlatinumCoin;
            int coinValue = 1000000;
            while (coins > 0)
            {
                int numCoinsToSpawn = coins / coinValue;
                if(numCoinsToSpawn > 0)
                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), coinType, numCoinsToSpawn);
                coins %= coinValue;
                coinType--;
                coinValue /= 100;
            }
        }
    }
}
