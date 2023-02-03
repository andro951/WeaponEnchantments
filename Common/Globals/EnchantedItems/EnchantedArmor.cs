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
	public class EnchantedArmor : EnchantedEquipItem
	{
		#region Infusion

		public int infusedArmorSlot = -1;
		public Item infusedItem;

		#endregion
		public override bool InstancePerEntity => true;
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => IsArmorItem(entity);
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
			if (!inEnchantingTable)
				return;

			base.UpdateEquip(item, player);

			//Fix for swapping an equipped armor/accessory with one in the enchanting table.
			if (player.GetWEPlayer().ItemInUI().TryGetEnchantedItem()) {
				if (item.GetInfusionArmorSlot() != infusedArmorSlot) {
					infusedArmorSlot = -1;
					item.TryInfuseItem(new Item(), true);
				}
			}
		}
		protected override string GetInfusedItemTooltip(Item item) => $"Infused Armor ID: {item.GetInfusionArmorSlot()}   Infused Item: {infusedItemName}";
		protected override string GetInfusionTooltip(Item item) => $"Set Bonus ID: {item.GetInfusionArmorSlot(true)}";
		protected override string GetNewInfusedItemTooltip(Item item, WEPlayer wePlayer) {
			return 
				$"*New Set Bonus ID: {wePlayer.infusionConsumeItem.GetInfusionArmorSlot()}   " +
				$"New Infused Item: {wePlayer.infusionConsumeItem.GetInfusionItemName()}*";
		}

		public override Dictionary<string, string>[] SkillPointsToNames() =>
			new Dictionary<string, string>[] {
				new Dictionary<string, string>() {
					{ "Skill", "Resilience" },
					{ "Scaling", "+0.25% Damage Resistance / Level" },
					{ "Milestone1", "+2.5% Damage resistance" },
					{ "Milestone2", "+10 Defense" },
					{ "Milestone3", "Getting hit increases your damage resistance, which stacks, but decays after some time" }
				},
				new Dictionary<string, string>() {
					{ "Skill", "Determination" },
					{ "Scaling", "+0.1 HP/s / Level" },
					{ "Milestone1", "+1 Health/s" },
					{ "Milestone2", "+50 Max health" },
					{ "Milestone3", "Regeneration increases based on health missing " }
				},
				new Dictionary<string, string>() {
					{ "Skill", "Deftness" },
					{ "Scaling", "+0.25% Dodge Chance / Level" },
					{ "Milestone1", "+2.5% Dodge Chance" },
					{ "Milestone2", "Dodges increase attack speed for a short moment" },
					{ "Milestone3", "Guarantees a dodge every five minutes, stacks with others" }
				}
			};

		public override void SkillPointsToStats()
		{
			throw new NotImplementedException();
		}
	}
}
