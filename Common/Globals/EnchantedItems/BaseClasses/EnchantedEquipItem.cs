using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;
using WeaponEnchantments.Items;
using WeaponEnchantments.UI;
using static WeaponEnchantments.Common.Configs.ConfigValues;
using static WeaponEnchantments.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Items.Enchantment;

namespace WeaponEnchantments.Common.Globals
{
	public abstract class EnchantedEquipItem : EnchantedItem
	{
		#region Tracking (instance)

		public bool equippedInArmorSlot = false;

		#endregion

		public override void UpdateInventory(Item item, Player player) {

			equippedInArmorSlot = false;

			base.UpdateInventory(item, player);
        }
        public override void UpdateEquip(Item item, Player player) {
            if (!inEnchantingTable)
                return;

            //Fix for swapping an equipped armor/accessory with one in the enchanting table.
            if (player.GetWEPlayer().enchantingTableItem.TryGetEnchantedItem()) {
                inEnchantingTable = false;
            }
        }
		protected override string GetPerLevelBonusTooltip() {
			if (WEMod.serverConfig.DamageReductionPerLevelDisabled)
				return "";

			float damageReduction = GetPerLevelDamageReduction();
			string tooltip = damageReduction > 0f ? 
				$"+{damageReduction.PercentString()} {$"{EnchantmentStat.DamageReduction}".Lang(L_ID1.Tooltip, L_ID2.EffectDisplayName)}" : "";

			return tooltip;
		}
		public float GetPerLevelDamageReduction(float armorMultiplier = 0f, float accessoryMultiplier = 0f) {
			if (armorMultiplier <= 0f || accessoryMultiplier <= 0f) {
				armorMultiplier = ArmorDamageReductionPerLevel;
				accessoryMultiplier = AccessoryDamageReductionPerLevel;
			}

			float multiplier = Item.accessory ? accessoryMultiplier : armorMultiplier;
			float damageReduction = multiplier * levelBeforeBooster;

			return damageReduction;
		}
	}
}
