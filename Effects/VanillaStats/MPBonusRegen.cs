using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    /// <summary>
    /// This is for constant mana regen
    /// </summary>
    public class MPBonusRegen : StatEffect, IVanillaStat {
        public MPBonusRegen(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base((byte)PlayerStat.BonusManaRegen, additive, multiplicative, flat, @base) { }

        public override PlayerStat statName => PlayerStat.BonusManaRegen;
        public override string DisplayName { get; } = "Bonus Mana Regeneration";
    }
}
