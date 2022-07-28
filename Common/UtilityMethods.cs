using System.Collections.Generic;
using Terraria;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments;
using WeaponEnchantments.Items;
using System.Reflection;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using WeaponEnchantments.UI;

namespace WeaponEnchantments.Common
{
    public static class UtilityMethods {
        public readonly static bool debugging = false;
        private static int spaces = 0;
        private static Dictionary<string, double> logsT = new Dictionary<string, double>();
        public static string reporteMessage = "\nPlease report this to andro951(Weapon Enchantments) allong with a description of what you were doing at the time.";

        ///<summary>
        ///Gets (EnchantedItem : GlobalItem)
        ///</summary>
        public static EnchantedItem G(this Item item) {
            EnchantedItem iGlobal = item.GetGlobalItem<EnchantedItem>();
            iGlobal.Item = item;
            return iGlobal;
        }
        ///<summary>
        ///Gets this item's enchantemnt at index i.  Gets (Enchantment)item.G().enchantments[i].ModItem
        ///</summary>
        public static WEPlayer G(this Player player) => player.GetModPlayer<WEPlayer>();
        public static WEProjectile G(this Projectile projectile) => projectile.GetGlobalProjectile<WEProjectile>();
        public static WEGlobalNPC G(this NPC npc) => npc.GetGlobalNPC<WEGlobalNPC>();
        public static bool TG(this Item item) => item != null && item.TryGetGlobalItem(out EnchantedItem iGlobal);
        public static bool TG(this Item item, out EnchantedItem iGlobal) {
            if (item != null && item.TryGetGlobalItem(out iGlobal)) {
                iGlobal.Item = item;
                return true;
            }
			else {
                iGlobal = null;
                return false;
			}
        }
        public static Item E(this Item item, int i) => item.G().enchantments[i];
        public static Enchantment EM(this Item item, int i) => (Enchantment)item.G().enchantments[i].ModItem;
        ///<summary>
        ///Gets item in the enchanting table itemslot.  Gets wePlayer.enchantingTableUI.itemSlot[i].Item
        ///</summary>
        public static Item I(this WEPlayer wePlayer, int i = 0) => wePlayer.enchantingTableUI.itemSlotUI[i].ItemInSlot;
        ///<summary>
        ///Gets enchantment in the enchanting table in enchantment slot i.  wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item
        ///</summary>
        public static WEUIItemSlot EUI(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlotUI[i];
        ///<summary>
        ///Gets enchantment in the enchanting table in enchantment slot i.  wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item
        ///</summary>
        public static Item E(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlotUI[i].ItemInSlot;
        ///<summary>
        ///Gets enchantment in the enchanting table in enchantment slot i.  (Enchantment)wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.ModItem
        ///</summary>
        public static Enchantment EM(this WEPlayer wePlayer, int i) => (Enchantment)wePlayer.enchantingTableUI.enchantmentSlotUI[i].ItemInSlot.ModItem;
        ///<summary>
        ///Gets essence in the enchanting table in essence slot i.  
        ///</summary>
        public static Item Es(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.essenceSlotUI[i].ItemInSlot;
        /// <summary>
        /// Applies the appliedStatModifier from the item's global item to the value.
        /// </summary>
        public static float A(this Item item, string key, float value) => item.G().appliedStatModifiers.ContainsKey(key) ? item.G().appliedStatModifiers[key].ApplyTo(value) : value;
        /// <summary>
        /// Applies the eStat modifier from the item's global item to the value.
        /// </summary>
        public static float AEI(this Item item, string key, float value)
        {
            if (item.G().appliedEStats.ContainsKey(key))
                return item.G().appliedEStats[key].ApplyTo(value);
            return value;
            //Main.LocalPlayer.GetModPlayer<WEPlayer>().eStats.ContainsKey(key) ? item.G().eStats[key].ApplyTo(value) : value;
        }
        public static bool CEP(this Player player, string key)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            if (wePlayer.eStats.ContainsKey(key))
                return true;
            Item weapon = wePlayer.trackedWeapon;
            if (weapon != null && WEMod.IsWeaponItem(weapon) && weapon.G().eStats.ContainsKey(key))
                return true;
            return false;
        }
        public static float AEP(this Player player, string key, float value)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            StatModifier combinedStatModifier = StatModifier.Default;
            if (wePlayer.eStats.ContainsKey(key))
                combinedStatModifier = wePlayer.eStats[key];
            Item weapon = wePlayer.trackedWeapon;
            if (weapon != null && !weapon.IsAir && weapon.G().eStats.ContainsKey(key))
                combinedStatModifier = combinedStatModifier.CombineWith(weapon.G().eStats[key]);
            return combinedStatModifier.ApplyTo(value);
        }
        public static float AEP(this Player player, Item item, string key, float value)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            StatModifier combinedStatModifier = StatModifier.Default;
            if (wePlayer.eStats.ContainsKey(key))
                combinedStatModifier = wePlayer.eStats[key];
            if (item != null && !item.IsAir && WEMod.IsEnchantable(item) && item.G().eStats.ContainsKey(key))
                combinedStatModifier = combinedStatModifier.CombineWith(item.G().eStats[key]);
            return combinedStatModifier.ApplyTo(value);
        }
        public static bool C(this Item item, string key) => item.G().eStats.ContainsKey(key);
        public static bool C(string key) => Main.LocalPlayer.GetModPlayer<WEPlayer>().eStats.ContainsKey(key);
        public static bool C(this Player player, string key, Item item) => player.G().eStats.ContainsKey(key) || item != null && !item.IsAir && WEMod.IsEnchantable(item) && item.G().eStats.ContainsKey(key);
        public static string S(this StatModifier statModifier) => "<A: " + statModifier.Additive + ", M: " + statModifier.Multiplicative + ", B: " + statModifier.Base + ", F: " + statModifier.Flat + ">";
        public static string S(this EStat eStat) => "<N: " + eStat.StatName + " A: " + eStat.Additive + ", M: " + eStat.Multiplicative + ", B: " + eStat.Base + ", F: " + eStat.Flat + ">";
        public static string S(this EnchantmentStaticStat staticStat) => "<N: " + staticStat.Name + " A: " + staticStat.Additive + ", M: " + staticStat.Multiplicative + ", B: " + staticStat.Base + ", F: " + staticStat.Flat + ">";
        public static string S(this Item item) => item != null ? !item.IsAir ? item.Name : "<Air>" : "null";
        public static string S(this Projectile projectile) => projectile != null ? projectile.Name : "null";
        public static string S(this Player player) => player != null ? player.name : "null";
        public static string S(this NPC npc, bool stats = false) => npc != null ? $"name: {npc.FullName} whoAmI: {npc.whoAmI}{(stats ? $"defense: {npc.defense}, defDefense: {npc.defDefense}, lifeMax: {npc.lifeMax}, life: {npc.life}" : "")}" : "null";
        public static string S(this Enchantment enchantment) => enchantment != null ? enchantment.Name : "null";
        public static string S(this Dictionary<int, int> dictionary, int key) => "contains " + key + ": " + dictionary.ContainsKey(key) + " count: " + dictionary.Count + (dictionary.ContainsKey(key) ? " value: " + dictionary[key] : "");
        public static string S(this Dictionary<string, StatModifier> dictionary, string key) => "contains " + key + ": " + dictionary.ContainsKey(key) + " count: " + dictionary.Count + (dictionary.ContainsKey(key) ? " value: " + dictionary[key].S() : "");
        public static string S(this Dictionary<string, EStat> dictionary, string key) => "contains " + key + ": " + dictionary.ContainsKey(key) + " count: " + dictionary.Count + (dictionary.ContainsKey(key) ? " value: " + dictionary[key].S() : "");
        public static string S(this bool b) => b ? "True" : "False";
        public static string RI(this string s) => s.Length > 2 ? s.Substring(0, 2) == "I_" ? s.Substring(2) : s : s;
        public static string RP(this string s) => s.Length > 2 ? s.Substring(0, 2) == "P_" ? s.Substring(2) : s : s;
        public static bool CI(this string s) => s.Length > 2 ? s.Substring(0, 2) == "I_" : false;

