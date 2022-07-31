using System.Collections.Generic;
using Terraria;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Items;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.UI;

namespace WeaponEnchantments.Common.Utility
{
    public static class UtilityMethods {

		#region GetModClasses

		public static EnchantedItem GetEnchantedItem(this Item item) {
            EnchantedItem iGlobal = item.GetGlobalItem<EnchantedItem>();
            iGlobal.Item = item;
            return iGlobal;
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
        public static bool TryGetEnchantedItem(this Item item) => item != null && item.TryGetGlobalItem(out EnchantedItem iGlobal);
        public static bool TryGetEnchantedItem(this Item item, out EnchantedItem iGlobal) {
            if (item != null && item.TryGetGlobalItem(out iGlobal)) {
                iGlobal.Item = item;
                return true;
            }
			else {
                iGlobal = null;
                return false;
			}
        }
        public static Item Enchantments(this Item item, int i) => item.GetEnchantedItem().enchantments[i];
        public static Enchantment EnchantmentsModItem(this Item item, int i) => (Enchantment)item.GetEnchantedItem().enchantments[i].ModItem;
        public static Item ItemInUI(this WEPlayer wePlayer, int i = 0) => wePlayer.enchantingTableUI.itemSlotUI[i].Item;
        public static WEUIItemSlot EnchantmentsInUI(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlotUI[i];
        public static Item Enchantments(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item;
        public static Enchantment EnchantmentsModItem(this WEPlayer wePlayer, int i) => (Enchantment)wePlayer.enchantingTableUI.enchantmentSlotUI[i].Item.ModItem;
        public static Item EnchantmentEssence(this WEPlayer wePlayer, int i) => wePlayer.enchantingTableUI.essenceSlotUI[i].Item;

		#endregion

		#region Stats

		public static float ApplyStatModifier(this Item item, string key, float value) => item.GetEnchantedItem().appliedStatModifiers.ContainsKey(key) ? item.GetEnchantedItem().appliedStatModifiers[key].ApplyTo(value) : value;
        public static float ApplyEStat(this Item item, string key, float value) {
            if (item.GetEnchantedItem().appliedEStats.ContainsKey(key))
                return item.GetEnchantedItem().appliedEStats[key].ApplyTo(value);

            return value;
            //Main.LocalPlayer.GetModPlayer<WEPlayer>().eStats.ContainsKey(key) ? item.G().eStats[key].ApplyTo(value) : value;
        }
        public static bool ContainsEStatOnPlayer(this Player player, string key) {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            if (wePlayer.eStats.ContainsKey(key))
                return true;
            Item weapon = wePlayer.trackedWeapon;
            if (weapon != null && WEMod.IsWeaponItem(weapon) && weapon.GetEnchantedItem().eStats.ContainsKey(key))
                return true;
            return false;
        }
        public static float ApplyEStatFromPlayer(this Player player, string key, float value)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            StatModifier combinedStatModifier = StatModifier.Default;
            if (wePlayer.eStats.ContainsKey(key))
                combinedStatModifier = wePlayer.eStats[key];
            Item weapon = wePlayer.trackedWeapon;
            if (weapon != null && !weapon.IsAir && weapon.GetEnchantedItem().eStats.ContainsKey(key))
                combinedStatModifier = combinedStatModifier.CombineWith(weapon.GetEnchantedItem().eStats[key]);
            return combinedStatModifier.ApplyTo(value);
        }
        public static float ApplyEStatFromPlayer(this Player player, Item item, string key, float value)
        {
            WEPlayer wePlayer = player.GetModPlayer<WEPlayer>();
            StatModifier combinedStatModifier = StatModifier.Default;
            if (wePlayer.eStats.ContainsKey(key))
                combinedStatModifier = wePlayer.eStats[key];
            if (item != null && !item.IsAir && WEMod.IsEnchantable(item) && item.GetEnchantedItem().eStats.ContainsKey(key))
                combinedStatModifier = combinedStatModifier.CombineWith(item.GetEnchantedItem().eStats[key]);
            return combinedStatModifier.ApplyTo(value);
        }
        public static bool ContainsEStat(this Item item, string key) => item.GetEnchantedItem().eStats.ContainsKey(key);
        public static bool ContainsEStat(string key) => Main.LocalPlayer.GetModPlayer<WEPlayer>().eStats.ContainsKey(key);
        public static bool ContainsEStat(this Player player, string key, Item item) => player.GetWEPlayer().eStats.ContainsKey(key) || item != null && !item.IsAir && WEMod.IsEnchantable(item) && item.GetEnchantedItem().eStats.ContainsKey(key);
        public static string RemoveInvert(this string s) => s.Length > 2 ? s.Substring(0, 2) == "I_" ? s.Substring(2) : s : s;
        public static string RemovePrevent(this string s) => s.Length > 2 ? s.Substring(0, 2) == "P_" ? s.Substring(2) : s : s;
        public static bool ContainsInvert(this string s) => s.Length > 2 ? s.Substring(0, 2) == "I_" : false;

		#endregion

		#region General

		public static void SpawnCoins(int coins) {
            int coinType = ItemID.PlatinumCoin;
            int coinValue = 1000000;
            while (coins > 0) {
                int numCoinsToSpawn = coins / coinValue;
                if (numCoinsToSpawn > 0)
                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc("PlayerDropItemCheck"), coinType, numCoinsToSpawn);

                coins %= coinValue;
                coinType--;
                coinValue /= 100;
            }
        }
        public static void CheckConvertExcessExperience(this Item item, Item consumedItem) {
            if(item.TryGetGlobalItem(out EnchantedItem iGlobal) && consumedItem.TryGetGlobalItem(out EnchantedItem cGlobal)) {
                long xp = (long)iGlobal.Experience + (long)cGlobal.Experience;
                if (xp <= (long)int.MaxValue) {
                    iGlobal.Experience += cGlobal.Experience;
                }
                else {
                    iGlobal.Experience = int.MaxValue;
                    WeaponEnchantmentUI.ConvertXPToEssence((int)(xp - (long)int.MaxValue), true);
                }
            }
			else {
                $"Failed to CheckConvertExcessExperience(item: {item.S()}, consumedItem: {consumedItem.S()})".LogNT();
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

		#endregion
	}
}
