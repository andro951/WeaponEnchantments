using System;
using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class AmmoCostEnchantment : Enchantment
	{
		public override void GetMyStats() {
			//AddEStat(EnchantmentTypeName, 0f, 1f, 0f, -EnchantmentStrength);
			Effects = new() {
				new AmmoCost(@base: EnchantmentStrengthData)
			};
		}

		public override string ShortTooltip => GetShortTooltip(text: GetLocalizationTypeName(EnchantmentTypeName + (EnchantmentStrength > 0f ? "1" : "2")));

		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class AmmoCostEnchantmentBasic : AmmoCostEnchantment
	{
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Flying)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal),
			new(ChestID.WebCovered),
			new(ChestID.Mushroom),
			new(ChestID.Marble),
			new(ChestID.SandStone)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, 0.5f),
			new(CrateID.Pearlwood_WoodenHard, 0.5f),
			new(CrateID.Iron, 0.5f),
			new(CrateID.Iron, 0.5f)
		};
	}
	public class AmmoCostEnchantmentCommon : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentRare : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentEpic : AmmoCostEnchantment { }
	public class AmmoCostEnchantmentLegendary : AmmoCostEnchantment { }
}
