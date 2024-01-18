using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Effects
{
	public class PercentArmorPenetration : ClassedStatEffect, INonVanillaStat
	{
		public PercentArmorPenetration(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null, DamageClass dc = null) : base(additive, multiplicative, flat, @base, dc) {

		}
		public PercentArmorPenetration(EStatModifier eStatModifier, DamageClass dc) : base(eStatModifier, dc) { }
		public override EnchantmentEffect Clone() {
			return new PercentArmorPenetration(EStatModifier.Clone(), damageClass);
		}

		public override EnchantmentStat statName => EnchantmentStat.PercentArmorPenetration;
		public override string TooltipValue => EStatModifier.SignPercentTooltip;
	}
}
