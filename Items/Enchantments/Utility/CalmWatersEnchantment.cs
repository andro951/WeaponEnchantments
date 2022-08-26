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

		public override string ShortTooltip => $"{Math.Round(EnchantmentStrength * AllowedListMultiplier, 3)}x {EnchantmentTypeName.AddSpaces()}";
		public override string Artist => "andro951";
		public override string Designer => "Creature";
	}
	public class CalmWatersEnchantmentBasic : CalmWatersEnchantment { }
	public class CalmWatersEnchantmentCommon : CalmWatersEnchantment { }
	public class CalmWatersEnchantmentRare : CalmWatersEnchantment { }
	public class CalmWatersEnchantmentSuperRare : CalmWatersEnchantment { }
	public class CalmWatersEnchantmentUltraRare : CalmWatersEnchantment { }

}
