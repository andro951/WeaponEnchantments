using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    /// <summary>
    /// This is for maximum, after charging, mana regen
    /// </summary>
    public class FlatMPRegen : StatEffect, IVanillaStat {
        public FlatMPRegen(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public override EnchantmentStat statType => EnchantmentStat.ManaRegen;
        public override string DisplayName { get; } = "Mana Regeneration";
    }
}
