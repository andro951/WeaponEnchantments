using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class LavaFishing : StatEffect, INonVanillaStat {
        public LavaFishing(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public LavaFishing(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new LavaFishing(EStatModifier.Clone());
        }
        public override EnchantmentStat statName => EnchantmentStat.LavaFishing;
        public override IEnumerable<object> TooltipArgs => new object[] { base.Tooltip };
		public override string TooltipValue => EStatModifier.PercentMult100Tooltip;
		public override string Tooltip => StandardTooltip;
    }
}
