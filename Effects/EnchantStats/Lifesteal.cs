using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Configs;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class LifeSteal : StatEffect {
        public LifeSteal(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base(additive, multiplicative, flat, @base) { }

        public override EditableStat statName => EditableStat.LifeSteal;
        public override string DisplayName { get; } = "Life Steal";

        protected override string modifierToString() {
            string final = "";
            float mult = statModifier.Multiplicative + statModifier.Additive - 2;
            float flats = (statModifier.Base * mult + statModifier.Flat).Percent();

            if (flats > 0) {
                final += $"{s(flats)}{flats}%";
            }
            if (mult > 0) {
                if (final != "") final += ' ';
                final += $"{s(mult)}{mult.Percent()}%";
            }
            return final;
        }
    }
}
