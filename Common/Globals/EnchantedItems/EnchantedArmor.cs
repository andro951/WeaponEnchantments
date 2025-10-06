using androLib.Common.Utility;
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
using static androLib.Common.EnchantingRarity;
using static WeaponEnchantments.Common.Globals.EnchantedItemStaticMethods;
using static WeaponEnchantments.Items.Enchantment;

namespace WeaponEnchantments.Common.Globals
{
	public class EnchantedArmor : EnchantedEquipItem
	{
		#region Infusion

		public const int DefaultInfusionArmorSlot = -1;
		public int infusedArmorSlot = DefaultInfusionArmorSlot;
		public Item infusedItem;

		#endregion
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.IsArmorItem();
		public override EItemType ItemType => EItemType.Armor;
		public override GlobalItem Clone(Item item, Item itemClone) {
			EnchantedArmor clone = (EnchantedArmor)base.Clone(item, itemClone);

			if (cloneReforgedItem || resetGlobals) {

				#region Infusion

				clone.infusedArmorSlot = infusedArmorSlot;
				clone.infusedItem = infusedItem?.Clone();

				#endregion
			}

			return clone;
		}
		public override void NetSend(Item item, BinaryWriter writer) {
			base.NetSend(item, writer);
			writer.Write(infusedArmorSlot);
			writer.Write(infusedItem?.type ?? -1);
		}
		public override void NetReceive(Item item, BinaryReader reader) {
			base.NetReceive(item, reader);
			infusedArmorSlot = reader.ReadInt32();
			int infusedItemType = reader.ReadInt32();
			infusedItem = infusedItemType > 0 ? new Item(infusedItemType) : null;
		}
		public override void UpdateEquip(Item item, Player player) {
			base.UpdateEquip(item, player);
			if (!inEnchantingTable)
				return;

			//Fix for swapping an equipped armor/accessory with one in the enchanting table.
			if (player.GetWEPlayer().enchantingTableItem.TryGetEnchantedItem()) {
				if (item.GetInfusionArmorSlot() != infusedArmorSlot) {
					infusedArmorSlot = DefaultInfusionArmorSlot;
					item.TryInfuseItem(new Item(), true);
				}
			}
		}
		protected override string GetInfusedItemTooltip(Item item) => $"{EnchantmentGeneralTooltipsID.InfusedArmorID}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips, new object[] { item.GetInfusionArmorSlot(), infusedItemName });//$"Infused Armor ID: {item.GetInfusionArmorSlot()}   Infused Item: {infusedItemName}";
		protected override string GetInfusionTooltip(Item item) => $"{EnchantmentGeneralTooltipsID.SetBonusID}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips, new object[] { item.GetInfusionArmorSlot(true) });// $"Set Bonus ID: {item.GetInfusionArmorSlot(true)}";
		protected override string GetNewInfusedItemTooltip(Item item, WEPlayer wePlayer) {
			return 
				$"*{$"{EnchantmentGeneralTooltipsID.NewSetBonusID}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} {wePlayer.infusionConsumeItem.GetInfusionArmorSlot()}   " +
				$"{$"{EnchantmentGeneralTooltipsID.NewInfusedItem}".Lang_WE(L_ID1.Tooltip, L_ID2.EnchantmentGeneralTooltips)} {wePlayer.infusionConsumeItem.GetInfusionItemName()}*";
		}
		public override void ResetInfusion(Item item) {
			base.ResetInfusion(item);
			infusedArmorSlot = DefaultInfusionArmorSlot;
			infusedItem = null;
		}
	}
}
