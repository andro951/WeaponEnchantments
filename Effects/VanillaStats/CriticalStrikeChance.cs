using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class CriticalStrikeChance : ClassedStatEffect, IVanillaStat {
        public CriticalStrikeChance(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null, DamageClass dc = null) : base(additive, multiplicative, flat * 100f, @base * 100f, dc) {

		}
		public CriticalStrikeChance(EStatModifier eStatModifier, DamageClass dc) : base(eStatModifier, dc) { }
		public override EnchantmentEffect Clone() {
			return new CriticalStrikeChance(EStatModifier.Clone(), damageClass);
		}

		public override EnchantmentStat statName => EnchantmentStat.CriticalStrikeChance;
		public override string TooltipValue => EStatModifier.SignPercentTooltip;
	}
}
