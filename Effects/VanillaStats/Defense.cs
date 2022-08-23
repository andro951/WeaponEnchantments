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
    public class DefenseEffect : StatEffect, IVanillaStat {
        public DefenseEffect(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) { }

        public override EnchantmentStat statName => EnchantmentStat.Defense;
        public override string DisplayName => "Defense";
        public override string Tooltip => $"{EStatModifier.SignTooltip} {DisplayName}";
    }
}