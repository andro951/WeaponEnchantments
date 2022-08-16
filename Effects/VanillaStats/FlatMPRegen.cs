using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    /// <summary>
    /// This is for maximum, after charging, mana regen
    /// </summary>
    public class FlatMPRegen : StatEffect, IVanillaStat {
        public FlatMPRegen(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base(additive, multiplicative, flat, @base) { }

        public override PlayerStat statName => PlayerStat.ManaRegen;
        public override string DisplayName { get; } = "Mana Regeneration";
    }
}
