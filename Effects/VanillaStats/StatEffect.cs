using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public abstract class StatEffect : EnchantmentEffect {
        protected StatEffect(StatModifier sm) {
            statModifier = sm;
        }

        public StatModifier statModifier;
        public abstract EditableStat statName { get; }

        protected virtual string modifierToString() {
            string final = "";
            float mult = statModifier.Multiplicative + statModifier.Additive - 2;
            float flats = statModifier.Base * mult + statModifier.Flat;

            if (flats > 0) {
                final += $"{s(flats)}{flats}";
            }
            if (mult > 0) {
                if (final != "") final += ' ';
                final += $"{s(mult)}{mult.Percent()}%";
            }
            return final;
        }
        public override string Tooltip => $"{DisplayName}: {modifierToString()}";
    }
}
