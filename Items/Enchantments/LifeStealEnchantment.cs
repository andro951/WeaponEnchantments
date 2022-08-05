﻿using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments
{
	public abstract class LifeStealEnchantment : Enchantment
	{
		public override EnchantmentEffect[] Effects { get => new EnchantmentEffect[] {
			new LifeSteal(new StatModifier(1, 1, (EnchantmentTier + 1) * 0.005f)),
			new LifeStealEnabler()
		};}

		public override string CustomTooltip => $"(remainder is saved to prevent always rounding to 0 for low damage weapons)";
		public override float ScalePercent => 0.8f;
		public override bool Max1 => true;
		public override float CapacityCostMultiplier => 2f;
		public override Dictionary<EItemType, float> AllowedList => new Dictionary<EItemType, float>() {
			{ EItemType.Weapon, 1f }
		};

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class LifeStealEnchantmentBasic : LifeStealEnchantment { }
	public class LifeStealEnchantmentCommon : LifeStealEnchantment { }
	public class LifeStealEnchantmentRare : LifeStealEnchantment { }
	public class LifeStealEnchantmentSuperRare : LifeStealEnchantment { }
	public class LifeStealEnchantmentUltraRare : LifeStealEnchantment { }

}
