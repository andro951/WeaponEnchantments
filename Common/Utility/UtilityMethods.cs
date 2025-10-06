using System.Collections.Generic;
using Terraria;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.UI;
using System;
using System.Linq;
using static WeaponEnchantments.WEPlayer;
using System.Reflection;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using WeaponEnchantments.Effects;
using Terraria.Localization;
using WeaponEnchantments.Tiles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using androLib.Common.Utility;
using androLib;

namespace WeaponEnchantments.Common.Utility
{
    public static class UtilityMethods {

        #region GetModClasses

        public static EnchantedItem GetEnchantedItem(this Item item) {
            if (item != null && item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem)) {
                enchantedItem.Item = item;
                return enchantedItem;
            }

            return null;
        }
        public static WEPlayer GetWEPlayer(this Player player) => player.GetModPlayer<WEPlayer>();
        public static bool TryGetWEPlayer(this Projectile projectile, out WEPlayer wePlayer) {
            int owner = projectile.owner;
            if (owner >= 0 && owner < Main.player.Length) {
                Main.player[owner].TryGetModPlayer(out wePlayer);
                return wePlayer != null;
            }

            wePlayer = null;
            return false;
        }
        public static bool TryGetWEPlayer(this Player player, out WEPlayer wePlayer) => player.TryGetModPlayer(out wePlayer);
        public static ProjectileWithSourceItem GetMyGlobalProjectile(this Projectile projectile) {
            if (projectile.TryGetGlobalProjectile(out WEProjectile wEProjectile))
                return wEProjectile;

            if (projectile.TryGetGlobalProjectile(out BobberProjectile bobberProjectile))
                return bobberProjectile;

            return null;
        }
        public static bool TryGetWEProjectile(this Projectile projectile, out WEProjectile pGlobal) {
            if (projectile != null && projectile.TryGetGlobalProjectile(out pGlobal)) {
                return true;
            }
            else {
                pGlobal = null;
                return false;
            }
        }
        public static bool TryGetProjectileWithSourceItem(this Projectile projectile, out ProjectileWithSourceItem projectileWithSourceItem) {
            if (projectile.TryGetGlobalProjectile(out WEProjectile wEProjectile)) {
                projectileWithSourceItem = wEProjectile;
                return true;
            }

            if (projectile.TryGetGlobalProjectile(out BobberProjectile bobberProjectile)) {
                projectileWithSourceItem = bobberProjectile;
                return true;
            }

            projectileWithSourceItem = null;
            return false;
        }
        public static WEGlobalNPC GetWEGlobalNPC(this NPC npc) => npc.GetGlobalNPC<WEGlobalNPC>();
        public static bool TryGetWEGlobalNPC(this NPC npc, out WEGlobalNPC weGlobalNPC) => npc.TryGetGlobalNPC(out weGlobalNPC);
        public static bool TryGetEnchantedItem(this Item item) => item != null && (item.TryGetGlobalItem(out EnchantedWeapon w) || item.TryGetGlobalItem(out EnchantedArmor a) || item.TryGetGlobalItem(out EnchantedAccessory ac) || item.TryGetGlobalItem(out EnchantedFishingPole fp) || item.TryGetGlobalItem(out EnchantedTool t));
        public static bool TryGetEnchantedItemSearchAll(this Item item, out EnchantedItem enchantedItem) {
            enchantedItem = null;
            if (item == null)
                return false;

            if (item.TryGetGlobalItem(out EnchantedWeapon enchantedWeapon)) {
                enchantedItem = enchantedWeapon;
            }
            else if (item.TryGetGlobalItem(out EnchantedArmor enchantedArmor)) {
                enchantedItem = enchantedArmor;
            }
            else if (item.TryGetGlobalItem(out EnchantedAccessory enchantedAccessory)) {
                enchantedItem = enchantedAccessory;
            }
            else if (item.TryGetGlobalItem(out EnchantedFishingPole enchantedFishingPole)) {
                enchantedItem = enchantedFishingPole;
            }
            else if (item.TryGetGlobalItem(out EnchantedTool enchantedTool)) {
                enchantedItem = enchantedTool;
            }

            if (enchantedItem != null) {
                enchantedItem.Item = item;
                return true;
            }

            return false;
        }
        public static bool TryGetEnchantedHeldItem(this Item item, out EnchantedHeldItem enchantedHeldItem) {
			enchantedHeldItem = null;
			if (item == null)
				return false;

			if (item.TryGetGlobalItem(out EnchantedWeapon enchantedWeapon)) {
				enchantedHeldItem = enchantedWeapon;
			}
			else if (item.TryGetGlobalItem(out EnchantedFishingPole enchantedFishingPole)) {
				enchantedHeldItem = enchantedFishingPole;
			}
			else if (item.TryGetGlobalItem(out EnchantedTool enchantedTool)) {
				enchantedHeldItem = enchantedTool;
			}

			if (enchantedHeldItem != null) {
				enchantedHeldItem.Item = item;
				return true;
			}

			return false;
		}
        public static bool TryGetEnchantedEquipItem(this Item item, out EnchantedEquipItem enchantedItem) {
            enchantedItem = null;
            if (item == null)
                return false;

            if (item.TryGetGlobalItem(out EnchantedArmor enchantedArmor)) {
                enchantedItem = enchantedArmor;
            }
            else if (item.TryGetGlobalItem(out EnchantedAccessory enchantedAccessory)) {
                enchantedItem = enchantedAccessory;
            }

            if (enchantedItem != null) {
                enchantedItem.Item = item;
                return true;
            }

            return false;
        }
        public static bool TryGetEnchantedWeapon(this Item item, out EnchantedWeapon w) {
            if (!item.NullOrAir() && item.TryGetGlobalItem(out w)) {
                w.Item = item;
                return true;
            }
            else {
                w = null;
                return false;
            }
        }
        public static bool TryGetEnchantedArmor(this Item item, out EnchantedArmor a) {
            if (item != null && item.TryGetGlobalItem(out a)) {
                a.Item = item;
                return true;
            }
            else {
                a = null;
                return false;
            }
        }
        public static bool TryGetEnchantedAccessory(this Item item, out EnchantedAccessory a) {
            if (item != null && item.TryGetGlobalItem(out a)) {
                a.Item = item;
                return true;
            }
            else {
                a = null;
                return false;
            }
        }
        public static bool TryGetEnchantedItem<T>(this Item item, out T enchantedItem) where T : EnchantedItem {
            enchantedItem = null;
            if (item.NullOrAir())
                return false;
            
            item.TryGetGlobalItem(out enchantedItem);
            if (enchantedItem == null) {
                item.TryGetEnchantedItemSearchAll(out EnchantedItem searchAllEnchantedItem);
                enchantedItem = searchAllEnchantedItem as T;
            }

            if (enchantedItem != null) {
                enchantedItem.Item = item;
                return true;
            }

            return false;
        }
        public static Item Enchantments(this Item item, int i) {
            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
                return enchantedItem.enchantments[i];

            return null;
        }
        public static Enchantment EnchantmentsModItem(this Item item, int i) {
            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem))
                return (Enchantment)enchantedItem.enchantments[i].ModItem;

            return null;
        }
        public static bool TryGetEnchantmentEssence(this Item item, out EnchantmentEssence enchantmentEssence) {
            ModItem modItem = item.ModItem;

            if (modItem != null && modItem is EnchantmentEssence essence) {
                enchantmentEssence = essence;
                return true;
            }

            enchantmentEssence = null;

            return false;
		}
		public static string GetEffectTooltip(this EnchantmentEffect enchantmentEffect, IEnumerable<object> args, string key = null) {
			string fullKey = key != null ? $"{enchantmentEffect.TooltipName}.{key}" : enchantmentEffect.TooltipName;
			if (fullKey.Lang_WE(out string result, L_ID1.Tooltip, L_ID2.EnchantmentEffects, args))
				return result;

			return "";
		}

		#endregion

		#region General

        public static void CheckConvertExcessExperience(this Item item, Item consumedItem) {
            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && consumedItem.TryGetEnchantedItemSearchAll(out EnchantedItem cGlobal)) {
                long xp = (long)enchantedItem.Experience + (long)cGlobal.Experience;
                if (xp <= (long)int.MaxValue) {
                    enchantedItem.Experience += cGlobal.Experience;
                }
                else {
                    enchantedItem.Experience = int.MaxValue;
                    EnchantingTableUI.ConvertXPToEssence((int)(xp - (long)int.MaxValue), true, item);
                }
            }
            else {
				if (consumedItem.NullOrAir())
					return;

				GameMessageTextID.FailedConvertExcessExperience.ToString().Lang_WE(L_ID1.GameMessages, new object[] { item.S(), consumedItem.S() }).LogNT(ChatMessagesIDs.FailedCheckConvertExcessExperience);// $"Failed to CheckConvertExcessExperience(item: {item.S()}, consumedItem: {consumedItem.S()})".LogNT_WE(ChatMessagesIDs.FailedCheckConvertExcessExperience);
            }
        }
        public static List<Item> SearchForItemIncludeEnchantmentStorage(int itemType, out int itemCount) => AndroUtilityMethods.SearchForItem(itemType, out itemCount, LocalWEPlayer.enchantmentStorageItems);


		public static string Lang_WE(this string s, L_ID1 id = L_ID1.Tooltip) => s.Lang_WE(out string result, id) ? result : "";
        public static bool Lang_WE(this string s, out string result, L_ID1 id = L_ID1.Tooltip) {
            string key = $"Mods.WeaponEnchantments.{id}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang_WE(this string s, L_ID1 id, string m) => s.Lang_WE(out string result, id, m) ? result : "";
        public static bool Lang_WE(this string s, out string result, L_ID1 id, string m) {
            string key = $"Mods.WeaponEnchantments.{id}.{m}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang_WE(this string s, L_ID1 id, L_ID2 id2) => s.Lang_WE(out string result, id, id2) ? result : "";
        public static bool Lang_WE(this string s, out string result, L_ID1 id, L_ID2 id2) {
            string key = $"Mods.WeaponEnchantments.{id}.{id2}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang_WE(this string s, L_ID1 id, L_ID2 id2, string m) => s.Lang_WE(out string result, id, id2, m) ? result : "";
        public static bool Lang_WE(this string s, out string result, L_ID1 id, L_ID2 id2, string m) {
            string key = $"Mods.WeaponEnchantments.{id}.{id2}.{m}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang_WE(this int i, L_ID_V id) {
            switch (id) {
                case L_ID_V.Item:
                    return Terraria.Lang.GetItemNameValue(i);
                case L_ID_V.NPC:
                    return Terraria.Lang.GetNPCNameValue(i);
                case L_ID_V.Buff:
                    return Terraria.Lang.GetBuffName(i);
                case L_ID_V.BuffDescription:
                    return Terraria.Lang.GetBuffDescription(i);
            }

            return null;
        }

        public static string Lang_WE(this string s, string m, IEnumerable<string> args) => s.Lang_WE(out string result, m, args) ? result : "";
        public static bool Lang_WE(this string s, out string result, string m, IEnumerable<string> args) {
            string key = $"Mods.WeaponEnchantments.{m}.{s}";
            result = Language.GetTextValue(key, args);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang_WE(this string s, L_ID1 id, L_ID2 id2, IEnumerable<object> args) => s.Lang_WE(out string result, id, id2, args) ? result : "";
        public static bool Lang_WE(this string s, out string result, L_ID1 id, L_ID2 id2, IEnumerable<object> args) {
            string key = $"Mods.WeaponEnchantments.{id}.{id2}.{s}";
            result = args != null ? Language.GetTextValue(key, args.ToArray()) : Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang_WE(this string s, L_ID1 id, IEnumerable<object> args) => s.Lang_WE(out string result, id, args) ? result : "";
        public static bool Lang_WE(this string s, out string result, L_ID1 id, IEnumerable<object> args) {
            string key = $"Mods.WeaponEnchantments.{id}.{s}";
            result = args != null ? Language.GetTextValue(key, args.ToArray()) : Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }

        #region AddOrCombine

        public static void AddOrCombine(this Dictionary<byte, EStatModifier> dictionary, byte key, EStatModifier newValue) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key].CombineWith(newValue);
            }
            else {
                dictionary.Add(key, newValue.Clone());
            }
        }
        public static void AddOrCombine(this SortedDictionary<EnchantmentStat, EStatModifier> dictionary, EStatModifier newValue) {
            EnchantmentStat key = newValue.StatType;
            if (dictionary.ContainsKey(key)) {
                dictionary[key].CombineWith(newValue);
            }
            else {
                dictionary.Add(key, newValue.Clone());
            }
        }
        public static void AddOrCombine(this SortedDictionary<EnchantmentStat, bool> dictionary, BoolEffect newValue) {
            EnchantmentStat key = newValue.statName;
            if (dictionary.ContainsKey(key)) {
                if (dictionary[key] && !newValue.EnableStat)
                    dictionary[key] = false;
            }
            else {
                dictionary.Add(key, newValue.EnableStat);
            }
        }
        public static void AddOrCombine(this SortedDictionary<EnchantmentStat, bool> dictionary, (EnchantmentStat, bool) newValue) {
            EnchantmentStat key = newValue.Item1;
            if (dictionary.ContainsKey(key)) {
                if (dictionary.ContainsKey(key)) {
                    if (dictionary[key] && !newValue.Item2)
                        dictionary[key] = false;
                }
            }
            else {
                dictionary.Add(newValue.Item1, newValue.Item2);
            }
        }
        public static void AddOrCombine(this Dictionary<byte, int> dictionary, byte key, int newValue) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key] = newValue + dictionary[key];
            }
            else {
                dictionary.Add(key, newValue);
            }
        }
        public static void AddOrCombine(this Dictionary<byte, float> dictionary, byte key, int newValue) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key] = newValue + dictionary[key];
            }
            else {
                dictionary.Add(key, newValue);
            }
        }
        public static void AddOrCombine(this SortedDictionary<short, BuffStats> dictionary, BuffEffect buffEffect) {
            short key = buffEffect.BuffStats.BuffID;
            if (dictionary.ContainsKey(key)) {
                dictionary[key].CombineNoReturn(buffEffect.BuffStats.Clone());
            }
            else {
                dictionary.Add(key, buffEffect.BuffStats.Clone());
            }
        }
        public static void AddOrCombine(this SortedDictionary<short, BuffStats> dictionary, BuffStats buffStat) {
            short key = buffStat.BuffID;
            if (dictionary.ContainsKey(key)) {
                dictionary[key].CombineNoReturn(buffStat.Clone());
            }
            else {
                dictionary.Add(key, buffStat.Clone());
            }
        }
        public static void AddOrCombine<TKey>(this SortedDictionary<TKey, List<DropData>> dictionary, TKey key, DropData newValue) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key].Add(newValue);
            }
            else {
                dictionary.Add(key, new List<DropData>() { newValue });
            }
        }
		public static void AddOrCombine<TKey>(this SortedDictionary<TKey, List<ModDropData>> dictionary, TKey key, ModDropData newValue) {
			if (dictionary.ContainsKey(key)) {
				dictionary[key].Add(newValue);
			}
			else {
				dictionary.Add(key, new List<ModDropData>() { newValue });
			}
		}
		public static void AddOrCombine<TKey, T>(this SortedDictionary<TKey, List<(T, List<DropData>)>> dictionary, TKey key, (T, List<DropData>) newValue) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key].Add(newValue);
            }
            else {
                dictionary.Add(key, new List<(T, List<DropData>)>() { newValue });
            }
        }

		#endregion

        public static SortedList<EnchantmentStat, T> ToSortedList<T>(this IEnumerable<T> list) where T : class, IEnchantmentStat {
            SortedList<EnchantmentStat, T> newList = new SortedList<EnchantmentStat, T>();
            foreach (T i in list) {
                newList.Add(i.statName, i);
			}
            
            return newList;
		}

        #endregion
    }
}
