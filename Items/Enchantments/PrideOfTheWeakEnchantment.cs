﻿using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class PrideOfTheWeakEnchantment : Enchantment, IStoreAppliedItem
	{
		public override int StrengthGroup => 23;
		public override int LowestCraftableTier => 0;
		public override float CapacityCostMultiplier => 0f;
		public override float ScalePercent => -1f;
		public override bool Max1 => true;
		public override void GetMyStats() {
			Effects = new() {
				new PrideOfTheWeak(multiplicative: EnchantmentStrengthData)
			};

			AllowedList = new() {
				{ EItemType.Weapons, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(percent: false, multiply100: false, multiplicative: true);
		public EnchantedItem EnchantedItem { get; set; }
		public override float EnchantmentStrength => EnchantedItem != null ? 1f + (EnchantmentStrengthData.Value - 1f) * EnchantedItem.GetPrideOfTheWeakMultiplier() : EnchantmentStrengthData.Value;
		public override string Artist => "Zorutan";
		public override string ArtModifiedBy => "andro951";
		public override string Designer => "andro951";
	}
	public class PrideOfTheWeakEnchantmentBasic : PrideOfTheWeakEnchantment
	{
		public override SellCondition SellCondition => SellCondition.Always;
		public override List<DropData> NpcAIDrops => new() {
			new(NPCAIStyleID.Slime, chance: 0.05f)
		};
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Chest_Normal, chance: 0.2f)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden, chance: 0.2f)
		};
	}
	public class PrideOfTheWeakEnchantmentCommon : PrideOfTheWeakEnchantment { }
	public class PrideOfTheWeakEnchantmentRare : PrideOfTheWeakEnchantment { }
	public class PrideOfTheWeakEnchantmentEpic : PrideOfTheWeakEnchantment { }
	public class PrideOfTheWeakEnchantmentLegendary : PrideOfTheWeakEnchantment { }

}