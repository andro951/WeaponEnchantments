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

namespace WeaponEnchantments.Common.Utility
{
    public static class UtilityMethods {

		#region GetModClasses

		public static EnchantedItem GetEnchantedItem(this Item item) {
            if(item != null && item.TryGetEnchantedItem(out EnchantedItem iGlobal)) {
                iGlobal.Item = item;
                return iGlobal;
            }

            return null;
        }
        public static WEPlayer GetWEPlayer(this Player player) => player.GetModPlayer<WEPlayer>();
        public static WEProjectile GetWEProjectile(this Projectile projectile) => projectile.GetGlobalProjectile<WEProjectile>();
        public static bool TryGetWEProjectile(this Projectile projectile, out WEProjectile pGlobal) {
            if(projectile != null && projectile.TryGetGlobalProjectile(out pGlobal)) {
                return true;
			}
			else {
                pGlobal = null;
                return false;
			}
        }
        public static WEGlobalNPC GetWEGlobalNPC(this NPC npc) => npc.GetGlobalNPC<WEGlobalNPC>();
        //public static bool TryGetEnchantedItem(this Item item) => item != null && item.TryGetGlobalItem(out EnchantedItem iGlobal);
        public static bool TryGetEnchantedItem(this Item item) => item != null && (item.TryGetGlobalItem(out EnchantedWeapon w) || item.TryGetGlobalItem(out EnchantedArmor a) || item.TryGetGlobalItem(out EnchantedAccessory c));
        /*public static bool TryGetEnchantedItem(this Item item, out EnchantedItem iGlobal) {
            if (item != null && item.TryGetGlobalItem(out iGlobal)) {
                iGlobal.Item = item;
                return true;
            }
			else {
                iGlobal = null;
                return false;
			}
        }*/
        public static bool TryGetEnchantedItemSearchAll(this Item item, out EnchantedItem iGlobal) {
            iGlobal = null;
            if (item == null)
                return false;

            if (item.TryGetGlobalItem(out EnchantedWeapon enchantedWeapon)) {
                iGlobal = enchantedWeapon;
            }
            else if (item.TryGetGlobalItem(out EnchantedArmor enchantedArmor)){
                iGlobal = enchantedArmor;
			}
            else if (item.TryGetGlobalItem(out EnchantedAccessory enchantedAccessory)) {
                iGlobal = enchantedAccessory;
			}

            if (iGlobal != null) {
                iGlobal.Item = item;
                return true;
            }

            return false;
        }
        public static bool TryGetEnchantedEquipItem(this Item item, out EnchantedEquipItem iGlobal) {
            iGlobal = null;
            if (item == null)
                return false;
            
            if (item.TryGetGlobalItem(out EnchantedArmor enchantedArmor)){
                iGlobal = enchantedArmor;
			}
            else if (item.TryGetGlobalItem(out EnchantedAccessory enchantedAccessory)) {
                iGlobal = enchantedAccessory;
			}

            if (iGlobal != null) {
                iGlobal.Item = item;
                return true;
            }

            return false;
        }
        public static bool TryGetEnchantedWeapon(this Item item, out EnchantedWeapon w) {
            if (item != null && item.TryGetGlobalItem(out w)) {
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
        public static bool TryGetEnchantedItem <T>(this Item item, out T enchantedWeapon) where T : EnchantedItem {
            enchantedWeapon = null;
            if (item == null || item.IsAir)
                return false;
            
            item.TryGetGlobalItem(out enchantedWeapon);
            if (enchantedWeapon == null) {
                item.TryGetEnchantedItemSearchAll(out EnchantedItem enchantedItem);
                enchantedWeapon = enchantedItem as T;
            }

            if (enchantedWeapon != null) {
                enchantedWeapon.Item = item;
                return true;
            }

            return false;
        }
        
        public static Item Enchantments(this Item item, int i) {
            if(item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return iGlobal.enchantments[i];

            return null;
        }
        public static Enchantment EnchantmentsModItem(this Item item, int i) {
            if (item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return (Enchantment)iGlobal.enchantments[i].ModItem;

            return null;
        }
        public static Item ItemInUI(this WEPlayer wePlayer, int i = 0) => wePlayer.enchantingTableUI.itemSlotUI[i].Item;
        public static WEUIItemSlot ItemUISlot(this WEPlayer wePlayer, int i = 0) => wePlayer.enchantingTableUI.itemSlotUI[i];
        public static WEUIItemSlot EnchantmentUISlot(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlotUI[i];
        public static Item EnchantmentInUI(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
        public static Enchantment EnchantmentsModItem(this WEPlayer wePlayer, int i) => (Enchantment)wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.ModItem;
        public static Item EssenceInTable(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.essenceSlotUI[i].Item;
        public static bool TryGetEnchantmentEssence(this Item item, out EnchantmentEssence enchantmentEssence) {
            ModItem modItem = item.ModItem;

            if(modItem != null && modItem is EnchantmentEssence essence) {
                enchantmentEssence = essence;
                return true;
			}

            enchantmentEssence = null;

            return false;
		}

        #endregion

        #region Stats

        public static float ApplyStatModifier(this Item item, string key, float value) {
            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return value;
            if (iGlobal.appliedStatModifiers.ContainsKey(key))
                return iGlobal.appliedStatModifiers[key].ApplyTo(value);

            return value;
        }
        public static float ApplyEStat(this Item item, string key, float value) {
            if (!item.TryGetEnchantedItem(out EnchantedItem iGlobal))
                return value;

            if (iGlobal.appliedEStats.ContainsKey(key))
                return iGlobal.appliedEStats[key].ApplyTo(value);

            return value;
            //Main.LocalPlayer.GetModPlayer<WEPlayer>().eStats.ContainsKey(key) ? item.G().eStats[key].ApplyTo(value) : value;
        }
        public static bool ContainsEStatOnPlayer(this Player player, string key) {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            if (wePlayer.eStats.ContainsKey(key))
                return true;
            Item weapon = wePlayer.trackedWeapon;
            if (weapon.TryGetEnchantedItem(out EnchantedItem iGlobal) && iGlobal.eStats.ContainsKey(key))
                return true;
            return false;
        }
        public static float ApplyEStatFromPlayer(this Player player, string key, float value) {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            StatModifier combinedStatModifier = StatModifier.Default;
            if (wePlayer.eStats.ContainsKey(key))
                combinedStatModifier = wePlayer.eStats[key];

            Item weapon = wePlayer.trackedWeapon;
            if (weapon.TryGetEnchantedItem(out EnchantedItem iGlobal) && iGlobal.eStats.ContainsKey(key))
                combinedStatModifier = combinedStatModifier.CombineWith(iGlobal.eStats[key]);

            return combinedStatModifier.ApplyTo(value);
        }
        public static float ApplyEStatFromPlayer(this Player player, Item item, string key, float value) {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            StatModifier combinedStatModifier = StatModifier.Default;
            if (wePlayer.eStats.ContainsKey(key))
                combinedStatModifier = wePlayer.eStats[key];

            if (item.TryGetEnchantedItem(out EnchantedItem iGlobal) && iGlobal.eStats.ContainsKey(key))
                combinedStatModifier = combinedStatModifier.CombineWith(iGlobal.eStats[key]);

            return combinedStatModifier.ApplyTo(value);
        }
        public static bool ContainsEStat(this Item item, string key) => item.TryGetEnchantedItem(out EnchantedItem iGlobal) && iGlobal.eStats.ContainsKey(key);
        public static bool ContainsEStat(string key) => Main.LocalPlayer.GetModPlayer<WEPlayer>().eStats.ContainsKey(key);
        public static bool ContainsEStat(this Player player, string key, Item item) => player.GetWEPlayer().eStats.ContainsKey(key) || item.TryGetEnchantedItem(out EnchantedItem iGlobal) && iGlobal.eStats.ContainsKey(key);
        public static string RemoveInvert(this string s) => s.Length > 2 ? s.Substring(0, 2) == "I_" ? s.Substring(2) : s : s;
        public static string RemovePrevent(this string s) => s.Length > 2 ? s.Substring(0, 2) == "P_" ? s.Substring(2) : s : s;
        public static bool ContainsInvert(this string s) => s.Length > 2 ? s.Substring(0, 2) == "I_" : false;

		#endregion

		#region General

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
        public static void CheckConvertExcessExperience(this Item item, Item consumedItem) {
            if(item.TryGetEnchantedItem(out EnchantedItem iGlobal) && consumedItem.TryGetEnchantedItem(out EnchantedItem cGlobal)) {
                long xp = (long)iGlobal.Experience + (long)cGlobal.Experience;
                if (xp <= (long)int.MaxValue) {
                    iGlobal.Experience += cGlobal.Experience;
                }
                else {
                    iGlobal.Experience = int.MaxValue;
                    if (WEMod.magicStorageEnabled) $"CheckConvertExcessExperience. item: {item.S()}, consumedItem: {consumedItem.S()}".Log();
                    WeaponEnchantmentUI.ConvertXPToEssence((int)(xp - (long)int.MaxValue), true);
                }
            }
			else {
                $"Failed to CheckConvertExcessExperience(item: {item.S()}, consumedItem: {consumedItem.S()})".LogNT(ChatMessagesIDs.FailedCheckConvertExcessExperience);
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
		public static T GetOneFromList<T>(this List<T> options, float chance) where T : new() {
            if (options.Count == 0)
                return new T();

            if (chance <= 0f)
                return new T();
            
            if(chance > 1f)
                chance = 1f;

            //Example: items contains 4 items and chance = 0.4f (40%)
            float randFloat = Main.rand.NextFloat();//Example randFloat = 0.24f
            if (randFloat < chance) {
                float count = options.Count;// = 4f
                float chancePerItem = chance / count;// chancePerItem = 0.4f / 4f = 0.1f.  (10% chance each item)  
                int chosenItemNum = (int)(randFloat / chancePerItem);// chosenItemNum = (int)(0.24f / 0.1f) = (int)(2.4f) = 2.

                return options[chosenItemNum];// items[2] being the 3rd item in the list.
            }
            else {
                //If the chance is less than the generated float, return new.
                return new T();
            }
        }
        public static float Percent(this float value) {
            return (float)Math.Round(value * 100, 1);
        }
        public static void AddOrCombine(this Dictionary<byte, EStatModifier> dictionary, byte key, EStatModifier newValue) {
            if (dictionary.ContainsKey(key)) {
                dictionary[key].CombineWith(newValue);
			}
			else {
                dictionary.Add(key, newValue);
			}
		}
        public static void AddOrCombine(this SortedDictionary<EnchantmentStat, EStatModifier> dictionary, EStatModifier newValue) {
            EnchantmentStat key = newValue.StatType;
            if (dictionary.ContainsKey(key)) {
                dictionary[key].CombineWith(newValue);
            }
            else {
                dictionary.Add(key, newValue);
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
        public static void AddOrCombine<T>(this SortedDictionary<short, BuffStats> dictionary, T buffEffect) where T : BuffEffect {
            short key = buffEffect.BuffStats.BuffID;
            if (dictionary.ContainsKey(key)) {
                dictionary[key].CombineNoReturn(buffEffect.BuffStats);
			}
			else {
                dictionary.Add(key, buffEffect.BuffStats.Clone());
			}
        }
        public static void AddOrCombine(this SortedDictionary<short, BuffStats> dictionary, BuffStats buffStat) {
            short key = buffStat.BuffID;
            if (dictionary.ContainsKey(key)) {
                dictionary[key].CombineNoReturn(buffStat);
            }
            else {
                dictionary.Add(key, buffStat.Clone());
            }
        }
        //public static void ApplyTo(this StatModifier statModifier, ref float value) {
        //    value = (value + statModifier.Base) * statModifier.Additive * statModifier.Multiplicative + statModifier.Flat;
        //}
        public static bool NullOrAir(this Item item) => item?.IsAir ?? true;

		#endregion
	}
}
