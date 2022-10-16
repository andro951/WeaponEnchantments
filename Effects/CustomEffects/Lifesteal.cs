using System.Collections.Generic;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.Common.Globals.EnchantedWeapon;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class LifeSteal : StatEffect, INonVanillaStat {
        public LifeSteal(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public LifeSteal(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new LifeSteal(EStatModifier.Clone());
        }

        //public override string Tooltip => $"{EStatModifier.PercentMult100Tooltip} {DisplayName} (remainder is saved to prevent always rounding to 0 for low damage weapons)";
        public override IEnumerable<object> TooltipArgs => new object[] { base.Tooltip };
		public override string TooltipValue => EStatModifier.PercentMult100Tooltip;
		public override string Tooltip => StandardTooltip;
        public override EnchantmentStat statName => EnchantmentStat.LifeSteal;
    }
}
