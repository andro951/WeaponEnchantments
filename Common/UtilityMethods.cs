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
        public static void UpdateEnchantment(this Item item, AllForOneEnchantmentBasic enchantment, bool remove = false)
        {
            EnchantedItem iGlobal = item.GetGlobalItem<EnchantedItem>();
            int i = 0;
            foreach(StaticStatStruct staticStat in enchantment.StaticStats)
            {
                bool found = false;
                foreach (FieldInfo field in item.GetType().GetFields())
                {
                    string name = field.Name;
                    if (name == staticStat.Name)
                    {
                        Type fieldType = field.FieldType;
                        //if (fieldType == typeof(float))
                        {
                            float value = (float)field.GetValue(item);
                            staticStat.UpdateStat(ref item, name, remove);
                            found = true;
                        }
                        /*else if (fieldType == typeof(int))
                        {
                            int value = (int)field.GetValue(field);
                            staticStat.UpdateStat(ref item, name, remove);
                            found = true;
                        }*/
                        break;
                    }
                    ModContent.GetInstance<WEMod>().Logger.Info("item field " + i.ToString() + ": " + name);
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
                            //if (propertyType == typeof(float))
                            {
                                float value = (float)property.GetValue(property, null);
                                staticStat.UpdateStat(ref item, name, remove);
                                found = true;
                            }
                            /*else if (propertyType == typeof(int))
                            {
                                int value = (int)property.GetValue(property, null);
                                staticStat.UpdateStat(ref item, name, remove);
                                found = true;
                            }*/
                            break;
                        }
                        ModContent.GetInstance<WEMod>().Logger.Info("item property " + i.ToString() + ": " + name);
                        i++;
                    }
                }
            }
            foreach(EStat eStat in enchantment.EStats)
            {
                float add = eStat.Additive * (remove ? -1f : 1f);
                float mult = remove ? 1 / eStat.Multiplicative : eStat.Multiplicative;
                StatModifier statModifier = new StatModifier(1f + add, mult);
                if (!iGlobal.statMultipliers.ContainsKey(eStat.StatName))
                {
                    iGlobal.statMultipliers.Add(eStat.StatName, statModifier);
                }
                else
                {
                    iGlobal.statMultipliers[eStat.StatName] = iGlobal.statMultipliers[eStat.StatName].CombineWith(statModifier);
                    if(iGlobal.statMultipliers[eStat.StatName].Additive == 1f && iGlobal.statMultipliers[eStat.StatName].Multiplicative == 1f)
                    {
                        iGlobal.statMultipliers.Remove(eStat.StatName);
                    }
                }
            }
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
