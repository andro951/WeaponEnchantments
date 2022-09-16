using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class FlatHPRegen : StatEffect, IVanillaStat {
        public FlatHPRegen(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public FlatHPRegen(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new FlatHPRegen(EStatModifier.Clone());
        }
        public override EnchantmentStat statName => EnchantmentStat.LifeRegen;
        public override string DisplayName => "Life Regeneration";
    }
}
