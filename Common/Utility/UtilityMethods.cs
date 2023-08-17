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
			if (fullKey.Lang(out string result, L_ID1.Tooltip, L_ID2.EnchantmentEffects, args))
				return result;

			return "";
		}

		#endregion

		#region General
        /*
		public static Item CSI(this int type) => ContentSamples.ItemsByType[type];
		public static NPC CSNPC(this int netID) => ContentSamples.NpcsByNetId[netID];
		public static void ReplaceItemWithCoins(ref Item item, int coins) {
            int coinType = ItemID.PlatinumCoin;
            int coinValue = 1000000;
            while (coins > 0) {
                int numCoinsToSpawn = coins / coinValue;
                if (numCoinsToSpawn > 0) {
                    item = new Item(coinType, numCoinsToSpawn + 1);
                    return;
                }

                coins %= coinValue;
                coinType--;
                coinValue /= 100;
            }
        }
        public static void SpawnCoins(this Player player, int coinValue) {
            int valuePerCoin = 1000000;
            for (int i = 3; i >= 0; i--) {
                int coins = coinValue / valuePerCoin;
                coinValue %= valuePerCoin;
                valuePerCoin /= 100;
                if (coins > 0)
                    player.QuickSpawnItem(player.GetSource_GiftOrReward(), ItemID.CopperCoin + i, coins);
            }
        }
        */
        public static void CheckConvertExcessExperience(this Item item, Item consumedItem) {
            if (item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem) && consumedItem.TryGetEnchantedItemSearchAll(out EnchantedItem cGlobal)) {
                long xp = (long)enchantedItem.Experience + (long)cGlobal.Experience;
                if (xp <= (long)int.MaxValue) {
                    enchantedItem.Experience += cGlobal.Experience;
                }
                else {
                    enchantedItem.Experience = int.MaxValue;
                    if (WEMod.magicStorageEnabled) $"CheckConvertExcessExperience. item: {item.S()}, consumedItem: {consumedItem.S()}".Log();
                    EnchantingTableUI.ConvertXPToEssence((int)(xp - (long)int.MaxValue), true, item);
                }
            }
            else {
				if (consumedItem.NullOrAir())
					return;

				$"Failed to CheckConvertExcessExperience(item: {item.S()}, consumedItem: {consumedItem.S()})".LogNT(ChatMessagesIDs.FailedCheckConvertExcessExperience);
            }
        }
        /*
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
		public static T GetOneFromList<T>(this List<T> options, float chance = 1f) where T : new() {
            if (options.Count == 0)
                return new T();

            if (chance <= 0f)
                return new T();

            if (chance > 1f)
                chance = 1f;

            //Example: items contains 4 items and chance = 0.4f (40%)
            float randFloat = Main.rand.NextFloat();//Example randFloat = 0.24f
            if (randFloat < chance) {
                float count = options.Count;// = 4f
                float chancePerItem = chance / count;// chancePerItem = 0.4f / 4f = 0.1f.  (10% chance each item)  
                int chosenItemNum = (int)(randFloat / chancePerItem);// chosenItemNum = (int)(0.24f / 0.1f) = (int)(2.4f) = 2.

                return options[chosenItemNum];// items[2] being the 3rd item in the list.
            }

            //If the chance is less than the generated float, return new.
            return new T();
        }
        public static T GetOneFromWeightedList<T>(this List<(float, T)> options, float chance) where T : new() {
            if (options.Count == 0)
                return new T();

            if (chance <= 0f)
                return new T();

            if (chance > 1f)
                chance = 1f;

            float randFloat = Main.rand.NextFloat();
            if (randFloat <= chance) {
                float total = 0f;
                foreach ((float, T) pair in options) {
                    total += pair.Item1;
                }

                total *= randFloat / chance;

                foreach ((float, T) pair in options) {
                    total -= pair.Item1;
                    if (total <= 0f)
                        return pair.Item2;
                }
            }

            return new T();
        }
        public static T GetOneFromWeightedList<T>(this SortedList<int, T> options, float chance) where T : new() {
            List<(float, T)> newList = new List<(float, T)>();
            foreach (KeyValuePair<int, T> pair in options) {
                newList.Add(((float)pair.Key, pair.Value));
            }

            return newList.GetOneFromWeightedList(chance);
        }
        */
        public static int GetOneFromWeightedList(this IEnumerable<WeightedPair> options, float chance) {
            if (options.Count() == 0)
                return 0;

            if (chance <= 0f)
                return 0;

            if (chance > 1f)
                chance = 1f;

            float randFloat = Main.rand.NextFloat();
            if (randFloat <= chance) {
                float total = 0f;
                foreach (WeightedPair pair in options) {
                    total += pair.Weight;
                }

                total *= randFloat / chance;

                foreach (WeightedPair pair in options) {
                    total -= pair.Weight;
                    if (total <= 0f)
                        return pair.ID;
                }
            }

            return 0;
        }
        public static int GetOneFromWeightedList(this IEnumerable<DropData> options, float chance) {
            if (options.Count() == 0)
                return 0;

            if (chance <= 0f)
                return 0;

            if (chance > 1f)
                chance = 1f;

            float randFloat = Main.rand.NextFloat();
            if (randFloat <= chance) {
                float total = 0f;
                foreach (DropData pair in options) {
                    total += pair.Weight;
                }

                total *= randFloat / chance;

                foreach (DropData pair in options) {
                    total -= pair.Weight;
                    if (total <= 0f)
                        return pair.ID;
                }
            }

            return 0;
        }
        /*
        public static float Percent(this float value) => value * 100f;
        public static string PercentString(this float value) => $"{(value * 100).S()}%";
        public static string Lang(this string s, string m) => s.Lang(out string result, m) ? result : "";
        public static bool Lang(this string s, out string result, string m) {
            string key = $"Mods.WeaponEnchantments.{m}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang(this string s, L_ID1 id = L_ID1.Tooltip) => s.Lang(out string result, id) ? result : "";
        public static bool Lang(this string s, out string result, L_ID1 id = L_ID1.Tooltip) {
            string key = $"Mods.WeaponEnchantments.{id}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang(this string s, L_ID1 id, string m) => s.Lang(out string result, id, m) ? result : "";
        public static bool Lang(this string s, out string result, L_ID1 id, string m) {
            string key = $"Mods.WeaponEnchantments.{id}.{m}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang(this string s, L_ID1 id, L_ID2 id2) => s.Lang(out string result, id, id2) ? result : "";
        public static bool Lang(this string s, out string result, L_ID1 id, L_ID2 id2) {
            string key = $"Mods.WeaponEnchantments.{id}.{id2}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang(this string s, L_ID1 id, L_ID2 id2, string m) => s.Lang(out string result, id, id2, m) ? result : "";
        public static bool Lang(this string s, out string result, L_ID1 id, L_ID2 id2, string m) {
            string key = $"Mods.WeaponEnchantments.{id}.{id2}.{m}.{s}";
            result = Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang(this int i, L_ID_V id) {
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

        public static string Lang(this string s, string m, IEnumerable<string> args) => s.Lang(out string result, m, args) ? result : "";
        public static bool Lang(this string s, out string result, string m, IEnumerable<string> args) {
            string key = $"Mods.WeaponEnchantments.{m}.{s}";
            result = Language.GetTextValue(key, args);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang(this string s, L_ID1 id, L_ID2 id2, IEnumerable<object> args) => s.Lang(out string result, id, id2, args) ? result : "";
        public static bool Lang(this string s, out string result, L_ID1 id, L_ID2 id2, IEnumerable<object> args) {
            string key = $"Mods.WeaponEnchantments.{id}.{id2}.{s}";
            result = args != null ? Language.GetTextValue(key, args.ToArray()) : Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string Lang(this string s, L_ID1 id, IEnumerable<object> args) => s.Lang(out string result, id, args) ? result : "";
        public static bool Lang(this string s, out string result, L_ID1 id, IEnumerable<object> args) {
            string key = $"Mods.WeaponEnchantments.{id}.{s}";
            result = args != null ? Language.GetTextValue(key, args.ToArray()) : Language.GetTextValue(key);

            if (result == key) {
                return false;
            }

            return true;
        }
        public static string GetTextValue(this string key, IEnumerable<object> args) {
            return Language.GetTextValue(key, args);
        }
        */

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
        /*
        public static void AddOrCombine(this IDictionary<int, int> dict1, IDictionary<int, int> dict2) {
            foreach (var pair in dict2) {
                dict1.AddOrCombine(pair);
            }
        }
        public static void AddOrCombine(this IDictionary<int, int> dict1, KeyValuePair<int, int> pair) {
            int key = pair.Key;
            if (dict1.ContainsKey(key)) {
                dict1[key] += pair.Value;
            }
            else {
                dict1.Add(key, pair.Value);
            }
        }
        public static void AddOrCombine<T>(this IDictionary<T, int> dict1, (T, int) pair) =>
            dict1.AddOrCombine(pair.Item1, pair.Item2);
        public static void AddOrCombine<T>(this IDictionary<T, int> dict, T key, int value) {
            if (dict.ContainsKey(key)) {
                dict[key] += value;
            }
            else {
                dict.Add(key, value);
            }
        }
        public static void TrySubtractRemove<T>(this IDictionary<T, int> dict, T key, int value) {
            if (dict.ContainsKey(key)) {
                dict[key] -= value;
                if (dict[key] <= 0)
                    dict.Remove(key);
            }
        }
        public static void AddOrCombine<T>(this IDictionary<T, List<int>> dict, T key, int value) {
			if (dict.ContainsKey(key)) {
				dict[key].Add(value);
			}
			else {
				dict.Add(key, new() { value });
			}
		}
        public static void AddOrCombine<TKey, T>(this IDictionary<TKey, HashSet<T>> dict, TKey key, T value) {
            if (dict.ContainsKey(key)) {
                dict[key].Add(value);
            }
            else {
                dict.Add(key, new() { value });
            }
        }
        public static void AddOrCombine<TKey, T>(this IDictionary<TKey, SortedSet<T>> dict, TKey key, T value) {
            if (dict.ContainsKey(key)) {
                dict[key].Add(value);
            }
            else {
                dict.Add(key, new() { value });
            }
        }
        public static void AddOrCombine<TKey, T>(this IDictionary<TKey, HashSet<T>> dict, TKey key, HashSet<T> value) {
            if (dict.ContainsKey(key)) {
                dict[key] = dict[key].Concat(value).ToHashSet();
            }
            else {
                dict.Add(key, value);
            }
        }
        public static void AddOrCombine<TKey, T>(this IDictionary<TKey, (HashSet<T>, HashSet<T>)> dict, TKey key, (HashSet<T>, HashSet<T>) value) {
            if (dict.ContainsKey(key)) {
                dict[key].Item1.UnionWith(value.Item1);
                dict[key].Item2.UnionWith(value.Item2);
            }
            else {
                dict.Add(key, value);
            }
        }
        public static void AddOrCombine<TKey, T1, T2>(this IDictionary<TKey, List<(T1, T2)>> dict, TKey key, (T1, T2) value) {
            if (dict.ContainsKey(key)) {
                dict[key].Add(value);
            }
            else {
                dict.Add(key, new List<(T1, T2)> { value });
            }
        }
		public static void AddOrCombineSetOrKeepHigher<TKey, TKey2>(this SortedDictionary<TKey, Dictionary<TKey2, int>> dict, TKey key, Dictionary<TKey2, int> dict2) {
            if (dict.ContainsKey(key)) {
                foreach(TKey2 key2 in dict2.Keys) {
                    dict[key].SetOrKeepHigher(key2, dict2[key2]);
				}
            }
            else {
                dict.Add(key, dict2);
            }
        }

        public static void AddOrCombineAddCheckOverflow<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key, T newValue) {
			if (dictionary.ContainsKey(key)) {
				dictionary[key] = WEMath.AddCheckOverflow(newValue, dictionary[key]);
			}
			else {
				dictionary.Add(key, newValue);
			}
		}
        public static void SetValue<TKey, T>(this IDictionary<TKey, T> dictionary, TKey key, T value) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key] = value;
            }
            else {
                dictionary.Add(key, value);
            }
        }
        public static void SetOrKeepHigher<TKey>(this IDictionary<TKey, int> dictionary, TKey key, int value) {
            if (dictionary.ContainsKey(key)) {
                if (dictionary[key] < value)
                    dictionary[key] = value;
            }
            else {
                dictionary.Add(key, value);
            }
        }
        public static void SetValue<TKey, T>(this Dictionary<TKey, T> dictionary, TKey key, T value) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key] = value;
            }
            else {
                dictionary.Add(key, value);
            }
		}
		public static bool ContainsHashSet(this HashSet<HashSet<int>> ingredients, HashSet<int> requiredItemType) {
			bool contains = false;
			foreach (HashSet<int> ingredientType in ingredients) {
				contains = true;
				if (ingredientType.Count != requiredItemType.Count) {
					contains = false;
				}
				else {
					foreach (int eachIngredientType in ingredientType) {
						if (!requiredItemType.Contains(eachIngredientType)) {
							contains = false;
							break;
						}
					}
				}

				if (contains)
					break;
			}

			return contains;
		}
		public static void CombineHashSet(this HashSet<HashSet<int>> set1, HashSet<HashSet<int>> set2) {
            foreach (HashSet<int> set in set2) {
                if (!set1.ContainsHashSet(set))
                    set1.Add(set);
            }
        }
        public static void TryAdd(this HashSet<HashSet<int>> sets, HashSet<int> newSet) {
			if (!sets.ContainsHashSet(newSet))
				sets.Add(newSet);
		}
        public static int RoundNearest(this float f, int mult) {
            float r = f % mult;
            int result = (int)(f - r);
            if (r >= mult / 2f)
                result += mult;

            return result;
        }
        */
		#endregion

		/*
        public static void Combine<T>(this List<T> list, List<T> list2) {
            foreach(T item in list2) {
                list.Add(item);
			}
		}
        */

		/*
		public static bool NullOrAir(this Item item) => item?.IsAir ?? true;
        public static SortedList<TKey, TValue> CombineSortedLists<TKey, TValue>(this SortedList<TKey, TValue> list1, SortedList<TKey, TValue> list2) {
            SortedList <TKey, TValue> newList = new SortedList <TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> pair in list1) {
                newList.Add(pair.Key, pair.Value);
			}

            foreach (KeyValuePair<TKey, TValue> pair in list2) {
                newList.Add(pair.Key, pair.Value);
            }

            return newList;
        }
        */
        public static SortedList<EnchantmentStat, T> ToSortedList<T>(this IEnumerable<T> list) where T : class, IEnchantmentStat {
            SortedList<EnchantmentStat, T> newList = new SortedList<EnchantmentStat, T>();
            foreach (T i in list) {
                newList.Add(i.statName, i);
			}
            
            return newList;
		}
		/*
        public static void Clamp(this ref int value, int min = 0, int max = 100) {
            value = value < min ? min : value > max ? max : value;
		}
        public static void Clamp(this ref float value, float min = 0f, float max = 1f) {
            value = value < min ? min : value > max ? max : value;
        }
        public static void CombineLists(this List<Item> list1, IEnumerable<Item> list2, bool noDuplicates = false) {
            List<int> list1Types = list1.Select(i => i.type).ToList();
            List<int> list2Types = list2.Select(i => i.type).ToList();
            foreach (Item item2 in list2) {
                if (!noDuplicates || !list1Types.Contains(item2.type))
                    list1.Add(item2);
			}
		}
        */
		public static Type TypeAboveModItem(this Item item) {
			if (item == null)
				return null;

			ModItem modItem = item.ModItem;
			if (modItem == null)
				return null;

			return modItem.TypeBeforeModItem();
		}
		public static Type TypeBeforeModItem(this ModItem modItem) {
            Type type = modItem.GetType();
            bool foundListUniqueType = false;
            while (!foundListUniqueType) {
                if (type.BaseType == typeof(WEModItem) || type.BaseType.BaseType != typeof(WEModItem) && type.BaseType == typeof(ModItem) ) {
                    foundListUniqueType = true;
                }
                else {
                    type = type.BaseType;
                }
            }

            return type;
        }
        /*
        public static Type TypeAboveGrandParent(this object child, Type parent) => child.GetType().TypeAboveGrandParent(parent);
        public static Type TypeAboveGrandParent(this Type child, Type parent) {
            Type type = child;
            bool foundListUniqueType = false;
            while (!foundListUniqueType && type.BaseType != null) {
                if (type.BaseType == parent) {
                    foundListUniqueType = true;
				}
				else {
                    type = type.BaseType;
				}
			}

            return type;
		}
        public static int ChildNumber(this object child, Type parent) => child.GetType().ChildNumber(parent);
        public static int ChildNumber(this Type child, Type parent) {
            Type type = child;
            int i = 0;
			while (type.BaseType != null) {
                if (type.BaseType == parent) {
                    return i;
				}
                else {
                    type = type.BaseType;
				}

                i++;
			}

            return -1;
		}
        public static bool ValidOwner(this Projectile projectile, out Player player) {
            player = null;
            if (projectile.owner >= 0 && projectile.owner < Main.player.Length) {
                player = Main.player[projectile.owner];
                return true;
			}
            
            return false;
        }
        */

        #endregion
    }
}
