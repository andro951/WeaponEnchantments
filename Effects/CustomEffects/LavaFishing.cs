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
        //public override string Tooltip => $"{EStatModifier.SignPercentMult100Tooltip} {DisplayName} (Allows fishing in lava and has a chance to improve catch rates in lava.  Stacks with other souces.)";
        public override IEnumerable<object> TooltipArgs => new object[] { $"{EStatModifier.PercentMult100Tooltip} {DisplayName}" };
        public override string Tooltip => StandardTooltip;
    }
}
