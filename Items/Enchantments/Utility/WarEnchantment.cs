using System;
using System.Collections.Generic;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class WarEnchantment : Enchantment
	{
		public override string CustomTooltip =>
			"(Minion Damage is reduced by your spawn rate multiplier, from enchantments, unless they are your minion attack target)\n" +
			"(minion attack target set from hitting enemies with whips or a weapon that is converted to summon damage from an enchantment)\n" +
			"(Prevents consuming boss summoning items if spawn rate multiplier, from enchantments, is > 1.6)\n" +
			"(Enemies spawned will be immune to lava/traps)";
		public override int StrengthGroup => 2;
		public override float ScalePercent => -1f;
		public override void GetMyStats() {
			Effects = new() {
				new EnemyMaxSpawns(multiplicative: EnchantmentStrengthData),
				new EnemySpawnRate(multiplicative: EnchantmentStrengthData)
			};

			AllowedList = new Dictionary<EItemType, float>() {
				{ EItemType.Weapons, 1f },
				{ EItemType.Armor, 1f },
				{ EItemType.Accessories, 1f },
				{ EItemType.FishingPoles, 1f },
				{ EItemType.Tools, 1f }
			};
		}

		public override string ShortTooltip => $"{Math.Round(EnchantmentStrength * AllowedListMultiplier, 3)}x {EnchantmentTypeName.AddSpaces()}";
		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class WarEnchantmentBasic : WarEnchantment { }
	public class WarEnchantmentCommon : WarEnchantment { }
	public class WarEnchantmentRare : WarEnchantment { }
	public class WarEnchantmentSuperRare : WarEnchantment { }
	public class WarEnchantmentUltraRare : WarEnchantment { }
}
