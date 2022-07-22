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
        public static EnchantedItem G(this Item item) => item.GetGlobalItem<EnchantedItem>();
        ///<summary>
        ///Gets this item's enchantemnt at index i.  Gets (AllForOneEnchantmentBasic)item.G().enchantments[i].ModItem
        ///</summary>
        public static WEPlayer G(this Player player) => player.GetModPlayer<WEPlayer>();
        public static WEProjectile G(this Projectile projectile) => projectile.GetGlobalProjectile<WEProjectile>();
        public static WEGlobalNPC G(this NPC npc) => npc.GetGlobalNPC<WEGlobalNPC>();
        public static bool TG(this Item item) => item != null && item.TryGetGlobalItem(out EnchantedItem iGlobal);
        public static bool TG(this Item item, out EnchantedItem iGlobal) {
            if (item != null && item.TryGetGlobalItem(out iGlobal))
                return true;
			else {
                iGlobal = null;
                return false;
			}
        }
        public static Item E(this Item item, int i) => item.G().enchantments[i];
        public static AllForOneEnchantmentBasic EM(this Item item, int i) => (AllForOneEnchantmentBasic)item.G().enchantments[i].ModItem;
        ///<summary>
        ///Gets item in the enchanting table itemslot.  Gets wePlayer.enchantingTableUI.itemSlot[i].Item
        ///</summary>
        public static Item I(this WEPlayer wePlayer, int i = 0) => wePlayer.enchantingTableUI.itemSlotUI[i].Item;
        ///<summary>
        ///Gets enchantment in the enchanting table in enchantment slot i.  wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item
        ///</summary>
        public static WEUIItemSlot EUI(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlotUI[i];
        ///<summary>
        ///Gets enchantment in the enchanting table in enchantment slot i.  wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item
        ///</summary>
        public static Item E(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
        ///<summary>
        ///Gets enchantment in the enchanting table in enchantment slot i.  (AllForOneEnchantmentBasic)wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.ModItem
        ///</summary>
        public static AllForOneEnchantmentBasic EM(this WEPlayer wePlayer, int i) => (AllForOneEnchantmentBasic)wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.ModItem;
        ///<summary>
        ///Gets essence in the enchanting table in essence slot i.  
        ///</summary>
        public static Item Es(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
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
        public static string S(this AllForOneEnchantmentBasic enchantment) => enchantment != null ? enchantment.Name : "null";
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
                            if (item1.type == item2.type &&/* global1.experience == global2.experience &&*/ global1.powerBoosterInstalled == global2.powerBoosterInstalled/* && item1.value == item2.value*/ && item1.prefix == item2.prefix && global1.infusedItemName == global2.infusedItemName)
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
                AllForOneEnchantmentBasic enchantment = (AllForOneEnchantmentBasic)(iGlobal.enchantments[i].ModItem);
                item.UpdateEnchantment(ref enchantment, i);
                wePlayer.UpdateItemStats(ref item);
            }
            if (UtilityMethods.debugging) ($"/\\ApplyEnchantment(i: " + i + ")").Log();
        }
        public static void UpdateDamageType(this Item item, int type)
        {
            switch ((DamageTypeSpecificID)type)
            {
                case DamageTypeSpecificID.Default:
                    item.DamageType = DamageClass.Default;
                    break;
                case DamageTypeSpecificID.Generic:
                    item.DamageType = DamageClass.Generic;
                    break;
                case DamageTypeSpecificID.Melee:
                    item.DamageType = DamageClass.Melee;
                    break;
                case DamageTypeSpecificID.MeleeNoSpeed:
                    item.DamageType = DamageClass.MeleeNoSpeed;
                    break;
                case DamageTypeSpecificID.Ranged:
                    item.DamageType = DamageClass.Ranged;
                    break;
                case DamageTypeSpecificID.Magic:
                    item.DamageType = DamageClass.Magic;
                    break;
                case DamageTypeSpecificID.Summon:
                    item.DamageType = DamageClass.Summon;
                    break;
                case DamageTypeSpecificID.SummonMeleeSpeed:
                    item.DamageType = DamageClass.SummonMeleeSpeed;
                    break;
                case DamageTypeSpecificID.MagicSummonHybrid:
                    item.DamageType = DamageClass.MagicSummonHybrid;
                    break;
                case DamageTypeSpecificID.Throwing:
                    item.DamageType = DamageClass.Throwing;
                    break;
            }
        }
        public static void ApplyAllowedList(this Item item, AllForOneEnchantmentBasic enchantment, ref float add, ref float mult, ref float flat, ref float @base)
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
                            if (cGlobal.experience > 0 || cGlobal.powerBoosterInstalled)
                            {
                                if (item.TryGetGlobalItem(out EnchantedItem iGlobal))
                                {
                                    item.CheckConvertExcessExperience(consumedItem);
                                    if(iGlobal.infusionPower < cGlobal.infusionPower && item.GetWeaponInfusionPower() < cGlobal.infusionPower)
                                        item.TryInfuseItem(consumedItem);
                                    if (cGlobal.powerBoosterInstalled)
                                    {
                                        if (!iGlobal.powerBoosterInstalled)
                                            iGlobal.powerBoosterInstalled = true;
                                        else
                                            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), ModContent.ItemType<PowerBooster>(), 1);
                                    }
                                    iGlobal.UpdateLevel();
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
                                            AllForOneEnchantmentBasic enchantment = ((AllForOneEnchantmentBasic)cGlobal.enchantments[k].ModItem);
                                            int uniqueItemSlot = WEUIItemSlot.FindSwapEnchantmentSlot(enchantment, item);
                                            bool cantFit = false;
                                            if (enchantment.GetLevelCost() <= iGlobal.GetLevelsAvailable())
                                            {
                                                if (uniqueItemSlot == -1)
                                                {
                                                    if (enchantment.Utility && iGlobal.enchantments[4].IsAir && (WEMod.IsWeaponItem(item) || WEMod.IsArmorItem(item)))
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
                                    int xpCounter = iGlobal.experience;
                                    for (int tier = EnchantingTable.maxEssenceItems - 1; tier >= 0; tier--)
                                    {
                                        numberEssenceRecieved = xpCounter / (int)EnchantmentEssenceBasic.xpPerEssence[tier] * 4 / 5;
                                        xpCounter -= (int)EnchantmentEssenceBasic.xpPerEssence[tier] * numberEssenceRecieved;
                                        if (xpCounter < (int)EnchantmentEssenceBasic.xpPerEssence[0] && xpCounter > 0 && tier == 0)
                                        {
                                            xpCounter = 0;
                                            numberEssenceRecieved += 1;
                                        }
                                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), EnchantmentEssenceBasic.IDs[tier], 1);
                                    }
                                    if (cGlobal.powerBoosterInstalled)
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
                                if (consumedItem.ModItem is AllForOneEnchantmentBasic)
                                {
                                    int size = ((AllForOneEnchantmentBasic)consumedItem.ModItem).EnchantmentSize;
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
                long xp = (long)iGlobal.experience + (long)cGlobal.experience;
                if (xp <= (long)int.MaxValue)
                    iGlobal.experience += cGlobal.experience;
                else
                {
                    iGlobal.experience = int.MaxValue;
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
    }
}
