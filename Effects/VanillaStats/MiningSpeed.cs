using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MiningSpeed : ClassedStatEffect, IVanillaStat {
        public MiningSpeed(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null, DamageClass dc = null) : base(additive, multiplicative, flat, @base, dc) {

        }
        
        public override EnchantmentStat statName => EnchantmentStat.MiningSpeed;
        public override string Tooltip => $"{EStatModifier.SmartTooltip} {DisplayName}";
    }
}