        private static readonly char[] upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] lowerCase = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] numbers = "0123456789".ToCharArray();
        private static readonly string[] apla = { "abcdefghijklmnopqrstuvwxyz", "ABCDEFGHIJKLMNOPQRSTUVWKYZ" };
        public static bool IsUpper(this char c)
        {
            foreach (char upper in upperCase)
            {
                if (upper == c)
                    return true;
            }
            return false;
        }
        public static bool IsLower(this char c)
        {
            foreach (char lower in lowerCase)
            {
                if (lower == c)
                    return true;
            }
            return false;
        }
        public static bool IsNumber(this char c)
        {
            foreach (char number in numbers)
            {
                if (number == c)
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
        public static string GetNameFolderName(this string s, int numberOfFolders = 1) {
            int i = s.Length - 1;
            for(int j = 0; j < numberOfFolders; j++) {
                i = s.FindChar('.', false);

                //Not last time loop will run
                if(j != numberOfFolders - 1) {
                    //Remove last folder from the string and continue the loop
                    s = s.Substring(0, i);
				}
			}

            return s.Substring(i + 1);
		}

        public static string GetFileName(this string s, char searchChar = '.') {
            int i = s.FindChar(searchChar, false);

            return s.Substring(i + 1);
		}

        public static int FindChar(this string s, char searchChar, bool startLeft = true) {
            int length = s.Length;
            int i = startLeft ? 0 : length - 1;
            if(startLeft) {
                for (; i < length; i++) {
                    char c = s[i];
                    if (c == searchChar) {
                        return i;
                    }
                }
            }
			else {
                for (; i >= 0; i--) {
                    char c = s[i];
                    if (c == searchChar) {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static string RemoveNameSpace(this string s, char searchChar = '.', bool removeSearchChar = true) {
            int i = s.FindChar(searchChar);

            if (removeSearchChar)
                i++;

            return s.Substring(i);
		}
        public static string AddSpaces(this string s)
        {
            int start = 0;
            int end = 0;
            string finalString = "";
            for (int i = 1; i < s.Length; i++)
            {
                if (s[i].IsUpper() || s[i].IsNumber())
                {
                    if (s[i - 1].IsUpper())
                    {
                        int j = 0;
                        while (i + j < s.Length - 1 && s[i + j].IsUpper())
                        {
                            j++;
                        }
                        i += j - 1;
                    }
                    else if (s[i - 1].IsNumber())
                    {
                        int j = 0;
                        while (i + j < s.Length - 1 && s[i + j].IsNumber())
                        {
                            j++;
                        }
                        i += j - 1;
                    }
                    end = i - 1;
                    finalString += s.Substring(start, end - start + 1) + " ";
                    start = end + 1;
                }
                else if (i == s.Length - 1)
                {
                    end = i;
                    finalString += s.Substring(start, end - start + 1);
                    start = -1;
                }
            }
            if (start != -1)
                finalString += s.Substring(start);
            return finalString;
        }
        public static string RemoveSpaces(this string s)
        {
            bool started = false;
            int start = 0;
            int end = 0;
            string finalString = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (started)
                {
                    if (s[i] == ' ')
                    {
                        started = false;
                        end = i;
                        finalString += s.Substring(start, end - start);
                    }
                }
                else
                {
                    if (s[i] != ' ')
                    {
                        started = true;
                        start = i;
                    }
                }
            }
            if (started)
                finalString += s.Substring(start, s.Length - start);
            return finalString;
        }
        public static string CapitalizeFirst(this string s)
        {
            if (s.Length > 0)
            {
                if (s[0].IsLower())
                    for (int i = 0; i < apla[0].Length; i++)
                    {
                        if (s[0] == apla[0][i])
                        {
                            char c = apla[1][i];
                            return c + s.Substring(1);
                        }
                    }
            }
            return s;
        }
        public static string ToFieldName(this string s)
        {
            if (s.Length > 0)
            {
                if (s[0].IsUpper())
                    for (int i = 0; i < apla[0].Length; i++)
                    {
                        if (s[0] == apla[1][i])
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
            foreach (string s in l1)
            {
                foreach (string s2 in l2)
                {
                    if (s2.IndexOf(s) > -1)
                    {
                        matches++;
                    }
                }
            }
            return matches;
        }
        public static bool IsSameEnchantedItem(this Item item1, Item item2)
        {
            if (item1 != null && item2 != null)
            {
                if (!item1.IsAir && !item2.IsAir)
                {
                    if (item1.TryGetGlobalItem(out EnchantedItem global1))
                    {
                        if (item2.TryGetGlobalItem(out EnchantedItem global2))
                        {
                            if (item1.type == item2.type &&/* global1.experience == global2.experience &&*/ global1.PowerBoosterInstalled == global2.PowerBoosterInstalled/* && item1.value == item2.value*/ && item1.prefix == item2.prefix && global1.infusedItemName == global2.infusedItemName)
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
            }
            return false;
        }
        public static void RoundCheck(this StatModifier statModifier, ref float value, int baseValue, StatModifier appliedStatModifier, int contentSample)
        {
            if (UtilityMethods.debugging) ($"\\/RoundCheck").Log();
            if (value > baseValue)
            {
                float checkValue = (float)((int)value + 1) * 1f / statModifier.ApplyTo(1f);
                if ((int)Math.Round(checkValue) == baseValue)
                {
                    float sampleValue = WEPlayer.CombineStatModifier(appliedStatModifier, statModifier, true).ApplyTo(contentSample);
                    if ((int)Math.Round(sampleValue) == baseValue)
                        value += 0.5f;
                }
            }
            if (UtilityMethods.debugging) ($"/\\RoundCheck").Log();
        }
        public static float GetReductionFactor(int hp)
        {
            float factor = hp < 7000 ? hp / 1000f + 1f : 8f;
            return factor;
        }
        public static int DamageBeforeArmor(this Item item, bool crit)
        {
            return (int)Math.Round(item.AEI("Damage", (float)item.damage * (crit ? 2f : 1f)));
        }
        public static void RemoveUntilPositive(this Item item, Player player)
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
                                    item.G().enchantments[k] = player.GetItem(player.whoAmI, iGlobal.enchantments[k], GetItemSettings.LootAllSettings);
                                }
                                if (!iGlobal.enchantments[k].IsAir)
                                {
                                    player.QuickSpawnItem(player.GetSource_Misc("PlayerDropItemCheck"), iGlobal.enchantments[k]);
                                    iGlobal.enchantments[k] = new Item();
                                }
                            }
                            Main.NewText("Your " + item.Name + "' level is too low to use that many enchantments.");
                        }//Check too many enchantments on item
                    }
                }
            }
        }
        public static void ApplyEnchantment(this Item item, int i)
        {
            if (UtilityMethods.debugging) ($"\\/ApplyEnchantment(i: " + i + ")").Log();
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (!item.IsAir)
            {
                EnchantedItem iGlobal = item.G();
                Enchantment enchantment = (Enchantment)(iGlobal.enchantments[i].ModItem);
                item.UpdateEnchantment(ref enchantment, i);
                wePlayer.UpdateItemStats(ref item);
            }
            if (UtilityMethods.debugging) ($"/\\ApplyEnchantment(i: " + i + ")").Log();
        }
        public static void UpdateDamageType(this Item item, DamageClass type)
        {
            item.DamageType = type;
        }
        public static void ApplyAllowedList(this Item item, Enchantment enchantment, ref float add, ref float mult, ref float flat, ref float @base)
        {
            if (WEMod.IsWeaponItem(item))
            {
                if (enchantment.AllowedList.ContainsKey("Weapon"))
                {
                    add *= enchantment.AllowedList["Weapon"];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList["Weapon"];
                    flat *= enchantment.AllowedList["Weapon"];
                    @base *= enchantment.AllowedList["Weapon"];
                    return;
                }
                else
                {
                    add = 1f;
                    mult = 1f;
                    flat = 0f;
                    @base = 0f;
                }
            }
            if (WEMod.IsArmorItem(item))
            {
                if (enchantment.AllowedList.ContainsKey("Armor"))
                {
                    add *= enchantment.AllowedList["Armor"];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList["Armor"];
                    flat *= enchantment.AllowedList["Armor"];
                    @base *= enchantment.AllowedList["Armor"];
                    return;
                }
                else
                {
                    add = 1f;
                    mult = 1f;
                    flat = 0f;
                    @base = 0f;
                }
            }
            if (WEMod.IsAccessoryItem(item))
            {
                if (enchantment.AllowedList.ContainsKey("Accessory"))
                {
                    add *= enchantment.AllowedList["Accessory"];
                    mult = 1f + (mult - 1f) * enchantment.AllowedList["Accessory"];
                    flat *= enchantment.AllowedList["Accessory"];
                    @base *= enchantment.AllowedList["Accessory"];
                    return;
                }
                else
                {
                    add = 1f;
                    mult = 1f;
                    flat = 0f;
                    @base = 0f;
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
                if (numCoinsToSpawn > 0)
                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), coinType, numCoinsToSpawn);
                coins %= coinValue;
                coinType--;
                coinValue /= 100;
            }
        }
        public static void LogNT(this string s) {
            s += reporteMessage;

            if(Main.netMode < NetmodeID.Server)
                Main.NewText(s);

            s.Log();
		}
        public static void Log(this string s)
        {
            UpdateSpaces(s);
            ModContent.GetInstance<WEMod>().Logger.Info(s.AddWS());
            UpdateSpaces(s, true);
        }
        public static void LogT(this string s)
        {
            UpdateSpaces(s);
            foreach (string key in logsT.Keys)
            {
                if (logsT[key] + 59 < Main.GameUpdateCount)
                    logsT.Remove(key);
            }
            if (!logsT.ContainsKey(s))
            {
                ModContent.GetInstance<WEMod>().Logger.Info(s.AddWS());
                logsT.Add(s, Main.GameUpdateCount);
            }
            UpdateSpaces(s, true);
        }
        public static void UpdateSpaces(string s, bool atEnd = false)
        {
            if (atEnd && s.Substring(0, 2) == "\\/")
                spaces++;
            else if (!atEnd && s.Substring(0, 2) == "/\\")
                spaces--;
        }
        public static string AddWS(this string s) => new string('|', spaces) + s;
        public static void CombineEnchantedItems(this Item item, ref List<Item> consumedItems)
        {
            if (consumedItems.Count > 0)
            {
                for (int c = 0; c < consumedItems.Count; c++)
                {
                    Item consumedItem = consumedItems[c];
                    if (!consumedItem.IsAir)
                    {
                        if (consumedItem.TryGetGlobalItem(out EnchantedItem cGlobal))
                        {
                            if (cGlobal.Experience > 0 || cGlobal.PowerBoosterInstalled)
                            {
                                if (item.TryGetGlobalItem(out EnchantedItem iGlobal))
                                {
                                    item.CheckConvertExcessExperience(consumedItem);
                                    if(iGlobal.infusionPower < cGlobal.infusionPower && item.GetWeaponInfusionPower() < cGlobal.infusionPower)
                                        item.TryInfuseItem(consumedItem);
                                    if (cGlobal.PowerBoosterInstalled)
                                    {
                                        if (!iGlobal.PowerBoosterInstalled)
                                            iGlobal.PowerBoosterInstalled = true;
                                        else
                                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);
                                    }
                                    //iGlobal.UpdateLevelAndValue();
                                    int j;
                                    for (j = 0; j <= EnchantingTable.maxEnchantments; j++)
                                    {
                                        if (j > 4)
                                            break;
                                        if (iGlobal.enchantments[j].IsAir)
                                            break;
                                    }
                                    for (int k = 0; k < EnchantingTable.maxEnchantments; k++)
                                    {
                                        if (!cGlobal.enchantments[k].IsAir)
                                        {
                                            Enchantment enchantment = ((Enchantment)cGlobal.enchantments[k].ModItem);
                                            int uniqueItemSlot = WEUIItemSlot.FindSwapEnchantmentSlot(enchantment, item);
                                            bool cantFit = false;
                                            if (enchantment.GetLevelCost() <= iGlobal.GetLevelsAvailable())
                                            {
                                                if (uniqueItemSlot == -1)
                                                {
                                                    if (enchantment.IsUtility && iGlobal.enchantments[4].IsAir && (WEMod.IsWeaponItem(item) || WEMod.IsArmorItem(item)))
                                                    {
                                                        iGlobal.enchantments[4] = cGlobal.enchantments[k].Clone();
                                                        item.ApplyEnchantment(j);
                                                    }
                                                    else if (j < 4)
                                                    {
                                                        iGlobal.enchantments[j] = cGlobal.enchantments[k].Clone();
                                                        item.ApplyEnchantment(j);
                                                        j++;
                                                    }
                                                    else
                                                    {
                                                        cantFit = true;
                                                    }
                                                }
                                                else
                                                {
                                                    cantFit = true;
                                                }
                                            }
                                            else
                                            {
                                                cantFit = true;
                                            }
                                            if (cantFit)
                                            {
                                                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), cGlobal.enchantments[k].type, 1);
                                            }
                                        }
                                        cGlobal.enchantments[k] = new Item();
                                    }
                                }
                                else
                                {
                                    item.CheckConvertExcessExperience(consumedItem);
                                    int numberEssenceRecieved;
                                    int xpCounter = iGlobal.Experience;
                                    for (int tier = EnchantingTable.maxEssenceItems - 1; tier >= 0; tier--)
                                    {
                                        numberEssenceRecieved = xpCounter / (int)EnchantmentEssence.xpPerEssence[tier] * 4 / 5;
                                        xpCounter -= (int)EnchantmentEssence.xpPerEssence[tier] * numberEssenceRecieved;
                                        if (xpCounter < (int)EnchantmentEssence.xpPerEssence[0] && xpCounter > 0 && tier == 0)
                                        {
                                            xpCounter = 0;
                                            numberEssenceRecieved += 1;
                                        }
                                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), EnchantmentEssence.IDs[tier], 1);
                                    }
                                    if (cGlobal.PowerBoosterInstalled)
                                    {
                                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);
                                    }
                                    for (int k = 0; k < EnchantingTable.maxEnchantments; k++)
                                    {
                                        if (!cGlobal.enchantments[k].IsAir)
                                        {
                                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), cGlobal.enchantments[k].type, 1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (consumedItem.ModItem is Enchantment)
                                {
                                    int size = ((Enchantment)consumedItem.ModItem).EnchantmentTier;
                                    if (size < 2)
                                    {
                                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), Containment.IDs[size], 1);
                                    }
                                    else if (size == 3)
                                    {
                                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), 180, 2);
                                    }
                                }
                                else if (consumedItem.ModItem is Containment containment)
                                {
                                    if (containment.size == 2 && item.type == Containment.barIDs[0, 2])
                                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), 180, 4);
                                }
                            }
                        }
                    }
                }
                consumedItems.Clear();
            }
        }
        public static void CheckConvertExcessExperience(this Item item, Item consumedItem)
		{
            if(item.TryGetGlobalItem(out EnchantedItem iGlobal) && consumedItem.TryGetGlobalItem(out EnchantedItem cGlobal))
			{
                long xp = (long)iGlobal.Experience + (long)cGlobal.Experience;
                if (xp <= (long)int.MaxValue)
                    iGlobal.Experience += cGlobal.Experience;
                else
                {
                    iGlobal.Experience = int.MaxValue;
                    WeaponEnchantmentUI.ConvertXPToEssence((int)(xp - (double)int.MaxValue), true);
                }
            }
			else
			{
                string error = $"Failed to CheckConvertExcessExperience(item: {item.S()}, consumedItem: {consumedItem.S()}) Please inform andro951(Weapon Enchantments) and give a description of what you were doing.";
                Main.NewText(error);
                error.Log();
            }
        }

        /// <summary>
		/// Randomly selects an item from the list if the chance is higher than the randomly generated float.<br/>
        /// <c>This can be done with var rand = new WeightedRandom<Item>(Main.rand)<br/>
        /// rand.Add(item, chance)<br/>
        /// rand.Add(new Item(), chanceN) //chance to get nothing<br/>
        /// Item chosen = rand.Get()<br/></c>
		/// </summary>
		/// <param name="options">Posible items to be selected.</param>
		/// <param name="chance">Chance to select an item from the list.</param>
		/// <returns>Item selected or null if chance was less than the generated float.</returns>
		public static T GetOneFromList<T>(List<T> options, float chance) where T : new() {
            //Example: items contains 4 items and chance = 0.4f (40%)
            float randFloat = Main.rand.NextFloat();//Example randFloat = 0.24f
            if (randFloat < chance) {
                float count = options.Count;// = 4f
                float chancePerItem = chance / count;// chancePerItem = 0.4f / 4f = 0.1f.  (10% chance each item)  
                int chosenItemNum = (int)(randFloat / chancePerItem);// chosenItemNum = (int)(0.24f / 0.1f) = (int)(2.4f) = 2.
                return options[chosenItemNum];// items[2] being the 3rd item in the list.
            }
            else {
                //If the chance is less than the generated float, return null.
                return new T();
            }
        }
    }
}
