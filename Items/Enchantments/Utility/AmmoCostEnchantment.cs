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

		public override string ShortTooltip => GetShortTooltip(text: EnchantmentStrength > 0f ? "Chance To Not Consume Ammo" : "Increased Ammo Cost");

		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => null;
		public override string Designer => "andro951";
	}
	public class AmmoCostEnchantmentBasic : AmmoCostEnchantment
	{
		public override List<WeightedPair> NpcAIDrops => new() {
			new(NPCAIStyleID.Flying)
		};
		public override SortedDictionary<ChestID, float> ChestDrops => new() {
			{ ChestID.Chest_Normal, 1f },
			{ ChestID.WebCovered, 1f },
			{ ChestID.Mushroom, 1f },
			{ ChestID.Marble, 1f },
			{ ChestID.SandStone, 1f }
		};
		public override List<WeightedPair> CrateDrops => new() {
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
