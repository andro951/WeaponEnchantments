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
    /// <summary>
    /// This is for constant mana regen
    /// </summary>
    public class BonusManaRegen : StatEffect, IVanillaStat {
        public BonusManaRegen(DifficultyStrength additive = null, DifficultyStrength multiplicative = null, DifficultyStrength flat = null, DifficultyStrength @base = null) : base(additive, multiplicative, flat, @base) {

        }
        public BonusManaRegen(EStatModifier eStatModifier) : base(eStatModifier) { }
        public override EnchantmentEffect Clone() {
            return new BonusManaRegen(EStatModifier.Clone());
        }
        public override EnchantmentStat statName => EnchantmentStat.BonusManaRegen;
        public override string DisplayName { get; } = "Bonus Mana Regeneration";
    }
}
