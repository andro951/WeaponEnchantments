using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MaxLifeSteal : StatEffect, INonVanillaStat {
        public MaxLifeSteal(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public MaxLifeSteal(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new MaxLifeSteal(EStatModifier.Clone());
        }

        public override IEnumerable<object> TooltipArgs => new object[] { base.Tooltip };
		public override string TooltipValue => EStatModifier.PercentMult100Tooltip;//PercentMult100Minus1Tooltip
		public override string Tooltip => StandardTooltip;
        public override EnchantmentStat statName => EnchantmentStat.MaxLifeSteal;
    }
}
