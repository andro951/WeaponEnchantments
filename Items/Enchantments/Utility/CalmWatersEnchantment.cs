using System;
using System.Collections.Generic;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class CalmWatersEnchantment : Enchantment {
		public override int StrengthGroup => 15;
		public override float ScalePercent => 0f;
		public override bool Max1 => true;
		public override float CapacityCostMultiplier => 1;
		public override void GetMyStats() {
			Effects = new() {
				new EnemyMaxSpawns(multiplicative: EnchantmentStrengthData),
				new EnemySpawnRate(multiplicative: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.FishingPoles, 1f }
			};
		}

		public override string ShortTooltip => GetShortTooltip(percent: false, multiply100: false, multiplicative: true);
		public override string Artist => "andro951";
		public override string ArtModifiedBy => null;
		public override string Designer => "Creature";
	}
	public class CalmWatersEnchantmentBasic : CalmWatersEnchantment
	{
		public override List<DropData> ChestDrops => new() {
			new(ChestID.Water)
		};
		public override List<DropData> CrateDrops => new() {
			new(CrateID.Wooden),
			new(CrateID.Pearlwood_WoodenHard)
		};
	}
	public class CalmWatersEnchantmentCommon : CalmWatersEnchantment { }
	public class CalmWatersEnchantmentRare : CalmWatersEnchantment { }
	public class CalmWatersEnchantmentEpic : CalmWatersEnchantment { }
	public class CalmWatersEnchantmentLegendary : CalmWatersEnchantment { }

}
